namespace DailyDuty.Classes;

/// <summary>
/// Represents a tracked raid with its completion status.
/// </summary>
/// <param name="Id">The ContentFinderCondition row ID for the raid.</param>
/// <param name="Complete">Whether the raid has been completed.</param>
public record RaidEntry(uint Id, bool Complete);
