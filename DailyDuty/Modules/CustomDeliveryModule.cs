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
    internal unsafe class CustomDeliveryModule :
        IZoneChangeThrottledNotification,
        ILoginNotification,
        ICompletable
    {
        public GenericSettings GenericSettings => Settings;
        public CompletionType Type => CompletionType.Weekly;
        private static CustomDeliverySettings Settings => Service.CharacterConfiguration.CustomDelivery;

        private delegate int GetCustomDeliveryAllowancesDelegate(byte* array);

        [Signature("0F B6 41 20 4C 8B C1")]
        private readonly GetCustomDeliveryAllowancesDelegate getCustomDeliveryAllowances = null!;

        [Signature("48 8D 0D ?? ?? ?? ?? 41 0F BA EC", ScanType = ScanType.StaticAddress)]
        private readonly byte* staticArrayPointer = null!;

        public CustomDeliveryModule()
        {
            SignatureHelper.Initialise(this);
        }

        public void SendNotification()
        {
            if (!IsCompleted() && !Condition.IsBoundByDuty())
            {
                Chat.Print(Strings.Module.CustomDeliveryLabel, $"{GetRemainingAllowances()} " + Strings.Common.AllowancesRemainingLabel);
            }
        }

        public bool IsCompleted() => GetRemainingAllowances() == 0;

        public int GetRemainingAllowances()
        {
            return 12 - getCustomDeliveryAllowances(staticArrayPointer);
        }
    }
}
