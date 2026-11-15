using DailyDuty.Enums;
using Dalamud.Excel.Sheets;
using Lumina.Excel;
using Lumina.Text.ReadOnly;

namespace DailyDuty.Utilities;

/// <summary>
/// Extension methods for Excel row types.
/// </summary>
public static class Excel {
    extension<T>(IExcelRow<T> row) where T : struct {
        /// <summary>
        /// Gets the display string for the row, if applicable.
        /// </summary>
        public ReadOnlySeString DisplayString => row switch {
            ContentsNote contentsNote => contentsNote.Name,
            ContentRoulette contentRoulette => contentRoulette.Name,
            _ => string.Empty,
        };

        /// <summary>
        /// Gets the completion status for the row, if applicable.
        /// </summary>
        public CompletionStatus CompletionStatus => row switch {
            ContentsNote contentsNote => contentsNote.IsComplete ? CompletionStatus.Complete : CompletionStatus.Incomplete,
            ContentRoulette contentRoulette => contentRoulette.IsComplete ? CompletionStatus.Complete : CompletionStatus.Incomplete,
            _ => CompletionStatus.Unknown,
        };

        /// <summary>
        /// Gets the completion status as a localized string.
        /// </summary>
        public ReadOnlySeString CompletionStatusString => row.CompletionStatus.ToString();
    }
}
