using System.Linq;
using DailyDuty.ConfigurationSystem;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.DisplaySystem.DisplayModules
{
    internal class CustomDeliveries : DisplayModule
    {
        private static Weekly.CustomDeliveriesSettings Settings => Service.Configuration.CharacterSettingsMap[Service.Configuration.CurrentCharacter].CustomDeliveriesSettings;
        protected override GenericSettings GenericSettings => Settings;

        public CustomDeliveries()
        {
            CategoryString = "Custom Delivery";
        }

        protected override void DisplayData()
        {
            ImGui.Text($"Remaining Allowances:\t{Settings.AllowancesRemaining}");
        }

        protected override void DisplayOptions()
        {
        }

        protected override void EditModeOptions()
        {
            ImGui.Text("Manually Set Allowances Remaining");
            ImGui.Spacing();

            ImGui.Text("Allowances:");

            ImGui.SameLine();

            ImGui.PushItemWidth(30 *ImGuiHelpers.GlobalScale);
            ImGui.InputInt($"##{CategoryString}", ref Settings.AllowancesRemaining, 0, 0);
            ImGui.PopItemWidth();

            ImGui.Spacing();
        }

        protected override void NotificationOptions()
        {
            OnLoginReminderCheckbox(Settings);
            OnTerritoryChangeCheckbox(Settings);
        }
        
        public override void Dispose()
        {
        }
    }
}
