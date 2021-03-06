using System;
using DailyDuty.Data.Components;
using DailyDuty.Data.ModuleSettings;
using DailyDuty.Enums;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.Utilities;
using Dalamud.Utility.Signatures;

namespace DailyDuty.Modules
{
    internal class BeastTribeModule :
        ICompletable,
        ILoginNotification,
        IZoneChangeThrottledNotification
    {
        public CompletionType Type => CompletionType.Daily;
        public GenericSettings GenericSettings => Settings;
        public string DisplayName => Strings.Module.BeastTribeLabel;
        public Action? ExpandedDisplay => null;

        private delegate int GetBeastTribeAllowancesDelegate(IntPtr agent);
        private delegate IntPtr GetBeastTribeAgentDelegate();

        [Signature("45 33 C9 48 81 C1 ?? ?? ?? ?? 45 8D 51 02")]
        private readonly GetBeastTribeAllowancesDelegate getBeastTribeAllowance = null!;

        [Signature("E8 ?? ?? ?? ?? 0F B7 DB")]
        private readonly GetBeastTribeAgentDelegate getBeastTribeBasePointer = null!;
        private static BeastTribeSettings Settings => Service.CharacterConfiguration.BeastTribe;

        public BeastTribeModule()
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
                Chat.Print(Strings.Module.BeastTribeLabel, Strings.Module.BeastTribeAllowancesRemainingLabel);
            }
        }

        public int GetRemainingAllowances()
        {
            return getBeastTribeAllowance(getBeastTribeBasePointer());
        }
    }
}
