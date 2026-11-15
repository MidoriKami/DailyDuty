namespace DailyDuty.Interfaces;

/// <summary>
/// Interface representing a configurable that might have its displayed information change.
/// </summary>
public interface IUpdatingConfigurable : IConfigurable {

    /// <summary>
    /// Updates the built config Ui.
    /// </summary>
    void UpdateConfigUi();
}
