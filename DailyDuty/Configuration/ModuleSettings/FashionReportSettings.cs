using System;
using DailyDuty.Configuration.Components;
using DailyDuty.System.Localization;

namespace DailyDuty.Configuration.ModuleSettings;

public enum FashionReportMode
{
    Single,
    Plus80,
    All
}

public static class FashionReportModeExtensions
{
    public static string GetLocalizedString(this FashionReportMode mode)
    {
        return mode switch
        {
            FashionReportMode.Single => Strings.Module.FashionReport.ModeSingle,
            FashionReportMode.Plus80 => Strings.Module.FashionReport.Mode80Plus,
            FashionReportMode.All => Strings.Module.FashionReport.ModeAll,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };
    }
}

public class FashionReportSettings : GenericSettings
{
    public int AllowancesRemaining = 4;
    public int HighestWeeklyScore = 0;
    public Setting<FashionReportMode> Mode = new(FashionReportMode.Single);
    public Setting<bool> EnableClickableLink = new(false);
}