using DailyDuty.Data.SettingsObjects;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.Windows.Settings.Headers
{
    internal class DebugCategory : ICollapsibleHeader
    {
        public void Dispose()
        {
            
        }

        private SystemSettings System => Service.Configuration.System;

        public string HeaderText => "Debug Tools";

        void ICollapsibleHeader.DrawContents()
        {
            ImGui.Indent(15 * ImGuiHelpers.GlobalScale);

            Draw.Checkbox("Save Debug Printout", ref System.ShowSaveDebugInfo, "Enable to show a debug message whenever Daily Duty saves changes to DailyDuty.json");

            Draw.Checkbox("Enable Debug Output", ref System.EnableDebugOutput, "Enable Messages with the [Debug] tag");

            Draw.Checkbox("Show Version Number", ref System.ShowVersionNumber, "Show current Daily Duty Version Number at the bottom of the configuration window");

            ImGui.Indent(-15 * ImGuiHelpers.GlobalScale);
        }
    }
}
