using System;
using DailyDuty.Enums;
using Dalamud.Game.Chat;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using Lumina.Text.ReadOnly;

namespace DailyDuty.Extensions;

public static class ChatGuiExtensions {
    extension(IChatGui chatGui) {
        public void PrintMessage(XivChatType channel, string moduleName, string message) {
            using var stringBuilder = new RentedSeStringBuilder();

            chatGui.Print(new PrintableChatMessage {
                LogKind = channel,
                Message = stringBuilder
                    .PushColorType(45)
                    .Append($"[{DailyDutyTag}] ")
                    .PopColorType()
                    .PushColorType(62)
                    .Append($"[{moduleName}] ")
                    .PopColorType()
                    .Append(message)
                    .ToReadOnlySeString(),
            });
        }

        /// <summary> Prints to Debug Channel </summary>
        public void PrintTaggedMessage(string message, string tag) {
            using var stringBuilder = new RentedSeStringBuilder();

            chatGui.Print(new PrintableChatMessage {
                LogKind = XivChatType.Debug,
                Message = stringBuilder
                    .PushColorType(45)
                    .Append($"[{DailyDutyTag}] ")
                    .PopColorType()
                    .PushColorType(62)
                    .Append($"[{tag}] ")
                    .PopColorType()
                    .Append(message)
                    .ToReadOnlySeString(),
            });
        }

        public void PrintPayloadMessage(XivChatType channel, PayloadId payload, string moduleName, string message) => chatGui.Print(new PrintableChatMessage {
            LogKind = channel,
            Message = BuildPayloadString(payload, moduleName, message),
        });
    }

    private static string DailyDutyTag => DateTime.Today is { Month: 4, Day: 1 } ? "DankDuty" : "DailyDuty";

    private static ReadOnlySeString BuildPayloadString(PayloadId payloadId, string moduleName, string message) {
        using var stringBuilder = new RentedSeStringBuilder();

        if (payloadId is not PayloadId.Unset) {
            return stringBuilder
                .PushColorType(45)
                .Append($"[{DailyDutyTag}] ")
                .PopColorType()
                .PushColorType(62)
                .Append($"[{moduleName}] ")
                .PopColorType()
                .PushDalamudLink(System.PayloadController.GetPayload(payloadId))
                .PushColorType(576)
                .Append(message)
                .PopColorType()
                .PopLink()
                .ToReadOnlySeString();
        }

        return stringBuilder
            .PushColorType(45)
            .Append($"[{DailyDutyTag}] ")
            .PopColorType()
            .PushColorType(62)
            .Append($"[{moduleName}] ")
            .PopColorType()
            .Append(message)
            .ToReadOnlySeString();
    }
}
