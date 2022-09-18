using System;
using DailyDuty.Localization;

namespace DailyDuty.Configuration.Components;

public enum FashionReportMode
{
    Single,
    Plus80,
    All
}

public static class FashionReportModeExtensions
{
    public static string GetTranslatedString(this FashionReportMode mode)
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