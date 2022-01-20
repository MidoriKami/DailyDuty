using DailyDuty.ConfigurationSystem;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.DisplaySystem.DisplayModules
{
    internal class CustomDeliveries : DisplayModule
    {
        private Weekly.CustomDeliveriesSettings settings => Service.Configuration.CharacterSettingsMap[Service.Configuration.CurrentCharacter].CustomDeliveriesSettings;

        public CustomDeliveries()
        {
            CategoryString = "Custom Deliveries";
        }

        protected override void DrawContents()
        {
            ImGui.Checkbox("Enabled##CustomDeliveries", ref settings.Enabled);
            ImGui.Spacing();

            if (settings.Enabled)
            {
                ImGui.Indent(15 * ImGuiHelpers.GlobalScale);

                ImGui.Checkbox("Notifications##CustomDeliveries", ref settings.NotificationEnabled);
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
