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

namespace DailyDuty.Windows.Settings.SettingsHeaders
{
    internal class DutyFinderOverlayConfiguration : ICollapsibleHeader
    {
        private DutyFinderOverlaySettings Settings => Service.Configuration.DutyFinderOverlaySettings;

        public void Dispose()
        {

        }

        public string HeaderText => "Duty Finder Overlay";

        void ICollapsibleHeader.DrawContents()
        {
            ImGui.Text("Some settings require closing and re-opening the duty finder to take effect");
            ImGui.Spacing();

            Draw.NotificationField("Enable Overlays", HeaderText, ref Settings.Enabled);

            ImGui.Indent(15 * ImGuiHelpers.GlobalScale);

            Draw.NotificationField("Enable Wondrous Tails Overlay", HeaderText, ref Settings.WondrousTailsOverlayEnabled, "Show an indicator in the Duty Finder indicating duties that are available for Wondrous Tails");

            Draw.NotificationField("Enable Duty Roulette Overlay", HeaderText, ref Settings.DutyRouletteOverlayEnabled, "Color tracked duty roulettes to clearly show which are complete/incomplete");
            
            ImGui.Indent(15 * ImGuiHelpers.GlobalScale);

            ImGui.ColorEdit4("Duty Complete Color", ref Settings.DutyRouletteCompleteColor, ImGuiColorEditFlags.NoInputs);
            ImGui.SameLine();
            if (ImGui.Button("Reset##ResetDutyCompleteColor"))
            {
                Settings.DutyRouletteCompleteColor = Colors.HealerGreen;
            }

            ImGui.ColorEdit4("Duty Incomplete Color", ref Settings.DutyRouletteIncompleteColor, ImGuiColorEditFlags.NoInputs);
            ImGui.SameLine();
            if (ImGui.Button("Reset##ResetDutyIncompleteColor"))
            {
                Settings.DutyRouletteIncompleteColor = Colors.DPSRed;
            }

            ImGui.Indent(-15 * ImGuiHelpers.GlobalScale);

            ImGui.Indent(-15 * ImGuiHelpers.GlobalScale);
        }
    }
}
