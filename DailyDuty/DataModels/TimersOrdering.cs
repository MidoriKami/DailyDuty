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
            TimersOrdering.Alphabetical => Strings.Common_Alphabetical,
            TimersOrdering.AlphabeticalDescending => $"{Strings.Common_Alphabetical} {Strings.Common_Descending}",
            TimersOrdering.TimeRemaining => Strings.Common_RemainingTime,
            TimersOrdering.TimeRemainingDescending => $"{Strings.Common_RemainingTime} {Strings.Common_Descending}",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}