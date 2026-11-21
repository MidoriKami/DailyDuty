using DailyDuty.Enums;
using Lumina.Excel.Sheets;
using Lumina.Excel;
using Lumina.Text.ReadOnly;
using GameContentsNote = FFXIVClientStructs.FFXIV.Client.Game.UI.ContentsNote;
using InstanceContent = FFXIVClientStructs.FFXIV.Client.Game.UI.InstanceContent;
using MobHunt = FFXIVClientStructs.FFXIV.Client.Game.UI.MobHunt;

namespace DailyDuty.Utilities;

/// <summary>
/// Extension methods for Excel row types.
/// </summary>
public static class Excel {

    /// <summary>
    /// Generic IsComplete helper.
    /// </summary>
    public static unsafe bool IsComplete<T>(uint rowId) where T : struct => typeof(T) switch {
        { } t when t == typeof(ContentsNote) => GameContentsNote.Instance()->IsContentNoteComplete((int) rowId),
        { } t when t == typeof(ContentRoulette) => InstanceContent.Instance()->IsRouletteComplete((byte) rowId),
        { } t when t == typeof(MobHuntOrderType) => MobHunt.Instance()->IsBillComplete((byte) rowId),
        _ => false,
    };

    extension<T>(IExcelRow<T> row) where T : struct {
        /// <summary>
        /// Gets the display string for the row, if applicable.
        /// </summary>
        public ReadOnlySeString DisplayString => row switch {
            ContentsNote contentsNote => contentsNote.Name,
            ContentRoulette contentRoulette => contentRoulette.Name,
            ContentFinderCondition contentFinderCondition => contentFinderCondition.Name,
            MobHuntOrderType mobHuntOrderType => mobHuntOrderType.EventItem.ValueNullable?.Name ?? string.Empty,
            _ => string.Empty,
        };

        /// <summary>
        /// Gets the completion status for the row, if applicable.
        /// </summary>
        public CompletionStatus CompletionStatus => row switch {
            ContentsNote contentsNote => contentsNote.IsComplete ? CompletionStatus.Complete : CompletionStatus.Incomplete,
            ContentRoulette contentRoulette => contentRoulette.IsComplete ? CompletionStatus.Complete : CompletionStatus.Incomplete,
            ContentFinderCondition => CompletionStatus.Unknown,
            MobHuntOrderType mobHuntOrderType => mobHuntOrderType.IsComplete ? CompletionStatus.Complete : CompletionStatus.Incomplete,
            _ => CompletionStatus.Unknown,
        };

        /// <summary>
        /// Gets the item ID for the row, if applicable. Returns 0 if no item is available.
        /// </summary>
        public uint ItemId => row switch {
            MobHuntOrderType mobHuntOrderType => mobHuntOrderType.EventItem.RowId,
            _ => 0,
        };
    }
}
