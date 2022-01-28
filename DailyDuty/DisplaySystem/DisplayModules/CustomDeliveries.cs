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
            ImGui.Text($"Remaining Allowances: {Settings.AllowancesRemaining}");
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

            ImGui.PushItemWidth(50 *ImGuiHelpers.GlobalScale);
            ImGui.InputInt($"##{CategoryString}", ref Settings.AllowancesRemaining);
            ImGui.PopItemWidth();

            ImGui.Spacing();
        }

        protected override void NotificationOptions()
        {
            PersistentNotification();
        }

        private void PersistentNotification()
        {
            ImGui.Checkbox($"Persistent Reminders##{CategoryString}", ref Settings.PersistentReminders);
            ImGuiComponents.HelpMarker("Send a chat notification on non-duty area change.");
            ImGui.Spacing();
        }

        public override void Dispose()
        {
        }
    }
}
