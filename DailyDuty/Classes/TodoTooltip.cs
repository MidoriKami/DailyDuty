using DailyDuty.Enums;
using Lumina.Text.ReadOnly;

namespace DailyDuty.Classes;

public class TodoTooltip {
    public PayloadId ClickAction = PayloadId.Unset;
    public ReadOnlySeString TooltipText = string.Empty;
    
    public static implicit operator TodoTooltip(string text) => new() {
        TooltipText = text,
    };

    public static implicit operator TodoTooltip(StatusMessage message) => new() {
        TooltipText = message.Message,
    };
}
