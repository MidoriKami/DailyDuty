using System.Text.Json.Serialization;
using Lumina.Excel;

namespace DailyDuty.Classes;

public class LuminaDataEntry<T> where T : struct, IExcelRow<T> {
    public required uint RowId { get; init; }
    
    public required bool Enabled { get; set; }
    
    public int TargetCount { get; set; }

    public int CurrentCount { get; set; }
    
    public bool IsCompleted { get; set; }
    
    [JsonIgnore] public T Entry => Services.DataManager.GetExcelSheet<T>().GetRow(RowId);
}
