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
using Action = System.Action;
using Condition = DailyDuty.Utilities.Condition;

namespace DailyDuty.Modules
{
    internal unsafe class HuntMarksDailyModule :
        IResettable,
        ILoginNotification,
        IZoneChangeThrottledNotification,
        ICompletable,
        IUpdateable
    {
        public GenericSettings GenericSettings => Settings;
        public CompletionType Type => CompletionType.Daily;
        public string DisplayName => Strings.Module.HuntMarksDailyLabel;

        [Signature("D1 48 8D 0D ?? ?? ?? ?? 48 83 C4 20 5F E9 ?? ?? ?? ??", ScanType = ScanType.StaticAddress)]
        public readonly MobHuntStruct* HuntData = null;
        public Action? ExpandedDisplay => null;

        private static DailyHuntMarksSettings Settings => Service.CharacterConfiguration.DailyHuntMarks;
        private readonly Stopwatch updateStopwatch = new();

        public HuntMarksDailyModule()
        {
            SignatureHelper.Initialise(this);
        }

        public void SendNotification()
        {
            if (!IsCompleted() && !Condition.IsBoundByDuty())
            {
                Chat.Print(Strings.Module.HuntMarksDailyLabel, $"{GetIncompleteCount()} " + Strings.Module.HuntMarksHuntsRemainingLabel);
            }
        }

        DateTime IResettable.GetNextReset() => Time.NextWeeklyReset();

        void IResettable.ResetThis()
        {
            Service.LogManager.LogMessage(ModuleType.HuntMarksDaily, "Daily Reset - Resetting");

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

                case TrackedHuntState.Obtained when !data.Obtained && !AllTargetsKilled(data):
                    Service.LogManager.LogMessage(ModuleType.HuntMarksWeekly, $"{hunt.Type} - Mark Bill Unobtained");
                    hunt.State = TrackedHuntState.Unobtained;
                    Service.CharacterConfiguration.Save();
                    break;

                case TrackedHuntState.Obtained when AllTargetsKilled(data):
                    Service.LogManager.LogMessage(ModuleType.HuntMarksWeekly, $"{hunt.Type} - Mark Bill Complete");
                    hunt.State = TrackedHuntState.Killed;
                    Service.CharacterConfiguration.Save();
                    break;
            }
        }
        
        private int GetIncompleteCount()
        {
            return Settings.TrackedHunts.Count(hunt => hunt.Tracked && hunt.State != TrackedHuntState.Killed);
        }

        public static bool AllTargetsKilled(HuntData data)
        {
            var targetInfo = data.TargetInfo;

            for (int i = 0; i < 5; ++i)
            {
                if (targetInfo[i]?.NeededKills != data.KillCounts[i])
                    return false;
            }
            
            return true;
        }
    }
}
