namespace DailyDuty.Enums;

public enum CompletionStatus {
    Unknown,
    Disabled,
    Incomplete,
    Unavailable,
    InProgress,
    Complete,
    Suppressed,
    ResultsAvailable,
    NotInitialized,
}

public static class CompletionStatusExtensions {
    extension(CompletionStatus status) {
        /// <summary>
        /// Gets whether this status indicates the user needs to take action.
        /// </summary>
        public bool RequiresAction
            => status is CompletionStatus.NotInitialized or CompletionStatus.Incomplete or CompletionStatus.ResultsAvailable;
    }
}
