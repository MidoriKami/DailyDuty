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

            var numColumns = Service.Configuration.System.SingleColumnSettings ? 1 : (int)(ImGui.GetWindowSize().X / 250.0f);

            ImGui.BeginTable("Debug Configuration Table)", numColumns);

            ImGui.TableNextColumn();
            Draw.Checkbox("Save Debug Printout", ref System.ShowSaveDebugInfo, "Enable to show a debug message whenever Daily Duty saves changes to DailyDuty.json");

            ImGui.TableNextColumn();
            Draw.Checkbox("Enable Debug Output", ref System.EnableDebugOutput, "Enable Messages with the [Debug] tag");

            ImGui.TableNextColumn();
            Draw.Checkbox("Show Version Number", ref System.ShowVersionNumber, "Show current Daily Duty Version Number at the bottom of the configuration window");

            ImGui.EndTable();

            ImGui.Indent(-15 * ImGuiHelpers.GlobalScale);
        }
    }
}
