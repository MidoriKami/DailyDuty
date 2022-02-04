using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DailyDuty.ConfigurationSystem;
using DailyDuty.Data;
using DailyDuty.System.Utilities;
using Dalamud.Logging;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.UI;
using Lumina.Excel.GeneratedSheets;
using Util = DailyDuty.System.Utilities.Util;
#pragma warning disable CS0649

namespace DailyDuty.System.Modules
{
    internal unsafe class WondrousTailsModule : Module
    {
        protected Weekly.WondrousTailsSettings Settings => Service.Configuration.CharacterSettingsMap[Service.Configuration.CurrentCharacter].WondrousTailsSettings;
        public override string ModuleName => "Wondrous Tails";
        public override GenericSettings GenericSettings => Settings;

        private readonly Stopwatch delayStopwatch = new();

        private uint lastDutyInstanceID = 0;
        private bool lastInstanceWasDuty = false;

        [Signature("88 05 ?? ?? ?? ?? 8B 43 18", ScanType = ScanType.StaticAddress)]
        private readonly WondrousTails* wondrousTailsBasePointer;

        public WondrousTailsModule()
        {
            SignatureHelper.Initialise(this);

            Settings.NumPlacedStickers = wondrousTailsBasePointer->Stickers;
        }

        protected override void OnLoginDelayed()
        {
            if (Settings.NewBookNotification)
            {
                NewBookNotification();
            }
        }

        protected override void AlwaysOnTerritoryChanged(object? sender, ushort e)
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
        }

        protected override void ThrottledOnTerritoryChanged(object? sender, ushort e)
        {
            if (Settings.Enabled == false) return;

            if (Settings.NewBookNotification)
            {
                NewBookNotification();
            }

            if (wondrousTailsBasePointer->SecondChance == 9 && !IsWondrousTailsBookComplete())
            {
                if (RerollValid())
                {
                    Util.PrintWondrousTails("You have 9 Second-chance points, you can re-roll your stickers!");
                }
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

            Util.UpdateDelayed(delayStopwatch, TimeSpan.FromSeconds(5), UpdateNumStamps );
        }

        private void UpdateNumStamps()
        {
            var numStickers = wondrousTailsBasePointer->Stickers;

            if (Settings.NumPlacedStickers != numStickers)
            {
                Settings.NumPlacedStickers = wondrousTailsBasePointer->Stickers;
                Settings.CompletionDate = DateTime.UtcNow;
                Settings.WeeklyKey = wondrousTailsBasePointer->WeeklyKey;
                Service.Configuration.Save();
            }
        }

        private bool RerollValid()
        {
            if (Settings.RerollNotificationTasks)
            {
                for (int i = 0; i < 16; ++i)
                {
                    var status = wondrousTailsBasePointer->TaskStatus(i);
                    if (status == ButtonState.AvailableNow || status == ButtonState.Unavailable)
                        return true;
                }
            }

            if (Settings.RerollNotificationStickers)
            {
                // We can reroll if any tasks are incomplete
                // We can spend re-rolls if we have more than 7 stickers
                if (wondrousTailsBasePointer->Stickers is >= 3 and <= 7)
                {
                    return true;
                }
            }

            return false;
        }

        private void NewBookNotification()
        {
            // If we haven't set the key yet, set it.
            if (Settings.WeeklyKey == 0)
            {
                Settings.WeeklyKey = wondrousTailsBasePointer->WeeklyKey;
                Settings.CompletionDate = DateTime.UtcNow;
                Service.Configuration.Save();
            }

            // If the completion time is before the previous reset
            if (Settings.CompletionDate < Util.NextWeeklyReset().AddDays(-7))
            {
                // And we are still using the old key
                if (Settings.WeeklyKey == wondrousTailsBasePointer->WeeklyKey)
                {
                    Util.PrintWondrousTails("A new Wondrous Tails Book is Available!");
                }
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
