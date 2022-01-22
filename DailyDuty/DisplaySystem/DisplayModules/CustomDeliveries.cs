using CheapLoc;
using DailyDuty.ConfigurationSystem;
using Dalamud.Interface;
using Dalamud.Utility;
using ImGuiNET;

namespace DailyDuty.DisplaySystem.DisplayModules
{
    internal class CustomDeliveries : DisplayModule
    {
        private Weekly.CustomDeliveriesSettings settings => Service.Configuration.CharacterSettingsMap[Service.Configuration.CurrentCharacter].CustomDeliveriesSettings;
        private int ManuallySetAllowanceNumber = 0;

        public CustomDeliveries()
        {
            CategoryString = Loc.Localize("CD", "Custom Deliveries");
        }

        protected override void DrawContents()
        {
            var stringEnabled = Loc.Localize("Enabled", "Enabled");
            var stringSet = Loc.Localize("Set", "Set");
            var stringNotifications = Loc.Localize("Notifications", "Notifications");
            var stringManualEdit = Loc.Localize("Manual Edit", "Manual Edit");

            ImGui.Checkbox($"{stringEnabled}##CustomDeliveries", ref settings.Enabled);
            ImGui.Spacing();

            if (settings.Enabled)
            {
                ImGui.Indent(15 * ImGuiHelpers.GlobalScale);
                ImGui.Checkbox($"{stringManualEdit}##EditCustomDeliveries", ref settings.EditMode);

                if (settings.EditMode)
                {
                    ImGui.Text(Loc.Localize("CD_Allowances", "Manually Set Number of Allowances"));

                    ImGui.PushItemWidth(150 * ImGuiHelpers.GlobalScale);
                    ImGui.InputInt("##EditAllowances", ref ManuallySetAllowanceNumber, 0, 0);
                    ImGui.PopItemWidth();

                    ImGui.SameLine();

                    if (ImGui.Button($"{stringSet}##SetCustomDeliveries", ImGuiHelpers.ScaledVector2(75, 25)))
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

                ImGui.Text(Loc.Localize("CD_Remaining", "Remaining Allowances: {0}").Format(settings.AllowancesRemaining));
                ImGui.Spacing();

                ImGui.Checkbox($"{stringNotifications}##CustomDeliveries", ref settings.NotificationEnabled);
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
