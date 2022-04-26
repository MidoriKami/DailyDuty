using System.Runtime.Intrinsics.X86;
using DailyDuty.Data.Components;
using DailyDuty.Enums;

namespace DailyDuty.Data.ModuleSettings
{
    public class BeastTribeSettings : GenericSettings
    {
        public int NotificationThreshold = 12;
        public ComparisonMode ComparisonMode;
    }
}