namespace DailyDuty.Models;

// ReSharper disable once UnusedTypeParameter
// Type is used in reflection to display the correct lumina info
public class LuminaTaskData<T>
{
    public required uint RowId { get; init; }
    public required bool Complete { get; set; }
    public required int CurrentCount { get; set; }
}