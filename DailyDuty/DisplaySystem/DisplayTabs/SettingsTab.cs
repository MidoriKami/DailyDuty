using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheapLoc;
using DailyDuty.ConfigurationSystem;
using DailyDuty.System;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.DisplaySystem.DisplayTabs
{
    internal class SettingsTab : TabCategory
    {
        public static bool EditModeEnabled = false;

        public SettingsTab()
        {
            TabName = Loc.Localize("Settings", "Settings");
            CategoryName = Loc.Localize("DailyDuty Settings", "Daily Duty Settings");
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
                CategoryString = Loc.Localize("General Settings", "General Settings");
            }

            protected override GenericSettings GenericSettings { get; } = new();

            protected override void DrawContents()
            {
                ImGui.Indent(15 * ImGuiHelpers.GlobalScale);

                string locString = Loc.Localize("Settings EditMode", "Enable Edit Mode");
                ImGui.Checkbox(locString, ref EditModeEnabled);
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
