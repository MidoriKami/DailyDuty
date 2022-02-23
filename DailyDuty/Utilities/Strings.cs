using System;
using DailyDuty.Data.Graphical;

namespace DailyDuty.Utilities
{
    internal static class Strings
    {
        private static readonly TimerOptions DefaultOptions = new()
        {
            CondensedDisplay = false,
            ShowSeconds = true,
            UseShortName = true
        };

        public static string Format(this TimeSpan span, TimerOptions? options = null)
        {
            options ??= DefaultOptions;

            string result = "";

            if (span.Days > 0)
            {
                if (options.CondensedDisplay)
                {
                    result = $"{span.Days}:";
                }
                else
                {
                    if (span.Days == 1)
                    {
                        result = $"{span.Days} day, ";
                    }
                    else if (span.Days > 1)
                    {
                        result = $"{span.Days} days, ";
                    }
                }
            }

            result += $"{span.Hours:00}:{span.Minutes:00}";

            if (options.ShowSeconds)
            {
                result += $":{span.Seconds:00}";
            }

            return result;
        }
    }
}