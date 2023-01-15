using System.Collections.Generic;
using System.Linq;
using Dalamud;
using Dalamud.Logging;
using Dalamud.Utility;
using KamiLib.Caching;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.Utilities;

internal static class TaskLookup
{
    public static List<uint> GetInstanceListFromID(uint id)
    {
        var bingoOrderData = LuminaCache<WeeklyBingoOrderData>.Instance.GetRow(id);
        if (bingoOrderData is null) return new List<uint>();
        
        switch (bingoOrderData.Type)
        {
            // Specific Duty
            case 0:
                return LuminaCache<ContentFinderCondition>.Instance
                    .Where(c => c.Content == bingoOrderData.Data)
                    .OrderBy(row => row.SortKey)
                    .Select(c => c.TerritoryType.Row)
                    .ToList();
            
            // Specific Level Dungeon
            case 1:
                return LuminaCache<ContentFinderCondition>.Instance
                    .Where(m => m.ContentType.Row is 2)
                    .Where(m => m.ClassJobLevelRequired == bingoOrderData.Data)
                    .OrderBy(row => row.SortKey)
                    .Select(m => m.TerritoryType.Row)
                    .ToList();
            
            // Level Range Dungeon
            case 2:
                return LuminaCache<ContentFinderCondition>.Instance
                    .Where(m => m.ContentType.Row is 2)
                    .Where(m => m.ClassJobLevelRequired >= bingoOrderData.Data - (bingoOrderData.Data > 50 ? 9 : 49) && m.ClassJobLevelRequired <= bingoOrderData.Data - 1)
                    .OrderBy(row => row.SortKey)
                    .Select(m => m.TerritoryType.Row)
                    .ToList();
            
            // Special categories
            case 3:
                return bingoOrderData.Unknown5 switch
                {
                    // Treasure Map Instances are Not Supported
                    1 => new List<uint>(),
                    
                    // PvP Categories are Not Supported
                    2 => new List<uint>(),
                    
                    // Deep Dungeons
                    3 => LuminaCache<ContentFinderCondition>.Instance
                        .Where(m => m.ContentType.Row is 21)
                        .OrderBy(row => row.SortKey)
                        .Select(m => m.TerritoryType.Row)
                        .ToList(),
                    
                    _ => new List<uint>()
                };
            
            // Multi-instance raids
            case 4:
                var raidIndex = (int)(bingoOrderData.Data - 11) * 2;
                
                return bingoOrderData.Data switch
                {
                    // Binding Coil, Second Coil, Final Coil
                    2 => new List<uint> { 241, 242, 243, 244, 245 },
                    3 => new List<uint> { 355, 356, 357, 358 },
                    4 => new List<uint> { 193, 194, 195, 196 },
                    
                    // Gordias, Midas, The Creator
                    5 => new List<uint> { 442, 443, 444, 445 },
                    6 => new List<uint> {520, 521, 522, 523},
                    7 => new List<uint> { 580, 581, 582, 583 },
                    
                    // Deltascape, Sigmascape, Alphascape
                    8 => new List<uint> { 691, 692, 693, 694 },
                    9 => new List<uint> { 748, 749, 750, 751 },
                    10 => new List<uint> { 798, 799, 800, 801 },
                    
                    > 10 => LuminaCache<ContentFinderCondition>.Instance
                        .OfLanguage(ClientLanguage.English)
                        .Where(row => row.ContentType.Row is 5)
                        .Where(row => row.ContentMemberType.Row is 3)
                        .Where(row => !row.Name.ToDalamudString().TextValue.Contains("Savage"))
                        .Where(row => row.ItemLevelRequired >= 425)
                        .OrderBy(row => row.SortKey)
                        .Select(row => row.TerritoryType.Row)
                        .ToArray()[raidIndex..(raidIndex + 2)]
                        .ToList(),
                    
                    _ => new List<uint>()
                };
        }
        
        PluginLog.Information($"[WondrousTails] Unrecognized ID: {id}");
        return new List<uint>();
    }
}