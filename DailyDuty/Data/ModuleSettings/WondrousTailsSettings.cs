using System;
using DailyDuty.Data.Components;

namespace DailyDuty.Data.ModuleSettings
{
    public class WondrousTailsSettings : GenericSettings
    {
        public int NumPlacedStickers = 0;
        public bool InstanceNotifications = false;
        public bool RerollNotificationStickers = false;
        public bool RerollNotificationTasks = false;
        public bool NewBookNotification = false;
        public DateTime CompletionDate = new();
        public bool BookCompleteNotification = false;
    }
}