using System;
using System.Collections.Generic;
using System.Numerics;
using DailyDuty.Data.Enums;
using DailyDuty.Data.SettingsObjects;
using DailyDuty.Data.SettingsObjects.Weekly;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.Modules.Weekly
{
    internal class DomanEnclave : 
        IConfigurable,
        ICompletable,
        IZoneChangeThrottledNotification,
        ILoginNotification,
        IWeeklyResettable
    {
        public List<int> DonationGoals = new()
        {
            20000,
            25000,
            30000,
            40000
        };

        public void Dispose()
        {

        }

        private DomanEnclaveSettings Settings => Service.Configuration.Current().DomanEnclave;
        public CompletionType Type => CompletionType.Weekly;
        public string HeaderText => "Doman Enclave";
        public GenericSettings GenericSettings => Settings;

        private readonly DalamudLinkPayload domanEnclaveTeleport;

        public DomanEnclave()
        {
            domanEnclaveTeleport = Service.TeleportManager.GetPayload(TeleportPayloads.DomanEnclave);

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
            DrawWeeklyBudgetDropdown();

            Draw.OnLoginReminderCheckbox(Settings, HeaderText);

            Draw.OnTerritoryChangeCheckbox(Settings, HeaderText);

            Draw.Checkbox("Show Donation Amounts", HeaderText, ref Settings.ShowTrackedDonationAmount, "Display a message in chat with the donation amount recorded, useful for debugging");
        }

        private void DrawWeeklyBudgetDropdown()
        {
            ImGui.PushItemWidth(75 * ImGuiHelpers.GlobalScale);

            if (ImGui.BeginCombo("Target Budget", Settings.Budget.ToString("n0"), ImGuiComboFlags.PopupAlignLeft))
            {
                foreach (var element in DonationGoals)
                {
                    bool isSelected = element == Settings.Budget;
                    if (ImGui.Selectable(element.ToString("n0"), isSelected))
                    {
                        Settings.Budget = element;
                    }

                    if (isSelected)
                    {
                        ImGui.SetItemDefaultFocus();
                    }
                }

                ImGui.EndCombo();
            }

            ImGui.Spacing();
        }

        public void EditModeOptions()
        {
            //Draw.EditNumberField("Target Budget", HeaderText, 50, ref Settings.Budget);

            Draw.EditNumberField("Deposited This Week", HeaderText, 50, ref Settings.CurrentEarnings);
        }

        public void DisplayData()
        {
            Draw.NumericDisplay("Deposited This Week", Settings.CurrentEarnings);

            Draw.NumericDisplay("Remaining Budget", Settings.Budget - Settings.CurrentEarnings);
        }

        public bool IsCompleted()
        {
            return Settings.CurrentEarnings >= Settings.Budget;
        }

        public DateTime NextReset
        {
            get => Settings.NextReset;
            set => Settings.NextReset = value;
        }

        void IResettable.ResetThis()
        {
            Settings.CurrentEarnings = 0;
        }
    }
}