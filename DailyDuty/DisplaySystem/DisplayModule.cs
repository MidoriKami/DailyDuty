using System;
using DailyDuty.ConfigurationSystem;
using DailyDuty.DisplaySystem.DisplayTabs;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;

namespace DailyDuty.DisplaySystem
{
    internal abstract class DisplayModule : IDisposable
    {
        public string CategoryString = "CategoryString Not Set";

        protected abstract GenericSettings GenericSettings { get; }


        protected virtual void DrawContents()
        {

            ImGui.Checkbox($"Enabled##{CategoryString}", ref GenericSettings.Enabled);
            ImGui.Spacing();

            if (GenericSettings.Enabled)
            {
                ImGui.Indent(15 * ImGuiHelpers.GlobalScale);

                DisplayData();

                ImGui.Spacing();

                if (SettingsTab.EditModeEnabled)
                {
                    ImGui.Indent(15 * ImGuiHelpers.GlobalScale);

                    EditModeOptions();
                    ImGui.Spacing();

                    ImGui.Indent(-15 * ImGuiHelpers.GlobalScale);
                }

                NotificationOptions();
                ImGui.Spacing();

                DisplayOptions();

                ImGui.Indent(-15 * ImGuiHelpers.GlobalScale);
            }

            ImGui.Spacing();
        }

        protected abstract void DisplayData();
        protected abstract void DisplayOptions();
        protected abstract void EditModeOptions();
        protected abstract void NotificationOptions();

        public virtual void Draw()
        {
            if (ImGui.CollapsingHeader(CategoryString))
            {
                ImGui.Spacing();

                DrawContents();
            }
        }

        public abstract void Dispose();

        protected void OnLoginReminderCheckbox(GenericSettings settings)
        {
            ImGui.Checkbox($"Login Reminder##{CategoryString}", ref settings.LoginReminder);
            ImGuiComponents.HelpMarker("Display this module's status in chat on login if this module is incomplete.");
        }

        protected void OnTerritoryChangeCheckbox(GenericSettings settings)
        {
            ImGui.Checkbox($"Zone Change Reminder##{CategoryString}", ref settings.TerritoryChangeReminder);
            ImGuiComponents.HelpMarker("Display this module's status in chat on any non-duty instance change if this module is incomplete.");
        }
    }
}
