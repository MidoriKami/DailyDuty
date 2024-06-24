using DailyDuty.Classes;

namespace DailyDuty.Models;

public static class ConditionalStatusMessage {
    public static StatusMessage GetMessage(bool conditional, string message, PayloadId payloadId) {
        if (conditional) {
            return new LinkedStatusMessage {
                Message = message,
                Payload = payloadId,
            };
        }
        else {
            return new StatusMessage {
                Message = message,
            };
        }
    }
}