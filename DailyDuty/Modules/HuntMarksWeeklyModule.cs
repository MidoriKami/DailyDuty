using System;
using System.Diagnostics;
using System.Linq;
using DailyDuty.Data.Components;
using DailyDuty.Data.ModuleSettings;
using DailyDuty.Enums;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.Structs;
using DailyDuty.Utilities;
using Dalamud.Utility.Signatures;
using ImGuiNET;

namespace DailyDuty.Modules
{
    internal unsafe class HuntMarksWeeklyModule :
        IResettable,
        ILoginNotification,
        IZoneChangeThrottledNotification,
        ICompletable,
        IUpdateable
    {
        public GenericSettings GenericSettings => Settings;
        public CompletionType Type => CompletionType.Weekly;
        public string DisplayName => Strings.Module.HuntMarksWeeklyLabel;

        [Signature("D1 48 8D 0D ?? ?? ?? ?? 48 83 C4 20 5F E9 ?? ?? ?? ??", ScanType = ScanType.StaticAddress)]
        public readonly MobHuntStruct* HuntData = null;

        public Action ExpandedDisplay => () =>
        {
            var settings = Service.SystemConfiguration.Windows.Todo;

            foreach (var hunt in Settings.TrackedHunts.Where(hunt => hunt.Tracked && hunt.State != TrackedHuntState.Killed))
            {
                ImGui.TextColored(settings.Colors.IncompleteColor, $"{hunt.Type.GetLabel()}");
            }
        };

        private static WeeklyHuntMarksSettings Settings => Service.CharacterConfiguration.WeeklyHuntMarks;
        private readonly Stopwatch updateStopwatch = new();

        public HuntMarksWeeklyModule()
        {
            SignatureHelper.Initialise(this);
        }

        public void SendNotification()
        {
            if (!IsCompleted() && !Condition.IsBoundByDuty())
            {
                var huntsRemainingLabel = GetIncompleteCount() == 1
                    ? Strings.Module.HuntMarksHuntRemainingSingularLabel
                    : Strings.Module.HuntMarksHuntsRemainingLabel;

                Chat.Print(Strings.Module.HuntMarksWeeklyLabel, $"{GetIncompleteCount()} {huntsRemainingLabel}");
            }
        }

        DateTime IResettable.GetNextReset() => Time.NextWeeklyReset();

        void IResettable.ResetThis()
        {
            Service.LogManager.LogMessage(ModuleType.HuntMarksWeekly, "Weekly Reset - Resetting");

            foreach (var hunt in Settings.TrackedHunts)
            {
                hunt.State = TrackedHuntState.Unobtained;
            }
        }

        public bool IsCompleted() => GetIncompleteCount() == 0;


        public void Update()
        {
            Time.UpdateDelayed(updateStopwatch, TimeSpan.FromSeconds(1), () =>
            {
                foreach (var hunt in Settings.TrackedHunts)
                {
                    UpdateState(hunt);
                }
            });
        }

        private void UpdateState(TrackedHunt hunt)
        {
            var data = HuntData->Get(hunt.Type);

            switch (hunt.State)
            {
                case TrackedHuntState.Unobtained when data.Obtained:
                    Service.LogManager.LogMessage(ModuleType.HuntMarksWeekly, $"{hunt.Type} - Mark Bill Obtained");
                    hunt.State = TrackedHuntState.Obtained;
                    Service.CharacterConfiguration.Save();
                    break;

                case TrackedHuntState.Obtained when !data.Obtained && data.KillCounts[0] != 1:
                    Service.LogManager.LogMessage(ModuleType.HuntMarksWeekly, $"{hunt.Type} - Mark Bill Unobtained");
                    hunt.State = TrackedHuntState.Unobtained;
                    Service.CharacterConfiguration.Save();
                    break;

                case TrackedHuntState.Obtained when data.KillCounts[0] == 1:
                    Service.LogManager.LogMessage(ModuleType.HuntMarksWeekly, $"{hunt.Type} - Mark Killed");
                    hunt.State = TrackedHuntState.Killed;
                    Service.CharacterConfiguration.Save();
                    break;
            }
        }

        private int GetIncompleteCount()
        {
            return Settings.TrackedHunts.Count(hunt => hunt.Tracked && hunt.State != TrackedHuntState.Killed);
        }
    }
}
