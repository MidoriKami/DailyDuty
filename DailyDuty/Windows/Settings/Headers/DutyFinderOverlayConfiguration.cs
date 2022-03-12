using DailyDuty.Data.SettingsObjects.Addons;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.Windows.Settings.Headers
{
    internal class DutyFinderOverlayConfiguration : ICollapsibleHeader
    {
        private DutyFinderAddonSettings Settings => Service.Configuration.Addons.DutyFinder;

        public void Dispose()
        {

        }

        public string HeaderText => "Duty Finder Overlay";

        void ICollapsibleHeader.DrawContents()
        {
            ImGui.Text("Some settings require closing and re-opening the duty finder to take effect");
            ImGui.Spacing();

            Draw.Checkbox("Enable Overlays", ref Settings.Enabled);

            ImGui.Indent(15 * ImGuiHelpers.GlobalScale);

            Draw.Checkbox("Enable Wondrous Tails Overlay", ref Settings.WondrousTailsOverlayEnabled, "Show an indicator in the Duty Finder indicating duties that are available for Wondrous Tails");

            Draw.Checkbox("Enable Duty Roulette Overlay", ref Settings.DutyRouletteOverlayEnabled, "Color tracked duty roulettes to clearly show which are complete/incomplete");
            
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
