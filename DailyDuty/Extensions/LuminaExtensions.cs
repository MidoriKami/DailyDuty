using Dalamud.Excel.Sheets;
using InstanceContent = FFXIVClientStructs.FFXIV.Client.Game.UI.InstanceContent;
using GameContentsNote = FFXIVClientStructs.FFXIV.Client.Game.UI.ContentsNote;

namespace DailyDuty.Extensions;

public static class LuminaExtensions {
    extension(ContentsNote contentsNote) {
        public unsafe bool IsComplete
            => GameContentsNote.Instance()->IsContentNoteComplete((int) contentsNote.RowId);
    }

    extension(ContentRoulette contentRoulette) {
        public unsafe bool IsComplete
            => InstanceContent.Instance()->IsRouletteComplete((byte) contentRoulette.RowId);
    }
}
