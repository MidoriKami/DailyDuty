using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Data.Components;
using DailyDuty.Data.ModuleSettings;
using DailyDuty.Enums;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.Utilities;
using Dalamud.Utility.Signatures;

namespace DailyDuty.Modules
{
    internal unsafe class BeastTribeModule :
        ICompletable,
        ILoginNotification,
        IZoneChangeThrottledNotification
    {
        public CompletionType Type { get; }
        public GenericSettings GenericSettings => Settings;

        [Signature("45 33 C9 48 81 C1 ?? ?? ?? ?? 45 8D 51 02")]
        private readonly delegate* unmanaged<IntPtr, int> getBeastTribeAllowance = null!;

        [Signature("E8 ?? ?? ?? ?? 0F B7 DB")]
        private readonly delegate* unmanaged<IntPtr> getBeastTribeBasePointer = null!;
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
                Chat.Print(Strings.Module.BeastTribeLabel, $"{GetRemainingAllowances()} " + Strings.Module.BeastTribeAllowancesRemainingLabel);
            }
        }

        public int GetRemainingAllowances()
        {
            return getBeastTribeAllowance(getBeastTribeBasePointer());
        }
    }
}
