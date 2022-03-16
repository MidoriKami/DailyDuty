using System;
using System.Linq;
using DailyDuty.Data.Enums;
using DailyDuty.Data.SettingsObjects;
using DailyDuty.Data.SettingsObjects.Daily;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Utility.Signatures;
using ImGuiNET;

namespace DailyDuty.Modules.Daily
{
    internal unsafe class BeastTribe : 
        IConfigurable,
        ICompletable,
        ILoginNotification,
        IZoneChangeThrottledNotification
    {
        private BeastTribeSettings Settings => Service.Configuration.Current().BeastTribe;
        public CompletionType Type => CompletionType.Daily;
        public string HeaderText => "Beast Tribe";
        public GenericSettings GenericSettings => Settings;

        [Signature("45 33 C9 48 81 C1 ?? ?? ?? ?? 45 8D 51 02")]
        private readonly delegate* unmanaged<IntPtr, int> getBeastTribeAllowance = null!;

        [Signature("E8 ?? ?? ?? ?? 0F B7 DB")]
        private readonly delegate* unmanaged<IntPtr> getBeastTribeBasePointer = null!;

        private int modeSelect;

        public BeastTribe()
        {
            SignatureHelper.Initialise(this);

            modeSelect = (int) Settings.Mode;
        }

        public void SendNotification()
        {
            if (ModuleComplete() == false && Condition.IsBoundByDuty() == false)
            {
                Chat.Print(HeaderText, $"{GetRemainingAllowances()} Allowances Remaining");
            }
        }

        public void Dispose()
        {
        }

        public void NotificationOptions()
        {
            ModeSelect();

            ModeLogic();

            ImGui.Spacing();

            Draw.OnLoginReminderCheckbox(Settings);

            Draw.OnTerritoryChangeCheckbox(Settings);
        }

        private void ModeLogic()
        {
            switch (Settings.Mode)
            {
                case BeastTribeMode.ManualTracking:
                    ManualTrackingConfiguration();
                    break;

                case BeastTribeMode.TribeTracking:
                    TribeTrackingConfiguration();
                    break;
            }
        }

        private void ModeSelect()
        {
            ImGui.Text("Tracking Mode");

            ImGui.Indent(15 * ImGuiHelpers.GlobalScale);

            ImGui.RadioButton($"Tribe##{HeaderText}", ref modeSelect, (int) BeastTribeMode.TribeTracking);
            ImGuiComponents.HelpMarker("Set how many tribes you wish to spend allowances on\n" +
                                       "and notify if that many allowances haven't been spent yet");

            ImGui.SameLine();

            ImGui.RadioButton($"Manual##{HeaderText}", ref modeSelect, (int) BeastTribeMode.ManualTracking);
            ImGuiComponents.HelpMarker("Send notification if above the set number of allowances");

            ImGui.Indent(-15 * ImGuiHelpers.GlobalScale);

            if (Settings.Mode != (BeastTribeMode) modeSelect)
            {
                Settings.Mode = (BeastTribeMode) modeSelect;
                Service.Configuration.Save();
            }
        }

        private void TribeTrackingConfiguration()
        {
            ImGui.Text("Track Enough Allowance for this many tribes");

            ImGui.SetNextItemWidth(100 * ImGuiHelpers.GlobalScale);
            if (ImGui.BeginCombo("", Settings.NumberTrackedTribes.ToString(), ImGuiComboFlags.PopupAlignLeft))
            {
                foreach (var element in Enumerable.Range(1, 4))
                {
                    bool isSelected = element == Settings.NumberTrackedTribes;
                    if (ImGui.Selectable(element.ToString(), isSelected))
                    {
                        Settings.NumberTrackedTribes = element;
                        Service.Configuration.Save();
                    }

                    if (isSelected)
                    {
                        ImGui.SetItemDefaultFocus();
                    }
                }

                ImGui.EndCombo();
            }
        }

        private void ManualTrackingConfiguration()
        {
            ImGui.Text("Track this many allowances");

            ImGui.SetNextItemWidth(150 * ImGuiHelpers.GlobalScale);
            ImGui.SliderInt("", ref Settings.NotificationThreshold, 0, 11);
        }

        public void EditModeOptions()
        {
        }

        public void DisplayData()
        {
            Draw.NumericDisplay("Allowances", GetRemainingAllowances());

            switch (Settings.Mode)
            {
                case BeastTribeMode.ManualTracking:
                    Draw.NumericDisplay("Tracked Allowances Remaining", Math.Max(GetRemainingAllowances() - Settings.NotificationThreshold, 0));
                    break;

                case BeastTribeMode.TribeTracking:
                    Draw.NumericDisplay("Tracked Allowances Remaining", Math.Max(GetRemainingAllowances() - (12 - Settings.NumberTrackedTribes * 3), 0));
                    break;
            }
        }

        public bool IsCompleted() => ModuleComplete();

        private int GetRemainingAllowances()
        {
            return getBeastTribeAllowance(getBeastTribeBasePointer());
        }

        private bool ModuleComplete()
        {
            switch (Settings.Mode)
            {
                case BeastTribeMode.ManualTracking when GetRemainingAllowances() > Settings.NotificationThreshold:
                case BeastTribeMode.TribeTracking when GetRemainingAllowances() > 12 - (Settings.NumberTrackedTribes * 3):
                    return false;
                
                default:
                    return true;
            }
        }
    }
}