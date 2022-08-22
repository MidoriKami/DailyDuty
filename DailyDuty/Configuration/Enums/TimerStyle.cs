using System;
using System.Collections.Generic;
using System.Text;

namespace DailyDuty.Configuration.Enums;

[Flags]
public enum TimerStyle
{
    Days = 0b1000,
    Hours = 0b0100,
    Minutes = 0b0010,
    Seconds = 0b0001,

    Full = Days | Hours | Minutes | Seconds,
    DaysNoSeconds = Days | Hours | Minutes,
    NoDays = Hours | Minutes | Seconds,
    NoDaysNoSeconds = Hours | Minutes,
}

public static class TimerStyleExtensions
{
    public static string GetLabel(this TimerStyle style)
    {
        if (style == 0)
            return string.Empty;

        var sb = new StringBuilder(16);
        if (style.HasFlag(TimerStyle.Days)) sb.Append($"DD").Append('.');
        if (style.HasFlag(TimerStyle.Hours)) sb.Append($"HH").Append(':');
        if (style.HasFlag(TimerStyle.Minutes)) sb.Append($"MM").Append(':');
        if (style.HasFlag(TimerStyle.Seconds)) sb.Append($"SS").Append(':');

        return sb.ToString(0, sb.Length - 1);
    }

    public static IEnumerable<TimerStyle> GetConfigurableStyles()
    {
        return new List<TimerStyle>()
        {
            TimerStyle.Full,
            TimerStyle.DaysNoSeconds,
            TimerStyle.NoDays,
            TimerStyle.NoDaysNoSeconds
        };
    }
}