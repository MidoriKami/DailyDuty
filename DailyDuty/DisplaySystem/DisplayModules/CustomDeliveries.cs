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
            ImGui.Spacing();

            foreach (var (npcID, npcCount) in Settings.DeliveryNPC)
            {
                var npcName = GetNameForNPC(npcID);
                ImGui.Text($"{npcName}: {npcCount}");
            }
        }

        protected override void DisplayOptions()
        {
        }

        protected override void EditModeOptions()
        {
            ImGui.Text("Manually Set Counts");
            ImGui.Spacing();

            foreach (var key in Settings.DeliveryNPC.Keys.ToList())
            {
                var npcName = GetNameForNPC(key);
                int tempCount = (int)Settings.DeliveryNPC[key];

                ImGui.PushItemWidth(30 * ImGuiHelpers.GlobalScale);
                if (ImGui.InputInt($"##{CategoryString}{key}", ref tempCount, 0, 0))
                {
                    if (Settings.DeliveryNPC[key] != tempCount)
                    {
                        if (tempCount is >= 0 and <= 6)
                        {
                            Settings.DeliveryNPC[key] = (uint)tempCount;
                            Service.Configuration.Save();
                        }
                    }
                }

                ImGui.PopItemWidth();

                ImGui.SameLine();

                ImGui.Text($"{npcName}");
            }
        }

        private string GetNameForNPC(uint id)
        {
            var npcData = Service.DataManager.GetExcelSheet<NotebookDivision>()
                !.GetRow(id);

            return npcData!.Name;
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
