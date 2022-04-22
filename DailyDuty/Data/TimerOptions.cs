using System.Collections.Generic;

namespace DailyDuty.Data
{
    public class TimerOptions
    {
        public static readonly List<TimerOptions> OptionsSamples = new()
        {
            new()
            {
                CondensedDisplay = false,
                ShowSeconds = true,
                UseShortName = false
            },

            new()
            {
                CondensedDisplay = false,
                ShowSeconds = false,
                UseShortName = false
            },

            new()
            {
                CondensedDisplay = true,
                ShowSeconds = true,
                UseShortName = false
            },

            new()
            {
                CondensedDisplay = true,
                ShowSeconds = false,
                UseShortName = false
            },

            new()
            {
                CondensedDisplay = false,
                ShowSeconds = true,
                UseShortName = true
            },

            new()
            {
                CondensedDisplay = false,
                ShowSeconds = false,
                UseShortName = true
            },

            new()
            {
                CondensedDisplay = true,
                ShowSeconds = true,
                UseShortName = true
            },

            new()
            {
                CondensedDisplay = true,
                ShowSeconds = false,
                UseShortName = true
            },
        };

        public bool UseShortName = false;
        public bool ShowSeconds = true;
        public bool CondensedDisplay = false;
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
