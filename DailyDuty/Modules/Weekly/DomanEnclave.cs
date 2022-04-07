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
        ILoginNotification,
        IUpdateable,
        IWeeklyResettable
    {
        private DomanEnclaveSettings Settings => Service.Configuration.Current().DomanEnclave;
        public CompletionType Type => CompletionType.Weekly;
        public string HeaderText => "Doman Enclave";
        public GenericSettings GenericSettings => Settings;

        public DateTime NextReset
        {
            get => Settings.NextReset;
            set => Settings.NextReset = value;
        }

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

            if (ModuleInitialized() == false)
            {
                Chat.Print(HeaderText, "Module needs to be initialized");
                Chat.Print(HeaderText, "Please visit the Doman Enclave once");
            }
            else if (IsCompleted() == false)
            {
                Chat.Print(HeaderText, $"{GetRemainingBudget():n0} gil Remaining", domanEnclaveTeleport);
            }
        }

        public void NotificationOptions()
        {
            Draw.OnLoginReminderCheckbox(Settings);

            Draw.OnTerritoryChangeCheckbox(Settings);
        }

        public void EditModeOptions()
        {
            if (ImGui.Button("Reset Stored Data"))
            {
                Settings.DonatedThisWeek = 0;
                Settings.WeeklyAllowance = 0;
                Service.Configuration.Save();
            }
        }

        public void DisplayData()
        {
            if (ModuleInitialized() == false)
            {
                ImGui.TextColored(Colors.SoftRed, "Module not initialized\n" +
                                                  "Please visit the Doman Enclave");

                ImGui.TextColored(Colors.SoftRed, "This only needs to be done once");
            }
            else
            {
                Draw.NumericDisplay("Deposited This Week", $"{Settings.DonatedThisWeek:n0}");

                Draw.NumericDisplay("Weekly Allowance", $"{Settings.WeeklyAllowance:n0}");

                Draw.NumericDisplay("Remaining Budget", $"{GetRemainingBudget():n0}");
            }
        }

        public bool IsCompleted() => ModuleInitialized() == true && GetRemainingBudget() == 0;

        public void Update()
        {
            if (DataAvailable() == true)
            {
                UpdateWeeklyAllowance();
                UpdateDonatedThisWeek();
            }
        }

        //
        //  Implementation
        //

        private void UpdateWeeklyAllowance()
        {
            var allowance = GetWeeklyAllowance();

            if (Settings.WeeklyAllowance != allowance)
            {
                Settings.WeeklyAllowance = allowance;
                Service.Configuration.Save();
            }
        }
        private void UpdateDonatedThisWeek()
        {
            var donatedThisWeek = GetDonatedThisWeek();

            if (Settings.DonatedThisWeek != donatedThisWeek)
            {
                Settings.DonatedThisWeek = donatedThisWeek;
                Service.Configuration.Save();
            }
        }

        private ushort GetDonatedThisWeek()
        {
            var baseAddress = getBasePointer();
            var donatedThisWeek = *((ushort*) baseAddress + 80);

            return donatedThisWeek;
        }

        private ushort GetWeeklyAllowance()
        {
            var baseAddress = getBasePointer();
            var adjustedAddress = baseAddress + 166;

            var allowance = *(ushort*) adjustedAddress;

            return allowance;
        }

        private int GetRemainingBudget() => Settings.WeeklyAllowance - Settings.DonatedThisWeek;

        private bool ModuleInitialized() => Settings.WeeklyAllowance != 0;

        private bool DataAvailable() => GetWeeklyAllowance() != 0;

        void IResettable.ResetThis() => Settings.DonatedThisWeek = 0;
    }
}