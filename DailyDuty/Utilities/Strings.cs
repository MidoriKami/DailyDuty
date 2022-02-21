using System;

namespace DailyDuty.Utilities;

internal static class Strings
{
    public static bool HideSeconds => Service.Configuration.TimersWindowSettings.HideSeconds;
    public static bool ShortStrings => Service.Configuration.TimersWindowSettings.ShortStrings;
    public static string FormatHours(this TimeSpan span)
    {
        if (HideSeconds)
        {
            return $"{span.Hours:00}:{span.Minutes:00}";
        }
        else
        {
            return $"{span.Hours:00}:{span.Minutes:00}:{span.Seconds:00}";
        }
        
    }

    public static string FormatDays(this TimeSpan span)
    {
        string daysDisplay = ""; 
        
        if (ShortStrings)
        {
            if (span.Days >= 1)
            {
                daysDisplay = $"{span.Days}:";
            }
        }
        else
        {
            if (span.Days == 1)
            {
                daysDisplay = $"{span.Days} day, ";
            }
            else if (span.Days > 1)
            {
                daysDisplay = $"{span.Days} days, ";
            }
        }

        if (HideSeconds)
        {
            return $"{daysDisplay}{span.Hours:00}:{span.Minutes:00}";
        }
        else
        {
            return $"{daysDisplay}{span.Hours:00}:{span.Minutes:00}:{span.Seconds:00}";
        }
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