using System;
using DailyDuty.Enums;

namespace DailyDuty.Extensions;

public static class CompletionStatusExtensions {
    extension(CompletionStatus status) {
        public string Description => status switch {
            CompletionStatus.Unknown => "Unknown",
            CompletionStatus.Disabled => "Disabled",
            CompletionStatus.Incomplete => "Incomplete",
            CompletionStatus.Unavailable => "Unavailable",
            CompletionStatus.InProgress => "In Progress",
            CompletionStatus.Complete => "Complete",
            CompletionStatus.Suppressed => "Suppressed",
            CompletionStatus.ResultsAvailable => "Results Available",
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }
}
