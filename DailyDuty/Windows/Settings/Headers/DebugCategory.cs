using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            Draw.Checkbox("Save Debug Printout", HeaderText, ref System.ShowSaveDebugInfo, "Enable to show a debug message whenever Daily Duty saves changes to DailyDuty.json");

            Draw.Checkbox("Enable Debug Output", HeaderText, ref System.EnableDebugOutput, "Enable Messages with the [Debug] tag");

            ImGui.Indent(-15 * ImGuiHelpers.GlobalScale);
        }
    }
}
