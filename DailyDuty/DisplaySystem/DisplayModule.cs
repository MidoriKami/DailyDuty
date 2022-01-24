using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Runtime.InteropServices;
using CheapLoc;
using DailyDuty.ConfigurationSystem;
using DailyDuty.DisplaySystem.DisplayTabs;
using Dalamud.Interface;
using Dalamud.Utility;
using ImGuiNET;

namespace DailyDuty.DisplaySystem
{
    internal abstract class DisplayModule : IDisposable
    {
        public string CategoryString = "CategoryString Not Set";

        protected abstract GenericSettings GenericSettings { get; }


        protected virtual void DrawContents()
        {
            var stringEnabled = Loc.Localize("Enabled", "Enabled");
            var stringNotifications = Loc.Localize("Notifications", "Notifications");
            var stringManualEdit = Loc.Localize("Manual Edit", "Manual Edit");

            ImGui.Checkbox($"{stringEnabled}##{CategoryString}", ref GenericSettings.Enabled);
            ImGui.Spacing();

            if (GenericSettings.Enabled)
            {
                ImGui.Indent(15 * ImGuiHelpers.GlobalScale);

                DisplayData();

                if (SettingsTab.EditModeEnabled)
                {
                    ImGui.Indent(15 * ImGuiHelpers.GlobalScale);

                    EditModeOptions();

                    ImGui.Indent(-15 * ImGuiHelpers.GlobalScale);
                }

                ImGui.Checkbox($"{stringNotifications}##{CategoryString}", ref GenericSettings.NotificationEnabled);
                ImGui.Spacing();

                if (GenericSettings.NotificationEnabled == true)
                {
                    ImGui.Indent(15 * ImGuiHelpers.GlobalScale);
                    
                    NotificationOptions();

                    ImGui.Indent(-15 * ImGuiHelpers.GlobalScale);
                }

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
    }
}
