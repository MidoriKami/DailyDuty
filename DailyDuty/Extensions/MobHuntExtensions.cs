using System.Collections.Generic;
using System.Linq;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.Sheets;

namespace DailyDuty.Extensions;

public static class MobHuntExtensions {
    public static IEnumerable<MobHuntOrderType> GetValidHuntMarks(byte targetType) =>
        IDataManager.Get().GetExcelSheet<MobHuntOrderType>()
            .Where(orderType => orderType is { RowId: not 0, Type: var type, EventItem.ValueNullable.Name.ByteLength: > 0 } && type == targetType);

    extension(MobHunt hunt) {
        public bool IsBillComplete(byte rowId) {
            var availableMarkId = hunt.GetAvailableHuntOrderRowId(rowId);
            var obtainedMarkId = hunt.GetObtainedHuntOrderRowId(rowId);

            if (availableMarkId != obtainedMarkId) return false;

            var orderData = IDataManager.Get().GetExcelSheet<MobHuntOrderType>().GetRow(rowId);
            var targetRow = orderData.OrderStart.RowId + hunt.ObtainedMarkId[rowId] - 1;

            return orderData switch {
                { Type: 2 } => hunt.IsEliteMarkComplete(targetRow, rowId),
                { Type: 1 } => hunt.IsNormalMarkComplete(targetRow, rowId),
                _ => false,
            };
        }

        private bool IsEliteMarkComplete(uint targetRow, int markId) {
            var eliteTargetInfo = IDataManager.Get().GetSubrowExcelSheet<MobHuntOrder>().GetSubrow(targetRow, 0);
            return hunt.CurrentKills[markId][0] == eliteTargetInfo.NeededKills;
        }

        private bool IsNormalMarkComplete(uint targetRow, int markId) {
            foreach (ushort index in Enumerable.Range(0, 5)) {
                var huntData = IDataManager.Get().GetSubrowExcelSheet<MobHuntOrder>().GetSubrow(targetRow, index);

                if (hunt.CurrentKills[markId][index] != huntData.NeededKills) return false;
            }

            return true;
        }
    }
}
