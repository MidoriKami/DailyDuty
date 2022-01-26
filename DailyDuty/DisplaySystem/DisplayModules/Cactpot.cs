using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheapLoc;
using DailyDuty.ConfigurationSystem;
using Dalamud.Utility;
using ImGuiNET;

namespace DailyDuty.DisplaySystem.DisplayModules
{
    internal class Cactpot : DisplayModule
    {
        protected Daily.Cactpot Settings => Service.Configuration
            .CharacterSettingsMap[Service.Configuration.CurrentCharacter].CactpotSettings;

        protected override GenericSettings GenericSettings => Settings;

        public Cactpot()
        {
            CategoryString = Loc.Localize("Cactpot", "Cactpot");
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
        }

        protected override void NotificationOptions()
        {
        }

        public override void Dispose()
        {
        }
    }
}
