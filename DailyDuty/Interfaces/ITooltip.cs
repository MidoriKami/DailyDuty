using Lumina.Text.ReadOnly;

namespace DailyDuty.Interfaces;

/// <summary>
/// Interface representing elements that can show a mouse over tooltip.
/// </summary>
public interface ITooltip {
    ReadOnlySeString GetTooltip();
}
