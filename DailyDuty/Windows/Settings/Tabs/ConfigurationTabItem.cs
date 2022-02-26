using System.Collections.Generic;
using DailyDuty.Interfaces;
using DailyDuty.Windows.Settings.Headers;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.Windows.Settings.Tabs
{
    internal class ConfigurationTabItem : ITabItem
    {
        private readonly List<ICollapsibleHeader> headers = new()
        {
            new GeneralConfiguration(),
            new EmbeddedTimerConfiguration(),
            new OverlaysConfiguration(),
            new WindowConfiguration(),
            new DebugCategory()
        };

        public static bool EditModeEnabled = false;

        public string TabName => "Configuration";

        public void Draw()
        {
            ImGui.Indent(15 * ImGuiHelpers.GlobalScale);

            Utilities.Draw.Checkbox("Temporary Edit Mode", TabName, ref EditModeEnabled, 
                "Allows you to manually correct the values stored in each of Daily/Weekly tabs\n" +
                "Edit Mode automatically disables when you close this window\n" +
                "Only use Edit Mode to correct errors in other tabs");

            ImGui.Indent(-15 * ImGuiHelpers.GlobalScale);

            foreach (var header in headers)
            {
                header.Draw();
            }
        }

        public void Dispose()
        {
        }
    }
}