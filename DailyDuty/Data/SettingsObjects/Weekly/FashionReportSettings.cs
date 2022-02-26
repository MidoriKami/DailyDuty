using DailyDuty.Data.ModuleData.FashionReport;

namespace DailyDuty.Data.SettingsObjects.Weekly
{
    public class FashionReportSettings : GenericSettings
    {
        public int AllowancesRemaining = 4;
        public int HighestWeeklyScore = 0;
        public FashionReportMode Mode;
    }
}