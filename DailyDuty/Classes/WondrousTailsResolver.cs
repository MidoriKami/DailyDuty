using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Plugin.Services;
using Lumina.Excel.Sheets;

namespace DailyDuty.Classes;

public static class WondrousTailsResolver {
    extension(IDataManager dataManager) {
        private IEnumerable<InstanceContent> GetInstancesForOrderData(uint orderId) {
            var orderData = dataManager.GetExcelSheet<WeeklyBingoOrderData>().GetRow(orderId);

            switch (orderData.Type) {
                // Data is InstanceContent directly
                case 0:
                    return [orderData.Data.GetValueOrDefault<InstanceContent>() ?? throw new Exception("Tried to parse non-instance content data.")];

                // Data is specific duty level
                case 1:
                    return dataManager.GetExcelSheet<ContentFinderCondition>()
                        .Where(cfc => cfc.ContentType.RowId is 2)                        // Dungeons type
                        .Where(cfc => cfc.ClassJobLevelRequired == orderData.Data.RowId) // Matches duty level specifically
                        .AsInstanceContent();

                // Dungeons by Range
                case 2:
                    return dataManager.GetExcelSheet<ContentFinderCondition>()
                        .Where(cfc => cfc.ContentType.RowId is 2)                        // Dungeons type
                        .Where(cfc => cfc.ClassJobLevelRequired >= orderData.Unknown2)   // Above minimum level
                        .Where(cfc => cfc.ClassJobLevelRequired <= orderData.Data.RowId) // Below maximum level
                        .AsInstanceContent();

                // Data is InstanceContentType
                case 3:
                    return dataManager.GetExcelSheet<InstanceContent>()
                        .Where(instance => instance.InstanceContentType.RowId == orderData.Data.RowId);

                // Data is WeeklyBingoMultipleOrder
                case 4:
                    return dataManager.GetExcelSheet<WeeklyBingoMultipleOrder>().GetRow(orderData.Data.RowId)
                        .Content
                        .Where(content => content is { IsValid: true, RowId: not 0 })
                        .Select(content => content.Value);

                // Leveling, excludes levels divisible by 10
                case 5:
                    return dataManager.GetExcelSheet<ContentFinderCondition>()
                        .Where(cfc => cfc.ContentType.RowId is 2)                                                // Dungeons type
                        .Where(cfc => cfc.ClassJobLevelRequired >= orderData.Unknown2)                           // Above minimum level
                        .Where(cfc => cfc.ClassJobLevelRequired <= orderData.Data.RowId)                         // Below maximum level
                        .Where(cfc => !(cfc.ClassJobLevelRequired >= 50 && cfc.ClassJobLevelRequired % 10 is 0)) // Not an even multiple of 10, over 50
                        .AsInstanceContent();

                // High Level Dungeons, specifically divisible by 10
                case 6:
                    return dataManager.GetExcelSheet<ContentFinderCondition>()
                        .Where(cfc => cfc is { ContentType.RowId: 2, HighLevelRoulette: true }) // Dungeons type
                        .Where(cfc => cfc.ClassJobLevelRequired >= orderData.Unknown2)          // Above minimum level
                        .Where(cfc => cfc.ClassJobLevelRequired <= orderData.Data.RowId)        // Below maximum level
                        .Where(cfc => cfc.ClassJobLevelRequired % 10 is 0)                      // Is an even multiple of 10
                        .AsInstanceContent();

                // Trials in Range
                case 7:
                    return dataManager.GetExcelSheet<ContentFinderCondition>()
                        .Where(cfc => cfc.ContentType.RowId is 4)                        // Trials type
                        .Where(cfc => cfc.ClassJobLevelRequired >= orderData.Unknown2)   // Above minimum level
                        .Where(cfc => cfc.ClassJobLevelRequired <= orderData.Data.RowId) // Below maximum level
                        .AsInstanceContent();

                // Alliance Raids in Range
                case 8:
                    return dataManager.GetExcelSheet<ContentFinderCondition>()
                        .Where(cfc => cfc is { ContentType.RowId: 5, ContentMemberType.RowId: 4 }) // Alliance Raids type
                        .Where(cfc => cfc.ClassJobLevelRequired >= orderData.Unknown2)             // Above minimum level
                        .Where(cfc => cfc.ClassJobLevelRequired <= orderData.Data.RowId)           // Below maximum level
                        .AsInstanceContent();

                // Normal Raids in Range
                case 9:
                    return dataManager.GetExcelSheet<ContentFinderCondition>()
                        .Where(cfc => cfc is { ContentType.RowId: 5, ContentMemberType.RowId: 3, NormalRaidRoulette: true }) // Normal Raids type
                        .Where(cfc => cfc.ClassJobLevelRequired >= orderData.Unknown2)                                       // Above minimum level
                        .Where(cfc => cfc.ClassJobLevelRequired <= orderData.Data.RowId)                                     // Below maximum level
                        .AsInstanceContent();
            }

            return [];
        }

        public IEnumerable<ContentFinderCondition> GetDutiesForOrderData(uint orderId) {
            foreach (var instanceContent in dataManager.GetInstancesForOrderData(orderId)) {
                var duty = dataManager.GetExcelSheet<ContentFinderCondition>().Where(cfc => MatchInstanceContent(cfc, instanceContent));
                foreach (var dutyContent in duty) {
                    yield return dutyContent;
                }
            }
        }

        public IEnumerable<TerritoryType> GetTerritoriesForOrderData(uint orderId) {
            foreach (var duty in dataManager.GetDutiesForOrderData(orderId)) {
                if (duty.TerritoryType.IsValid) {
                    yield return duty.TerritoryType.Value;
                }
            }
        }
    }

    private static bool MatchInstanceContent(ContentFinderCondition cfc, InstanceContent instanceContent) {
        if (cfc.Content.GetValueOrDefault<InstanceContent>() is not { } cfcInstanceContent) return false;

        return cfcInstanceContent.RowId == instanceContent.RowId;
    }
    
    private static IEnumerable<InstanceContent> AsInstanceContent(this IEnumerable<ContentFinderCondition> enumerable)
        => enumerable.Select(cfc => cfc.Content.GetValueOrDefault<InstanceContent>())
            .Where(instanceContent => instanceContent.HasValue)
            .OfType<InstanceContent>();
}
