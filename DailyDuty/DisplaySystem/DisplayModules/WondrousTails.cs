using DailyDuty.ConfigurationSystem;
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

        protected override void EditModeOptions()
        {
        }

        protected override void NotificationOptions()
        {
            NotificationField("Duty Start Notification", ref Settings.InstanceStartNotification, "When you join a duty, send a notification if the joined duty is available for a Wondrous Tails sticker.");

            NotificationField("Duty End Reminder", ref Settings.InstanceEndNotification, "When exiting a duty, send a notification if the previous duty is now eligible for a sticker.");

            NotificationField("Reroll Alert - Stickers", ref Settings.RerollNotificationStickers, "When changing zones, send a notification if you have the maximum number of second chance points and more than 7 stickers.\n" +
                                                                               "Useful to re-roll the stickers for a chance at better rewards, while preventing over-capping.");

            NotificationField("Reroll Alert - Tasks", ref Settings.RerollNotificationTasks, "When changing zones, send a notification if you have the maximum number of second chance points and have a task available to reroll.\n" +
                "Useful to re-roll tasks that you might want to complete later");

            NotificationField("New Book Alert", ref Settings.NewBookNotification, "Notify me that a new book is available if I have a completed book.");

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
