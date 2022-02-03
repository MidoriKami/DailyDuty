using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.ConfigurationSystem;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;

namespace DailyDuty.DisplaySystem.DisplayTabs.SettingsSubModules
{
    internal class SettingsModule : DisplayModule
    {
        public SettingsModule()
        {
            CategoryString = "General Settings";
        }

        protected override GenericSettings GenericSettings { get; } = new();

        protected override void DrawContents()
        {
            ImGui.Indent(15 * ImGuiHelpers.GlobalScale);

            ImGui.Checkbox($"Enable Temporary Edit Mode##{CategoryString}", ref SettingsTab.EditModeEnabled);
            ImGuiComponents.HelpMarker("Allows you to manually correct the values stored in each of Daily/Weekly tabs.\n" +
                                       "Edit Mode automatically disables when you close this window.\n" +
                                       "Only use Edit Mode to correct errors in other tabs.");
            ImGui.Spacing();

            NotificationDelaySettings();

            ImGui.Indent(-15 * ImGuiHelpers.GlobalScale);
        }

        private void NotificationDelaySettings()
        {
            ImGui.PushItemWidth(30 * ImGuiHelpers.GlobalScale);
            ImGui.InputInt($"##ZoneChangeDelay{CategoryString}",
                ref Service.Configuration.TerritoryUpdateStaggerRate, 0, 0);

            ImGui.PopItemWidth();
            ImGui.SameLine();
            ImGui.Text("Zone Changes Before Resending Notifications");

            ImGuiComponents.HelpMarker("Prevents sending notifications until this many zone changes have happened.\n" +
                                       "1: Notify on Every Zone Change\n" +
                                       "10: Notify on Every 10th Zone change\n" +
                                       "Minimum: 1\n" +
                                       "Maximum: 10");
            ImGui.Spacing();


            if (Service.Configuration.TerritoryUpdateStaggerRate < 1)
            {
                Service.Configuration.TerritoryUpdateStaggerRate = 1;
            }
            else if (Service.Configuration.TerritoryUpdateStaggerRate > 10)
            {
                Service.Configuration.TerritoryUpdateStaggerRate = 10;
            }
        }

        protected override void DisplayData()
        {
        }

        protected override void EditModeOptions()
        {
        }

        protected override void NotificationOptions()
        {
        }

        public override void Dispose()
        {

        }
    }
}
