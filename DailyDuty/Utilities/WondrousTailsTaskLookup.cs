using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Logging;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.Utilities
{
    internal static class WondrousTailsTaskLookup
    {
        public static List<uint> GetInstanceListFromID(uint id)
        {
            var values = TryGetFromDatabase(id);

            if (values != null)
            {
                return new() {values.Value};
            }

            switch (id)
            {
                // Dungeons Lv 1-49
                case 1:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelRequired is >= 1 and <= 49)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Dungeons Lv 50
                case 2:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelRequired is 50)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Dungeons Lv 51-59
                case 3:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelRequired is >= 51 and <= 59)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Dungeons Lv 60
                case 4:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelRequired is 60)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Dungeons Lv 61-69
                case 59:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelRequired is >= 61 and <= 69)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Dungeons Lv 70
                case 60:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelRequired is 70)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Dungeons Lv 71-79
                case 85:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelRequired is >= 71 and <= 79)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Dungeons Lv 80
                case 86:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelRequired is 80)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Dungeons lv 81-89
                case 108:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelRequired is >= 81 and <= 89)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Dungeons Lv 90
                case 109:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId == 2)
                        .Where(m => m.ClassJobLevelRequired is 90)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                case 53:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId is 21)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Treasure Maps
                case 46:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId is 9)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Rival Wings
                case 67:
                    return new List<uint>();

            }

            PluginLog.Information($"[WondrousTails] Unrecognized ID: {id}");
            return new List<uint>();
        }

        private static uint? TryGetFromDatabase(uint id)
        {
            var instanceContentData = Service.DataManager.GetExcelSheet<WeeklyBingoOrderData>()
                !.GetRow(id)
                !.Data;

            if (instanceContentData < 20000)
            {
                return null;
            }

            var data = Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                !.Where(c => c.Content == instanceContentData)
                .Select(c => c.TerritoryType.Value!.RowId)
                .FirstOrDefault();

            return data;
        }
    }
}
