using System.Collections.Generic;

namespace DailyDuty.Data.Components
{
    public class TimerOptions
    {
        public static readonly List<TimerOptions> OptionsSamples = new()
        {
            new TimerOptions
            {
                CondensedDisplay = false,
                ShowSeconds = true,
                UseShortName = false
            },

            new TimerOptions
            {
                CondensedDisplay = false,
                ShowSeconds = false,
                UseShortName = false
            },

            new TimerOptions
            {
                CondensedDisplay = true,
                ShowSeconds = true,
                UseShortName = false
            },

            new TimerOptions
            {
                CondensedDisplay = true,
                ShowSeconds = false,
                UseShortName = false
            },

            new TimerOptions
            {
                CondensedDisplay = false,
                ShowSeconds = true,
                UseShortName = true
            },

            new TimerOptions
            {
                CondensedDisplay = false,
                ShowSeconds = false,
                UseShortName = true
            },

            new TimerOptions
            {
                CondensedDisplay = true,
                ShowSeconds = true,
                UseShortName = true
            },

            new TimerOptions
            {
                CondensedDisplay = true,
                ShowSeconds = false,
                UseShortName = true
            },
        };

        public bool UseShortName;
        public bool ShowSeconds = true;
        public bool CondensedDisplay;
    }

    public static class TimerOptionsExtensions
    {
        public static string Format(this TimerOptions options)
        {
            var result = options.UseShortName ? "Short: " : "Full Name: ";

            result += options.CondensedDisplay ? "DD:HH:MM" : "Days, HH:MM";

            result += options.ShowSeconds ? ":SS" : "";

            return result;
        }
    }
}
