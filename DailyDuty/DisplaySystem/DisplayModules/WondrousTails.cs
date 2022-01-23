using CheapLoc;
using DailyDuty.ConfigurationSystem;
using DailyDuty.System.Modules;
using Dalamud.Interface;
using ImGuiNET;
using NotImplementedException = System.NotImplementedException;

namespace DailyDuty.DisplaySystem.DisplayModules
{
    internal class WondrousTails : DisplayModule
    {
        protected Weekly.WondrousTailsSettings Settings => Service.Configuration.CharacterSettingsMap[Service.Configuration.CurrentCharacter].WondrousTailsSettings;
        protected override GenericSettings GenericSettings => Settings;

        public WondrousTails()
        {
            CategoryString = Loc.Localize("WT", "Wondrous Tails");
        }

        protected override void DisplayData()
        {
            PrintBookStatus();
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

        private void PrintBookStatus()
        {
            var bookStatus = Loc.Localize("Book Status", "Book Status") + ": ";
            var stringComplete = Loc.Localize("Complete", "Complete");
            var stringIncomplete = Loc.Localize("Incomplete", "Incomplete");

            ImGui.Text(bookStatus);
            ImGui.SameLine();

            if (Settings.NumPlacedStickers == 9)
            {
                ImGui.TextColored(new(0, 255, 0, 255), stringComplete);
            }
            else
            {
                ImGui.TextColored(new(255, 0, 0, 100), stringIncomplete);
            }

            ImGui.Spacing();
        }

        public override void Dispose()
        {

        }
    }
}
