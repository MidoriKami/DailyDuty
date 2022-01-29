using DailyDuty.ConfigurationSystem;
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
            CategoryString = "Wondrous Tails";
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
            DutyStartNotification();
            DutyEndNotification();
            RerollNotification();
        }

        private void RerollNotification()
        {
            ImGui.Checkbox("Reroll Alert", ref Settings.RerollNotification);
            ImGuiComponents.HelpMarker("When changing zones, send a notification if you have the maximum number of second chance points.\n" +
                                       "Useful to re-roll the stickers for a chance at better rewards, while preventing over-capping.");
        }

        private void DutyEndNotification()
        {
            ImGui.Checkbox("Duty End Reminder", ref Settings.InstanceEndNotification);
            ImGuiComponents.HelpMarker("When exiting a duty, send a notification if the previous duty is now eligible for a sticker.");
        }

        private void DutyStartNotification()
        {
            ImGui.Checkbox("Duty Start Notifications", ref Settings.InstanceStartNotification);
            ImGuiComponents.HelpMarker("When you join a duty, send a notification if the joined duty is available for a Wondrous Tails sticker.");
        }


        private void PrintBookStatus()
        {
            ImGui.Text("Book Status:");
            ImGui.SameLine();

            if (Settings.NumPlacedStickers == 9)
            {
                ImGui.TextColored(new(0, 255, 0, 255), "Complete");
            }
            else
            {
                ImGui.TextColored(new(255, 0, 0, 100), "Incomplete");
            }
        }

        public override void Dispose()
        {

        }
    }
}
