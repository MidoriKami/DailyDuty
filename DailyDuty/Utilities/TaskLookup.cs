using System.Collections.Generic;
using System.Linq;
using DailyDuty.Data.Components;
using Dalamud.Logging;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.Utilities
{
    internal static class TaskLookup
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

                // Palace of the Dead / Heaven on High
                case 53:
                    return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                        !.Where(m => m.ContentType.Value?.RowId is 21)
                        .Select(m => m.TerritoryType.Value!.RowId)
                        .ToList();

                // Treasure Maps
                case 46:
                    //return Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                    //    !.Where(m => m.ContentType.Value?.RowId is 9)
                    //    .Select(m => m.TerritoryType.Value!.RowId)
                    //    .ToList();
                    return new List<uint>();

                // The Binding Coil of Bahamut
                case 121:
                    return new List<uint>() { 241, 242, 243, 244, 245 };

                // The Second Coil of Bahamut
                case 122:
                    return new List<uint>() { 355, 356, 357, 358 };

                // The Final Coil of Bahamut
                case 123:
                    return new List<uint>() { 193, 194, 195, 196 };

                // Alexander: Gordias
                case 124:
                    return new List<uint>() { 442, 443, 444, 445 };

                // Alexander: Midas
                case 125:
                    return new List<uint>() {520, 521, 522, 523};

                // Alexander: The Creator
                case 126:
                    return new List<uint>() { 580, 581, 582, 583 };

                // Omega: Deltascape
                case 127:
                    return new List<uint>() { 691, 692, 693, 694 };

                // Omega: Sigmascape
                case 128:
                    return new List<uint>() { 748, 749, 750, 751 };

                // Omega: Alphascape
                case 129:
                    return new List<uint>() { 798, 799, 800, 801 };

                // PvP
                case 67 or 54 or 52:
                    return new List<uint>();

            }

            if (id != 0)
            {
                PluginLog.Information($"[WondrousTails] Unrecognized ID: {id}");
            }

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

        public static void PrintTasks(List<WondrousTailsTask> tasks)
        {
            foreach (var task in tasks)
            {
                var message = $"TaskState: [{task.TaskState}], DutyList: [{string.Join(", ", task.DutyList)}]";
                Chat.Log("WondrousTailsTask", message);
            }
        }
    }
}
