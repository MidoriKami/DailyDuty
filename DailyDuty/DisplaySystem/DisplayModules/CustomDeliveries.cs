using CheapLoc;
using DailyDuty.ConfigurationSystem;
using DailyDuty.DisplaySystem.DisplayTabs;
using Dalamud.Interface;
using Dalamud.Utility;
using ImGuiNET;

namespace DailyDuty.DisplaySystem.DisplayModules
{
    internal class CustomDeliveries : DisplayModule
    {
        private static Weekly.CustomDeliveriesSettings Settings => Service.Configuration.CharacterSettingsMap[Service.Configuration.CurrentCharacter].CustomDeliveriesSettings;
        protected override GenericSettings GenericSettings => Settings;

        private int manuallySetAllowanceNumber = 0;

        public CustomDeliveries()
        {
            CategoryString = Loc.Localize("CD", "Custom Deliveries");
        }

        protected override void DisplayData()
        {
            var stringNotifications = Loc.Localize("Notifications", "Notifications");

            ImGui.Text(Loc.Localize("CD_Remaining", "Remaining Allowances: {0}").Format(Settings.AllowancesRemaining));
            ImGui.Spacing();
        }

        protected override void DisplayOptions()
        {
        }

        protected override void EditModeOptions()
        {
            var stringSet = Loc.Localize("Set", "Set");

            ImGui.Text(Loc.Localize("CD_Allowances", "Manually Set Number of Allowances"));

            ImGui.PushItemWidth(150 * ImGuiHelpers.GlobalScale);
            ImGui.InputInt("##EditAllowances", ref manuallySetAllowanceNumber, 0, 0);
            ImGui.PopItemWidth();

            ImGui.SameLine();

            BoundedNumberButton(stringSet, 0, 12, ref manuallySetAllowanceNumber, i =>
            {
                Settings.AllowancesRemaining = (uint)manuallySetAllowanceNumber;
                Service.Configuration.Save();
            });
        }

        protected override void NotificationOptions()
        {
        }

        public override void Dispose()
        {
        }
    }
}
