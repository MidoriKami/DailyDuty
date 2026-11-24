using DailyDuty.Utilities;
using Lumina.Excel.Sheets;

namespace DailyDuty.Extensions;

public static class LuminaExtensions {
    extension(ContentsNote contentsNote) {
        public bool IsComplete => Excel.IsComplete<ContentsNote>(contentsNote.RowId);
    }

    extension(ContentRoulette contentRoulette) {
        public bool IsComplete => Excel.IsComplete<ContentRoulette>(contentRoulette.RowId);
    }

    extension(MobHuntOrderType mobHuntOrderType) {
        public bool IsComplete => Excel.IsComplete<MobHuntOrderType>(mobHuntOrderType.RowId);
    }
}
