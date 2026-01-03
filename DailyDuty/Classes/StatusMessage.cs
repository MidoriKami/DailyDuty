using DailyDuty.Enums;
using Lumina.Text.ReadOnly;

namespace DailyDuty.Classes;

public class StatusMessage {
    public string Message { get; set; } = string.Empty;
    public PayloadId PayloadId { get; set; } = PayloadId.Unset;

    public static implicit operator StatusMessage(string message) => new() {
        Message = message,
        PayloadId = PayloadId.Unset,
    };
    
    public static implicit operator StatusMessage(ReadOnlySeString message) => new() {
        Message = message.ToString(),
        PayloadId = PayloadId.Unset,
    };
}
