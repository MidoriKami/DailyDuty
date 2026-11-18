namespace DailyDuty.Interfaces;

/// <summary>
/// Interface representing things with names.
/// </summary>
public interface INamed {

    /// <summary>
    /// Gets the name of this item.
    /// </summary>
    string DisplayName { get; }
}
