using System.Collections.Generic;
using System.Linq;
using CheapLoc;
using DailyDuty.ConfigurationSystem;
using DailyDuty.DisplaySystem.DisplayTabs;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Utility;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using NotImplementedException = System.NotImplementedException;

namespace DailyDuty.DisplaySystem.DisplayModules
{
    internal class CustomDeliveries : DisplayModule
    {
        private static Weekly.CustomDeliveriesSettings Settings => Service.Configuration.CharacterSettingsMap[Service.Configuration.CurrentCharacter].CustomDeliveriesSettings;
        protected override GenericSettings GenericSettings => Settings;

        public CustomDeliveries()
        {
            CategoryString = Loc.Localize("CD", "Custom Deliveries");
        }

        protected override void DisplayData()
        {
            ImGui.Text(Loc.Localize("CD_Remaining", "Remaining Allowances: {0}").Format(Settings.AllowancesRemaining));
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
            var labelString = Loc.Localize("Manually Set Counts", "Manually Set Counts");

            ImGui.Text(labelString);
            ImGui.Spacing();

            foreach (var key in Settings.DeliveryNPC.Keys.ToList())
            {
                var npcName = GetNameForNPC(key);
                int tempCount = (int)Settings.DeliveryNPC[key];

                ImGui.PushItemWidth(30 * ImGuiHelpers.GlobalScale);
                if (ImGui.InputInt($"##{key}", ref tempCount, 0, 0))
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
            var locString = Loc.Localize("PersistentReminder", "Persistent Reminder");
            var description = Loc.Localize("CustomDelivery_PersistentNotification", "Show persistent reminder if custom delivery allowances are available.");

            ImGui.Checkbox(locString, ref Settings.PersistentReminders);
            ImGuiComponents.HelpMarker(description);
            ImGui.Spacing();
        }

        public override void Dispose()
        {
        }
    }
}
