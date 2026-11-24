namespace DailyDuty.Interfaces;

/// <summary>
/// Represents an element that can show a tooltip that when clicked performs an action.
/// </summary>
public interface IClickableTooltip : ITooltip {

    /// <summary>
    /// Function to invoke when this entry is clicked on.
    /// </summary>
    void OnTooltipClicked();
}
