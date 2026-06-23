using System;
using DailyDuty.Classes;
using DailyDuty.Enums;

namespace DailyDuty.Extensions;

public static class CompletionStatusExtensions {
    extension(CompletionStatus status) {
        public string Description => status switch {
            CompletionStatus.Unknown => Strings.CompletionStatus_Unknown,
            CompletionStatus.Disabled => Strings.CompletionStatus_Disabled,
            CompletionStatus.Incomplete => Strings.CompletionStatus_Incomplete,
            CompletionStatus.Unavailable => Strings.CompletionStatus_Unavailable,
            CompletionStatus.InProgress => Strings.CompletionStatus_InProgress,
            CompletionStatus.Complete => Strings.CompletionStatus_Complete,
            CompletionStatus.Suppressed => Strings.CompletionStatus_Suppressed,
            CompletionStatus.ResultsAvailable => Strings.CompletionStatus_ResultsAvailable,
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }
}
