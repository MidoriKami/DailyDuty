using System;
using System.Globalization;
using Lumina.Excel.Sheets;

namespace DailyDuty.Models;

// ReSharper disable once UnusedTypeParameter
// Type is used in reflection to display the correct lumina info
public class LuminaTaskConfig<T> {
    public required uint RowId { get; init; }
    public required bool Enabled { get; set; }
    public required int TargetCount { get; set; }

    public string Label() => this switch {
        LuminaTaskConfig<ContentRoulette> =>  Service.DataManager.GetExcelSheet<ContentRoulette>().GetRow(RowId).Name.ToString(),
        LuminaTaskConfig<ClassJob> => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Service.DataManager.GetExcelSheet<ClassJob>().GetRow(RowId).Name.ToString()),
        LuminaTaskConfig<MobHuntOrderType> => GetMobHuntOrderTypeString(RowId),
        LuminaTaskConfig<Addon> => Service.DataManager.GetExcelSheet<Addon>().GetRow(RowId).Text.ToString(),
        LuminaTaskConfig<ContentFinderCondition> => Service.DataManager.GetExcelSheet<ContentFinderCondition>().GetRow(RowId).Name.ToString(),
        LuminaTaskConfig<ContentsNote> => Service.DataManager.GetExcelSheet<ContentsNote>().GetRow(RowId).Name.ToString(),
        _ => throw new Exception("Data Type Not Registered"),
    };

    private static string GetMobHuntOrderTypeString(uint row) {
        var itemInfo = Service.DataManager.GetExcelSheet<MobHuntOrderType>().GetRow(row);
        
        var eventItem = itemInfo.EventItem.Value.Name.ToString();
        if(eventItem == string.Empty) eventItem = itemInfo.EventItem.Value.Singular.ToString();

        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(eventItem);
    }
}