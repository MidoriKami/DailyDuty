using DailyDuty.Data.Components;
using DailyDuty.Enums;

namespace DailyDuty.Data.ModuleSettings
{
    public class FashionReportSettings : GenericSettings
    {
        public int AllowancesRemaining = 4;
        public int HighestWeeklyScore = 0;
        public FashionReportMode Mode;
    }
}