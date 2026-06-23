using System;
using DailyDuty.Classes;
using DailyDuty.Enums;

namespace DailyDuty.Extensions;

public static class CompletionStatusExtensions {
    extension(CompletionStatus status) {
        public string Description => status switch {
            CompletionStatus.Unknown => Strings.Unknown,
            CompletionStatus.Disabled => Strings.Disabled,
            CompletionStatus.Incomplete => Strings.Incomplete,
            CompletionStatus.Unavailable => Strings.Unavailable,
            CompletionStatus.InProgress => Strings.In_Progress,
            CompletionStatus.Complete => Strings.Complete,
            CompletionStatus.Suppressed => Strings.Suppressed,
            CompletionStatus.ResultsAvailable => Strings.Results_Available,
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }
}
