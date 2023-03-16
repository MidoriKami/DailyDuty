namespace DailyDuty.Models;

// ReSharper disable once UnusedTypeParameter
// Type is used in reflection to display the correct lumina info
public class LuminaTaskConfig<T>
{
    public required uint RowId { get; set; }
    public required bool Enabled { get; set; }
}