using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheapLoc;
using DailyDuty.ConfigurationSystem;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Utility;
using ImGuiNET;

namespace DailyDuty.DisplaySystem.DisplayModules
{
    internal class MiniCactpot : DisplayModule
    {
        protected Daily.Cactpot Settings => Service.Configuration.CharacterSettingsMap[Service.Configuration.CurrentCharacter].MiniCactpotSettings;

        protected override GenericSettings GenericSettings => Settings;

        public MiniCactpot()
        {
            CategoryString = Loc.Localize("MiniCactpot", "MiniCactpot");
        }

        protected override void DisplayData()
        {
            var locString = Loc.Localize("Tickets Remaining", "Tickets Remaining: {0}").Format(Settings.TicketsRemaining);
            ImGui.Text(locString);
        }

        protected override void DisplayOptions()
        {
        }

        protected override void EditModeOptions()
        {
            ImGui.PushItemWidth(30 * ImGuiHelpers.GlobalScale);

            var locString = Loc.Localize("MiniCactpotOverride", "Override Ticket Count: ");
            ImGui.Text(locString);

            ImGui.SameLine();

            if (ImGui.InputInt($"##{CategoryString}", ref Settings.TicketsRemaining, 0, 0))
            {
                if (Settings.TicketsRemaining > 3)
                {
                    Settings.TicketsRemaining = 3;
                }
                else if (Settings.TicketsRemaining < 0)
                {
                    Settings.TicketsRemaining = 0;
                }

                Service.Configuration.Save();
            }

            ImGui.PopItemWidth();


        }

        protected override void NotificationOptions()
        {
            DrawPersistentNotificationCheckBox();
        }

        public override void Dispose()
        {
        }

        private void DrawPersistentNotificationCheckBox()
        {
            var locString = Loc.Localize("MiniCactpotPersistentReminders", "Persistent Reminder##MiniCactpot");
            var description = Loc.Localize("MiniCactpotPersistentNotificationDescription", "Show persistent reminder if Mini MiniCactpot Tickets are available.");

            ImGui.Checkbox(locString, ref Settings.PersistentReminders);
            ImGuiComponents.HelpMarker(description);
            ImGui.Spacing();
        }
    }
}
