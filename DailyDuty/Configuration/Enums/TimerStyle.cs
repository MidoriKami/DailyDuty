using System;
using System.Collections.Generic;

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
        var parts = new List<string>();

        if (style.HasFlag(TimerStyle.Days)) parts.Add("DD");
        if (style.HasFlag(TimerStyle.Hours)) parts.Add("HH");
        if (style.HasFlag(TimerStyle.Minutes)) parts.Add("MM");
        if (style.HasFlag(TimerStyle.Seconds)) parts.Add("SS");

        return string.Join(":", parts);
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