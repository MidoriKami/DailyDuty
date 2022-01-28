using DailyDuty.ConfigurationSystem;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;

namespace DailyDuty.DisplaySystem.DisplayTabs
{
    internal class SettingsTab : TabCategory
    {
        public static bool EditModeEnabled = false;

        public SettingsTab()
        {
            TabName = "Settings";
            CategoryName = "Daily Duty Settings";
            FrameID = (int) DisplayManager.Tab.Settings;

            Modules = new()
            {
                new SettingsModule()
            };
        }

        public class SettingsModule : DisplayModule
        {
            public SettingsModule()
            {
                CategoryString = "General Settings";
            }

            protected override GenericSettings GenericSettings { get; } = new();

            protected override void DrawContents()
            {
                ImGui.Indent(15 * ImGuiHelpers.GlobalScale);

                ImGui.Checkbox($"Enable Edit Mode##{CategoryString}", ref EditModeEnabled);
                ImGuiComponents.HelpMarker("Allows you to manually correct the values stored in each of Daily/Weekly tabs.\n" +
                                           "Edit Mode automatically disables when you close this window.\n" +
                                           "Only use Edit Mode to correct errors in other tabs.");
                ImGui.Spacing();

                ImGui.Indent(-15 * ImGuiHelpers.GlobalScale);
            }

            protected override void DisplayData()
            {
            }

            protected override void DisplayOptions()
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
}
