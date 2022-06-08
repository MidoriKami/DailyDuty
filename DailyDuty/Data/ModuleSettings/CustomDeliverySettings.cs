using DailyDuty.Data.Components;
using DailyDuty.Enums;

namespace DailyDuty.Data.ModuleSettings
{
    public class CustomDeliverySettings : GenericSettings
    {
        public int NotificationThreshold = 12;
        public ComparisonMode ComparisonMode;
    }
}