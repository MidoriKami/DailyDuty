using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using DailyDuty.ConfigurationSystem;
using DailyDuty.System.Utilities;
using Dalamud.Game;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.UI;
using Lumina.Excel.GeneratedSheets;
using Util = DailyDuty.System.Utilities.Util;

namespace DailyDuty.System.Modules
{
    internal unsafe class WondrousTailsModule : Module
    {
        [StructLayout(LayoutKind.Explicit)]
        public struct WondrousTails
        {
            [FieldOffset(0x06)]
            public fixed byte Tasks[16];

            [FieldOffset(0x16)]
            public uint Rewards;

            [FieldOffset(0x1A)]
            private readonly ushort _stickers;

            public int Stickers
                => CountSetBits(_stickers);

            [FieldOffset(0x1E)]
            public readonly byte CurrentBookWeek;

            [FieldOffset(0x20)]
            private readonly ushort _secondChance;

            public int SecondChance
                => (_secondChance >> 7) & 0b1111;

            [FieldOffset(0x22)] 
            private fixed byte _taskStatus[4];

            public ButtonState TaskStatus(int idx)
                => (ButtonState) ((_taskStatus[idx >> 2] >> ((idx & 0b11) * 2)) & 0b11);
        }


        protected Weekly.WondrousTailsSettings Settings => Service.Configuration.CharacterSettingsMap[Service.Configuration.CurrentCharacter].WondrousTailsSettings;
        private readonly Stopwatch delayStopwatch = new();

        private uint lastDutyInstanceID = 0;
        private bool lastInstanceWasDuty = false;

        private readonly WondrousTails* wondrousTailsBasePointer;

        public WondrousTailsModule()
        {
            var scanner = new SigScanner();
            wondrousTailsBasePointer = (WondrousTails*) scanner.GetStaticAddressFromSig("88 05 ?? ?? ?? ?? 8B 43 18");

            Settings.NumPlacedStickers = wondrousTailsBasePointer->Stickers;
        }

        protected override void OnLoginDelayed()
        {
        }

        protected override void OnTerritoryChanged(object? sender, ushort e)
        {
            if (Settings.Enabled == false) return;

            if (ConditionManager.IsBoundByDuty() && Settings.InstanceStartNotification == true && !IsWondrousTailsBookComplete())
            {
                lastInstanceWasDuty = true;
                lastDutyInstanceID = e;
                OnDutyStartNotification();
            }
            else if(lastInstanceWasDuty == true && Settings.InstanceEndNotification == true && !IsWondrousTailsBookComplete())
            {
                OnDutyEndNotification();
                lastInstanceWasDuty = false;
            }
            else
            {
                lastInstanceWasDuty = false;
            }

            if (Settings.RerollNotification == true && wondrousTailsBasePointer->SecondChance == 9 && !IsWondrousTailsBookComplete())
            {
                Util.PrintWondrousTails("You have 9 Second-chance points, you can re-roll your stickers!");
            }
        }

        private void OnDutyEndNotification()
        {
            var node = FindNode(lastDutyInstanceID);
            if (node == null) return;

            var buttonState = node.Value.Item1;

            if (buttonState is ButtonState.Completable or ButtonState.AvailableNow)
            {
                Util.PrintWondrousTails("You can claim a stamp for the last instance!");
            }
        }

        private void OnDutyStartNotification()
        {
            var node = FindNode(lastDutyInstanceID);
            if (node == null) return;

            var buttonState = node.Value.Item1;

            switch (buttonState)
            {
                case ButtonState.Unavailable:
                    if (wondrousTailsBasePointer->SecondChance > 0)
                    {
                        Util.PrintWondrousTails($"This instance is available for a stamp if you re-roll it! You have {wondrousTailsBasePointer->SecondChance} Re-Rolls Available.");
                    }
                    break;

                case ButtonState.AvailableNow:
                    Util.PrintWondrousTails("A stamp is already available for this instance.");
                    break;

                case ButtonState.Completable:
                    Util.PrintWondrousTails("Completing this instance will reward you with a stamp!");
                    break;

                case ButtonState.Unknown:
                    break;
            }
        }

        public override void UpdateSlow()
        {
            if (Settings.Enabled == false) return;

            Util.UpdateDelayed(delayStopwatch, TimeSpan.FromSeconds(5), UpdateNumStamps );
        }

        private void UpdateNumStamps()
        {
            var numStickers = wondrousTailsBasePointer->Stickers;

            if (Settings.NumPlacedStickers != numStickers)
            {
                Settings.NumPlacedStickers = wondrousTailsBasePointer->Stickers;
                Service.Configuration.Save();
            }
        }

        private IEnumerable<(ButtonState, List<uint>)> GetAllTaskData()
        {
            var result = new (ButtonState, List<uint>)[16];

            for (int i = 0; i < 16; ++i)
            {
                var taskButtonState = wondrousTailsBasePointer->TaskStatus(i);
                var instances = GetInstanceListFromID(wondrousTailsBasePointer->Tasks[i]);

                result[i] = (taskButtonState, instances);

                #if DEBUG
                    PluginLog.LogDebug($"[WondrousTails] TaskState: {taskButtonState}, Instances: {Util.FormatList(instances)}");
                #endif
            }

            return result;
        }
        
        private static int CountSetBits(int n)
        {
            int count = 0;
            while (n > 0)
            {
                count += n & 1;
                n >>= 1;
            }

            return count;
        }

        public bool IsWondrousTailsBookComplete()
        {
            return wondrousTailsBasePointer->Stickers == 9;
        }

        private (ButtonState, List<uint>)? FindNode(uint instanceID)
        {
            foreach (var (buttonState, instanceList) in GetAllTaskData())
            {
                if (instanceList.Contains(instanceID))
                {
                    return (buttonState, instanceList);
                }
            }

            return null;
        }

        private List<uint> GetInstanceListFromID(uint id)
        {
            var values = TryGetFromDatabase(id);

            if (values != null)
            {
                return new() {values.Value};
            }

            switch (id)
            {
                // Dungeons Lv 1-49
                case 1:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelRequired is >= 1 and <= 49)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Dungeons Lv 50
                case 2:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelRequired is 50)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Dungeons Lv 51-59
                case 3:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelRequired is >= 51 and <= 59)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Dungeons Lv 60
                case 4:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelRequired is 60)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Dungeons Lv 61-69
                case 59:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelRequired is >= 61 and <= 69)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Dungeons Lv 70
                case 60:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelRequired is 70)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Dungeons Lv 71-79
                case 85:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelRequired is >= 71 and <= 79)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Dungeons Lv 80
                case 86:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelRequired is 80)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Dungeons lv 81-89
                case 108:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelRequired is >= 81 and <= 89)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Dungeons Lv 90
                case 109:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelRequired is 90)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                case 53:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId is 21)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Treasure Maps
                case 46:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId is 9)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Rival Wings
                case 67:
                    return new List<uint>();

            }

            PluginLog.Information($"[WondrousTails] Unrecognized ID: {id}");
            return new List<uint>();
        }

        private uint? TryGetFromDatabase(uint id)
        {
            var instanceContentData = Service.DataManager.GetExcelSheet<WeeklyBingoOrderData>()
                !.GetRow(id)
                !.Data;

            if (instanceContentData < 20000)
            {
                return null;
            }

            var data = Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                !.Where(c => c.Content == instanceContentData)
                .Select(c => c.TerritoryType.Value!.RowId)
                .FirstOrDefault();

            return data;
        }

        private AddonWeeklyBingo* GetWondrousTailsPointer()
        {
            return (AddonWeeklyBingo*)Service.GameGui.GetAddonByName("WeeklyBingo", 1);
        }

        public override bool IsCompleted()
        {
            return IsWondrousTailsBookComplete();
        }

        public override void DoDailyReset(Configuration.CharacterSettings settings)
        {
            // Wondrous Tails does not reset daily
        }

        public override void DoWeeklyReset(Configuration.CharacterSettings settings)
        {
            // All data is gathered at runtime, no data is saved
        }
    }
}
