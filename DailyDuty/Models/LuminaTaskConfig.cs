namespace DailyDuty.Models;

// ReSharper disable once UnusedTypeParameter
// Type is used in reflection to display the correct lumina info
public class LuminaTaskConfig<T>
{
    public required uint RowId { get; init; }
    public required bool Enabled { get; set; }
    public required int TargetCount { get; set; } = -1;
}