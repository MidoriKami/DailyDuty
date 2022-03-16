using DailyDuty.Data.Enums;

namespace DailyDuty.Data.SettingsObjects.Daily
{
    public class BeastTribeSettings : GenericSettings
    {
        public int NotificationThreshold = 12;
        public int NumberTrackedTribes = 4;

        public BeastTribeMode Mode = BeastTribeMode.TribeTracking;
    }
}