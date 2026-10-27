using System.Linq;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Dalamud.Excel.Sheets;

namespace DailyDuty.Extensions;

public static class MobHuntExtensions {
    extension(ref MobHunt huntStruct) {
        public bool IsBillComplete(byte rowId) {
            var availableMarkId = huntStruct.GetAvailableHuntOrderRowId(rowId);
            var obtainedMarkId = huntStruct.GetObtainedHuntOrderRowId(rowId);

            if (availableMarkId != obtainedMarkId) return false;

            var orderData = IDataManager.Get().GetExcelSheet<MobHuntOrderType>().GetRow(rowId);
            var targetRow = orderData.OrderStart.RowId + huntStruct.ObtainedMarkId[rowId] - 1;

            return orderData switch {
                { Type: 2 } => huntStruct.IsEliteMarkComplete(targetRow, rowId),
                { Type: 1 } => huntStruct.IsNormalMarkComplete(targetRow, rowId),
                _ => false,
            };
        }

        private bool IsEliteMarkComplete(uint targetRow, int markId) {
            var eliteTargetInfo = IDataManager.Get().GetSubrowExcelSheet<MobHuntOrder>().GetSubrow(targetRow, 0);
            return huntStruct.CurrentKills[markId][0] == eliteTargetInfo.NeededKills;
        }

        private bool IsNormalMarkComplete(uint targetRow, int markId) {
            foreach (var index in Enumerable.Range(0, 5)) {
                var huntData = IDataManager.Get().GetSubrowExcelSheet<MobHuntOrder>().GetSubrow(targetRow, (ushort)index);

                if (huntStruct.CurrentKills[markId][index] != huntData.NeededKills) return false;
            }

            return true;
        }
    }
}
