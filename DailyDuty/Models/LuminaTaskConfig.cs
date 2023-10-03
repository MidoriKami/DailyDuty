using System;
using System.Globalization;
using KamiLib.Game;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.Models;

// ReSharper disable once UnusedTypeParameter
// Type is used in reflection to display the correct lumina info
public class LuminaTaskConfig<T>
{
    public required uint RowId { get; init; }
    public required bool Enabled { get; set; }
    public required int TargetCount { get; set; }

    public string Label() => this switch
    {
        LuminaTaskConfig<ContentRoulette> => LuminaCache<ContentRoulette>.Instance.GetRow(RowId)!.Name.ToString(),
        LuminaTaskConfig<ClassJob> => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(LuminaCache<ClassJob>.Instance.GetRow(RowId)!.Name.ToString()),
        LuminaTaskConfig<MobHuntOrderType> => GetMobHuntOrderTypeString(RowId),
        LuminaTaskConfig<Addon> => LuminaCache<Addon>.Instance.GetRow(RowId)!.Text.ToString(),
        LuminaTaskConfig<ContentFinderCondition> => LuminaCache<ContentFinderCondition>.Instance.GetRow(RowId)!.Name.ToString(),
        LuminaTaskConfig<ContentsNote> => LuminaCache<ContentsNote>.Instance.GetRow(RowId)!.Name.ToString(),
        _ => throw new Exception("Data Type Not Registered")
    };
    
    private static string GetMobHuntOrderTypeString(uint row)
    {
        var itemInfo = LuminaCache<MobHuntOrderType>.Instance.GetRow(row)!;
        
        var eventItem = itemInfo.EventItem.Value?.Name.ToString();
        if(eventItem == string.Empty) eventItem = itemInfo.EventItem.Value?.Singular.ToString();

        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(eventItem ?? "Unable to Read Event Item Name");
    }
}