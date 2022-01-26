using CheapLoc;
using DailyDuty.ConfigurationSystem;
using DailyDuty.System.Modules;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;

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
            var locString = Loc.Localize("NotifyInstanceAvailability", "Instance Notifications");
            var description = Loc.Localize("NotifyInstanceAvailability_Description", "When you join a duty, send a notification if the joined duty is available for a Wondrous Tails sticker.");

            ImGui.Checkbox(locString, ref Settings.InstanceNotification);
            ImGuiComponents.HelpMarker(description);
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
        }

        public override void Dispose()
        {

        }
    }
}
