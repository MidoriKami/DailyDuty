using DailyDuty.ConfigurationSystem;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.DisplaySystem.DisplayModules
{
    internal class CustomDeliveries : DisplayModule
    {
        private Weekly.CustomDeliveriesSettings settings => Service.Configuration.CharacterSettingsMap[Service.Configuration.CurrentCharacter].CustomDeliveriesSettings;
        private int ManuallySetAllowanceNumber = 0;

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
                ImGui.Checkbox("Manual Edit##EditCustomDeliveries", ref settings.EditMode);

                if (settings.EditMode)
                {
                    ImGui.Text("Manually Set Number of Allowances");

                    ImGui.PushItemWidth(150 * ImGuiHelpers.GlobalScale);
                    ImGui.InputInt("##EditAllowances", ref ManuallySetAllowanceNumber, 0, 0);
                    ImGui.PopItemWidth();

                    ImGui.SameLine();

                    if (ImGui.Button("Set##SetCustomDeliveries", ImGuiHelpers.ScaledVector2(75, 25)))
                    {
                        if (ManuallySetAllowanceNumber > 12)
                        {
                            ManuallySetAllowanceNumber = 12;
                        }
                        else if (ManuallySetAllowanceNumber < 0)
                        {
                            ManuallySetAllowanceNumber = 0;
                        }

                        settings.AllowancesRemaining = (uint)ManuallySetAllowanceNumber;
                        Service.Configuration.Save();
                    }
                }

                ImGui.Text($"Remaining Allowances: {settings.AllowancesRemaining}");
                ImGui.Spacing();

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
