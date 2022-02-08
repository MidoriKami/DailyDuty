using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Data.Enums;
using DailyDuty.Data.ModuleData.FashionReport;

namespace DailyDuty.Data.SettingsObjects.WeeklySettings;

public class FashionReportSettings : GenericSettings
{
    public int AllowancesRemaining = 4;
    public int HighestWeeklyScore = 0;
    public FashionReportMode Mode;
}