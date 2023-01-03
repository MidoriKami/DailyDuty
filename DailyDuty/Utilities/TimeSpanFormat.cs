using System;
using DailyDuty.DataModels;
using DailyDuty.Localization;
using Dalamud.Utility;

namespace DailyDuty.Utilities;

public static class TimeSpanExtensions
{
    public static string FormatTimespan(this TimeSpan span, TimerStyle style)
    {
        return style switch
        {
            // Human Style just shows the highest order nonzero field.
            TimerStyle.Human when span.Days > 1 => Strings.Timers_NumDays.Format(span.Days),
            TimerStyle.Human when span.Days == 1 => Strings.Timers_DayPlusHours.Format(span.Days, span.Hours),
            TimerStyle.Human when span.Hours > 1 => Strings.Timers_NumHours.Format(span.Hours),
            TimerStyle.Human when span.Minutes >= 1 => Strings.Timers_NumMins.Format(span.Minutes),
            TimerStyle.Human => Strings.Timers_NumSecs.Format(span.Seconds),

            TimerStyle.Full => $"{(span.Days >= 1 ? $"{span.Days}." : "")}{span.Hours:D2}:{span.Minutes:D2}:{span.Seconds:D2}",
            TimerStyle.NoSeconds => $"{(span.Days >= 1 ? $"{span.Days}." : "")}{span.Hours:D2}:{span.Minutes:D2}",
            _ => string.Empty
        };
    }
}