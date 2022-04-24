using System;
using DailyDuty.Data.Components;

namespace DailyDuty.Data.ModuleSettings
{
    public class WondrousTailsSettings : GenericSettings
    {
        public bool InstanceNotifications = false;
        public bool RerollNotificationStickers = false;
        public bool RerollNotificationTasks = false;
        public bool NewBookNotification = false;
        public bool BookCompleteNotification = false;
    }
}