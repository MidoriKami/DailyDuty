using System;
using DailyDuty.Localization;

namespace DailyDuty.DataModels;

public enum TimersOrdering
{
    Alphabetical,
    AlphabeticalDescending,
    TimeRemaining,
    TimeRemainingDescending,
}

public static class TimersOrderingExtensions
{
    public static string GetTranslatedString(this TimersOrdering type)
    {
        return type switch
        {
            TimersOrdering.Alphabetical => Strings.Common.Alphabetical,
            TimersOrdering.AlphabeticalDescending => $"{Strings.Common.Alphabetical} {Strings.Common.Descending}",
            TimersOrdering.TimeRemaining => Strings.Common.TimeRemaining,
            TimersOrdering.TimeRemainingDescending => $"{Strings.Common.TimeRemaining} {Strings.Common.Descending}",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}