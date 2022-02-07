using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DailyDuty.Utilities;

internal static class Strings
{
    public static string FormatHours(this TimeSpan span)
    {
        return $"{span.Hours:00}:{span.Minutes:00}:{span.Seconds:00}";
    }

    public static string FormatDays(this TimeSpan span)
    {
        string daysDisplay = ""; 

        if (span.Days == 1)
        {
            daysDisplay = $"{span.Days} day, ";
        }
        else if (span.Days > 1)
        {
            daysDisplay = $"{span.Days} days, ";
        }

        return $"{daysDisplay}{span.Hours:00}:{span.Minutes:00}:{span.Seconds:00}";
    }

    public static string FormatAuto(this TimeSpan span)
    {
        if (span.Days > 0)
        {
            return FormatDays(span);
        }
        else
        {
            return FormatHours(span);
        }
    }
}