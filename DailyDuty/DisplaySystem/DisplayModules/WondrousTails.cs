using DailyDuty.ConfigurationSystem;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.DisplaySystem.DisplayModules
{
    internal class WondrousTails : DisplayModule
    {
        protected Weekly.WondrousTailsSettings Settings => Service.Configuration.CharacterSettingsMap[Service.Configuration.CurrentCharacter].WondrousTailsSettings;

        public WondrousTails()
        {
            CategoryString = "Wondrous Tails";
        }

        protected override void DrawContents()
        {
            ImGui.Checkbox("Enabled##WondrousTails", ref Settings.Enabled);
            ImGui.Spacing();

            if (Settings.Enabled)
            {
                ImGui.Indent(15 *ImGuiHelpers.GlobalScale);

                ImGui.Checkbox("Notifications##WondrousTails", ref Settings.NotificationEnabled);
                ImGui.Spacing();

                ImGui.Indent(-15 * ImGuiHelpers.GlobalScale);
            }

            ImGui.Spacing();
        }

        public override void Dispose()
        {

        }
    }
}
