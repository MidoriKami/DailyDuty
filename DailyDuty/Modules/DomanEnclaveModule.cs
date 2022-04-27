using System;
using DailyDuty.Data.Components;
using DailyDuty.Data.ModuleSettings;
using DailyDuty.Enums;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.Utilities;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Utility.Signatures;

namespace DailyDuty.Modules
{
    internal unsafe class DomanEnclaveModule :
        ICompletable,
        IZoneChangeThrottledNotification,
        ILoginNotification,
        IResettable,
        IUpdateable
    {
        public CompletionType Type => CompletionType.Weekly;
        public GenericSettings GenericSettings => Settings;
        public string DisplayName => Strings.Module.DomanEnclaveLabel;
        private static DomanEnclaveSettings Settings => Service.CharacterConfiguration.DomanEnclave;

        private delegate IntPtr GetPointerDelegate();

        [Signature("E8 ?? ?? ?? ?? 48 85 C0 74 09 0F B6 B8")]
        private readonly GetPointerDelegate getBasePointer = null!;

        private readonly DalamudLinkPayload domanEnclaveTeleport;

        public DomanEnclaveModule()
        {
            SignatureHelper.Initialise(this);

            domanEnclaveTeleport = Service.TeleportManager.GetPayload(TeleportPayloads.DomanEnclave);
        }

        public bool IsCompleted() => ModuleInitialized() && GetRemainingBudget() == 0;

        public void SendNotification()
        {
            if (!IsCompleted() && !Condition.IsBoundByDuty())
            {
                Chat.Print(Strings.Module.DomanEnclaveLabel, $"{GetRemainingBudget():n0} " + Strings.Module.DomanEnclaveGilRemainingLabel, Settings.EnableClickableLink ? domanEnclaveTeleport : null);
            }
        }

        public void Update()
        {
            if (DataAvailable())
            {
                UpdateWeeklyAllowance();
                UpdateDonatedThisWeek();
            }
        }

        private void UpdateWeeklyAllowance()
        {
            var allowance = GetWeeklyAllowance();

            if (Settings.WeeklyAllowance != allowance)
            {
                Service.LogManager.LogMessage(ModuleType.DomanEnclave, $"Weekly Allowance Updated - {GetWeeklyAllowance()}");
                
                Settings.WeeklyAllowance = allowance;
                Service.CharacterConfiguration.Save();
            }
        }
        private void UpdateDonatedThisWeek()
        {
            var donatedThisWeek = GetDonatedThisWeek();

            if (Settings.DonatedThisWeek != donatedThisWeek)
            {
                Service.LogManager.LogMessage(ModuleType.DomanEnclave, $"Donation Value Updated - {GetDonatedThisWeek()}");
                
                Settings.DonatedThisWeek = donatedThisWeek;
                Service.CharacterConfiguration.Save();
            }
        }

        private ushort GetDonatedThisWeek()
        {
            var baseAddress = getBasePointer();
            var donatedThisWeek = *((ushort*) baseAddress + 80);

            return donatedThisWeek;
        }

        public ushort GetWeeklyAllowance()
        {
            var baseAddress = getBasePointer();
            var adjustedAddress = baseAddress + 166;

            var allowance = *(ushort*) adjustedAddress;

            return allowance;
        }

        public int GetRemainingBudget() => Settings.WeeklyAllowance - Settings.DonatedThisWeek;
        private bool DataAvailable() => GetWeeklyAllowance() != 0;
        private bool ModuleInitialized() => Settings.WeeklyAllowance != 0;
        DateTime IResettable.GetNextReset() => Time.NextWeeklyReset();
        void IResettable.ResetThis() => Settings.DonatedThisWeek = 0;
    }
}
