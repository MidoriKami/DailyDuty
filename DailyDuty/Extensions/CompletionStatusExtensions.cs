using Resources;
using System;
using DailyDuty.Classes;
using DailyDuty.Enums;

namespace DailyDuty.Extensions;

public static class CompletionStatusExtensions {
    extension(CompletionStatus status) {
        public string Description => status switch {
            CompletionStatus.Unknown => Strings.ResourceManager.GetString("Unknown", Strings.Culture) ?? "Unknown",
            CompletionStatus.Disabled => Strings.ResourceManager.GetString("Disabled", Strings.Culture) ?? "Disabled",
            CompletionStatus.Incomplete => Strings.ResourceManager.GetString("Incomplete", Strings.Culture) ?? "Incomplete",
            CompletionStatus.Unavailable => Strings.ResourceManager.GetString("Unavailable", Strings.Culture) ?? "Unavailable",
            CompletionStatus.InProgress => Strings.ResourceManager.GetString("In Progress", Strings.Culture) ?? "In Progress",
            CompletionStatus.Complete => Strings.ResourceManager.GetString("Complete", Strings.Culture) ?? "Complete",
            CompletionStatus.Suppressed => Strings.ResourceManager.GetString("Suppressed", Strings.Culture) ?? "Suppressed",
            CompletionStatus.ResultsAvailable => Strings.ResourceManager.GetString("Results Available", Strings.Culture) ?? "Results Available",
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }
}
