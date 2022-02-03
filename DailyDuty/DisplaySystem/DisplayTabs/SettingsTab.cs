using DailyDuty.ConfigurationSystem;
using DailyDuty.DisplaySystem.DisplayTabs.SettingsSubModules;
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
                new SettingsModule(),
                new ToDoWindowModule()
            };
        }
    }
}
