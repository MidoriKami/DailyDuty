using System;

namespace DailyDuty.Extensions;

/// <summary>
/// Extension methods for TimeSpan formatting.
/// </summary>
public static class TimeSpanExtensions {
    extension(TimeSpan timeSpan) {

        /// <summary>
        /// Formats a TimeSpan as a string in the pattern d.hh:mm or d.hh:mm:ss.
        /// </summary>
        /// <param name="hideSeconds">Whether to exclude seconds from the output.</param>
        public string FormatTime(bool hideSeconds = false)
            => hideSeconds ? $"{timeSpan:d\\.hh\\:mm}" : $"{timeSpan:d\\.hh\\:mm\\:ss}";
    }
}
