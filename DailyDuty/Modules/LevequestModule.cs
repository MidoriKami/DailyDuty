using System;
using DailyDuty.Data.Components;
using DailyDuty.Data.ModuleSettings;
using DailyDuty.Enums;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.Structs;
using DailyDuty.Utilities;
using Dalamud.Utility.Signatures;

namespace DailyDuty.Modules
{
    internal unsafe class LevequestModule :
        ICompletable,
        IZoneChangeThrottledNotification,
        ILoginNotification
    {
        public CompletionType Type => CompletionType.Daily;
        public GenericSettings GenericSettings => Settings;
        private static LevequestSettings Settings => Service.CharacterConfiguration.Levequest;
        public string DisplayName => Strings.Module.LevequestLabel;
        public Action? ExpandedDisplay => null;

        [Signature("88 05 ?? ?? ?? ?? 0F B7 41 06", ScanType = ScanType.StaticAddress)]
        private readonly LevequestStruct* levequestStruct = null;

        public LevequestModule()
        {
            SignatureHelper.Initialise(this);
        }

        public bool IsCompleted()
        {
            switch (Settings.ComparisonMode)
            {
                case ComparisonMode.LessThan when Settings.NotificationThreshold > GetRemainingAllowances():
                case ComparisonMode.EqualTo when Settings.NotificationThreshold == GetRemainingAllowances():
                case ComparisonMode.LessThanOrEqual when Settings.NotificationThreshold >= GetRemainingAllowances():
                    return true;

                default:
                    return false;
            }
        }

        public void SendNotification()
        {
            if (!IsCompleted() && !Condition.IsBoundByDuty())
            {
                Chat.Print(Strings.Module.LevequestLabel, Strings.Module.LevequestAboveThresholdLabel);
            }
        }

        public int GetRemainingAllowances() => levequestStruct->AllowancesRemaining;

        public int GetAcceptedLeves() => levequestStruct->LevesAccepted;
    }
}
