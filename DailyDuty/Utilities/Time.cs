using System;
using Dalamud.Plugin.Services;

namespace DailyDuty.Utilities;

/// <summary>
/// Utility class for calculating game-specific reset times.
/// </summary>
public static class Time {
    private static DateTime GetNextDateTimeForHour(int hours)
        => DateTime.UtcNow.Hour < hours ? DateTime.UtcNow.Date.AddHours(hours) : DateTime.UtcNow.Date.AddDays(1).AddHours(hours);

    /// <summary>
    /// Returns the next daily reset time (15:00 UTC).
    /// </summary>
    public static DateTime NextDailyReset()
        => GetNextDateTimeForHour(15);

    /// <summary>
    /// Returns the next weekly reset time (Tuesday 08:00 UTC).
    /// </summary>
    public static DateTime NextWeeklyReset()
        => NextDayOfWeek(DayOfWeek.Tuesday, 8);

    /// <summary>
    /// Returns the next Fashion Report reset time (Friday 08:00 UTC, when the new Fashion Report week begins).
    /// </summary>
    public static DateTime NextFashionReportReset()
        => NextDayOfWeek(DayOfWeek.Friday, 8);

    /// <summary>
    /// Returns the next Grand Company supply reset time (20:00 UTC).
    /// </summary>
    public static DateTime NextGrandCompanyReset()
        => GetNextDateTimeForHour(20);

    /// <summary>
    /// Returns the next Leve allowance reset time (12:00 UTC or midnight).
    /// </summary>
    public static DateTime NextLeveAllowanceReset() {
        var now = DateTime.UtcNow;

        return now.Hour < 12 ? now.Date.AddHours(12) : now.Date.AddDays(1);
    }

    private static DateTime NextDayOfWeek(DayOfWeek weekday, int hour) {
        var today = DateTime.UtcNow;

        if (today.Hour < hour && today.DayOfWeek == weekday) {
            return today.Date.AddHours(hour);
        }
        var nextReset = today.AddDays(1);

        while (nextReset.DayOfWeek != weekday) {
            nextReset = nextReset.AddDays(1);
        }

        return nextReset.Date.AddHours(hour);
    }

    public class DatacenterException : Exception;

    /// <summary>
    /// Returns the next Jumbo Cactpot reset time based on the player's data center region.
    /// </summary>
    public static DateTime NextJumboCactpotReset() => IPlayerState.Get().HomeWorld.ValueNullable?.DataCenter.ValueNullable?.Region.RowId switch {
        // Japan
        1 => NextDayOfWeek(DayOfWeek.Saturday, 12),

        // North America
        2 => NextDayOfWeek(DayOfWeek.Sunday, 2),

        // Europe
        3 => NextDayOfWeek(DayOfWeek.Saturday, 19),

        // Australia
        4 => NextDayOfWeek(DayOfWeek.Saturday, 9),

        // China
        5 => NextDayOfWeek(DayOfWeek.Saturday, 13),

        // Korea
        6 => NextDayOfWeek(DayOfWeek.Saturday, 12),

        // Cloud
        7 => NextDayOfWeek(DayOfWeek.Sunday, 2),

        // Taiwan
        8 => NextDayOfWeek(DayOfWeek.Saturday, 13),

        // Unknown Region
        _ => throw new DatacenterException(),
    };
}
