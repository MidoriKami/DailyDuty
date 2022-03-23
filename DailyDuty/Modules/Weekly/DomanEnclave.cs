using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using DailyDuty.Data.Enums;
using DailyDuty.Data.SettingsObjects;
using DailyDuty.Data.SettingsObjects.Weekly;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Interface;
using Dalamud.Utility.Signatures;
using ImGuiNET;

namespace DailyDuty.Modules.Weekly
{
    internal unsafe class DomanEnclave : 
        IConfigurable,
        ICompletable,
        IZoneChangeThrottledNotification,
        ILoginNotification
    {
        private DomanEnclaveSettings Settings => Service.Configuration.Current().DomanEnclave;
        public CompletionType Type => CompletionType.Weekly;
        public string HeaderText => "Doman Enclave";
        public GenericSettings GenericSettings => Settings;

        private readonly DalamudLinkPayload domanEnclaveTeleport;

        [Signature("E8 ?? ?? ?? ?? 48 85 C0 74 09 0F B6 B8")]
        private readonly delegate* unmanaged<IntPtr> getBasePointer = null!;

        public DomanEnclave()
        {
            SignatureHelper.Initialise(this);

            domanEnclaveTeleport = Service.TeleportManager.GetPayload(TeleportPayloads.DomanEnclave);
        }

        public void Dispose()
        {

        }

        public void SendNotification()
        {
            if (Condition.IsBoundByDuty() == true) return;

            if (IsCompleted() == false)
            {
                Chat.Print(HeaderText, $"{Settings.Budget - Settings.CurrentEarnings:n0} gil remaining", domanEnclaveTeleport);
            }
        }

        public void NotificationOptions()
        {
            Draw.OnLoginReminderCheckbox(Settings);

            Draw.OnTerritoryChangeCheckbox(Settings);
        }

        public void EditModeOptions()
        {

        }

        public void DisplayData()
        {
            Draw.NumericDisplay("Deposited This Week", GetDonatedThisWeek());

            Draw.NumericDisplay("Weekly Allowance", GetWeeklyAllowance());

            Draw.NumericDisplay("Remaining Budget", GetWeeklyAllowance() - GetDonatedThisWeek());
        }

        public bool IsCompleted()
        {
            return GetWeeklyAllowance() - GetDonatedThisWeek() == 0;
        }

        private short GetDonatedThisWeek()
        {
            var baseAddress = getBasePointer();
            var donatedThisWeek = *((short*) baseAddress + 80);

            return donatedThisWeek;
        }

        private short GetWeeklyAllowance()
        {
            var baseAddress = getBasePointer();
            var adjustedAddress = baseAddress + 166;

            var allowance = *(short*) adjustedAddress;

            return allowance;
        }
    }
}