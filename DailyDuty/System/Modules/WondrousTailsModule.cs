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
            public ushort Stickers;

            [FieldOffset(0x1E)]
            private readonly byte _flags;

            public bool HasBook
                => (_flags & 0x10) != 0;

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
        private readonly Stopwatch loginNoticeStopwatch = new();

        private uint lastDutyInstanceID = 0;
        private bool lastInstanceWasDuty = false;

        private readonly WondrousTails* wondrousTailsBasePointer;

        public WondrousTailsModule()
        {
            Service.ClientState.Login += OnLogin;
            Service.ClientState.TerritoryChanged += OnTerritoryChanged;

            var scanner = new SigScanner();
            wondrousTailsBasePointer = (WondrousTails*) scanner.GetStaticAddressFromSig("88 05 ?? ?? ?? ?? 8B 43 18");
        }

        private void OnLogin(object? sender, EventArgs e)
        {
            if (Settings.Enabled == false) return;

            loginNoticeStopwatch.Start();
        }

        private void OnTerritoryChanged(object? sender, ushort e)
        {
            if (Settings.Enabled == false) return;
            if (loginNoticeStopwatch.IsRunning == true) return;

            if (IsWondrousTailsBookComplete() == true && Settings.NotificationEnabled == true && Service.LoggedIn == true)
            {
                Util.PrintWondrousTails("You have a completed book! Be sure to turn it in!");
            }

            if (ConditionManager.IsBoundByDuty() && Settings.NotificationEnabled == true && !IsWondrousTailsBookComplete())
            {
                lastInstanceWasDuty = true;
                lastDutyInstanceID = e;
                OnDutyStartNotification();
            }
            else if(lastInstanceWasDuty == true)
            {
                OnDutyEndNotification();
                lastInstanceWasDuty = false;
            }
            else
            {
                lastInstanceWasDuty = false;
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

        public override void Update()
        {
            if (Settings.Enabled == false) return;
            if (loginNoticeStopwatch.IsRunning == false) return;

            var frameCount = Service.PluginInterface.UiBuilder.FrameCount;
            if (frameCount % 10 != 0) return;

            if (loginNoticeStopwatch.Elapsed >= TimeSpan.FromSeconds(5) && loginNoticeStopwatch.IsRunning)
            {
                if (IsWondrousTailsBookComplete() == true)
                {
                    Util.PrintWondrousTails("You have a completed book! Be sure to turn it in!");
                }

                loginNoticeStopwatch.Stop();
                loginNoticeStopwatch.Reset();
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
            var playerHasBook = wondrousTailsBasePointer->HasBook;
            var fullNumber = wondrousTailsBasePointer->Stickers;

            var numBits = CountSetBits(fullNumber);

            return numBits == 9 && playerHasBook;
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
                        .Where(m => m.ClassJobLevelSync is >= 1 and <= 49)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Dungeons Lv 50
                case 2:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelSync is 50)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Dungeons Lv 51-59
                case 3:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelSync is >= 51 and <= 59)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Dungeons Lv 60
                case 4:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelSync is 60)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Dungeons Lv 61-69
                case 59:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelSync is >= 61 and <= 69)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Dungeons Lv 70
                case 60:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelSync is 70)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Dungeons Lv 71-79
                case 85:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelSync is >= 71 and <= 79)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Dungeons Lv 80
                case 86:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelSync is 80)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Dungeons lv 81-89
                case 108:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelSync is >= 81 and <= 89)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Dungeons Lv 90
                case 109:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelSync is 90)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                case 53:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId is 21)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Treasure Maps
                case 46:
                    // todo: Find Treasure Map Instance List
                    return new List<uint>();

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

        public override void Dispose()
        {
            Service.ClientState.Login -= OnLogin;
            Service.ClientState.TerritoryChanged -= OnTerritoryChanged;
        }

        public override bool IsCompleted()
        {
            return IsWondrousTailsBookComplete();
        }

        public override void DoDailyReset()
        {
            // Wondrous Tails does not reset daily
        }

        public override void DoWeeklyReset()
        {
            // All data is gathered at runtime, no data is saved
        }
    }
}
