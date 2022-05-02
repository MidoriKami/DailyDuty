using DailyDuty.Data.Components;
using DailyDuty.Enums;

namespace DailyDuty.Data.ModuleSettings
{
    public class LevequestSettings : GenericSettings
    {
        public int NotificationThreshold = 95;
        public ComparisonMode ComparisonMode;
    }
}