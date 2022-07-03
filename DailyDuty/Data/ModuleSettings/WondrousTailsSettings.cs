using DailyDuty.Data.Components;

namespace DailyDuty.Data.ModuleSettings
{
    public class WondrousTailsSettings : GenericSettings
    {
        public bool InstanceNotifications = false;
        public bool EnableOpenBookLink = false;
        public bool StickerAvailableNotification = false;
        public bool UnclaimedBookWarning = true;
        public bool DeadlineEarlyWarning = false;
        public int EarlyWarningDays = 3;
    }
}