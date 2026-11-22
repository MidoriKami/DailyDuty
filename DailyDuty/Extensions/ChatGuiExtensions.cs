using DailyDuty.Classes;
using Dalamud.Game.Text;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using Lumina.Text.ReadOnly;

namespace DailyDuty.Extensions;

/// <summary>
/// Extension methods for IChatGui to print formatted module messages.
/// </summary>
public static class ChatGuiExtensions {
    extension(IChatGui chatGui) {
        /// <summary>
        /// Prints a formatted chat message with the DailyDuty and module name tags.
        /// </summary>
        public void PrintMessage(XivChatType channel, ReadOnlySeString moduleName, ReadOnlySeString message) {
            using var stringBuilder = new RentedSeStringBuilder();

            var builtString = stringBuilder.AppendDailyDutyPrefix(moduleName).Append(message).ToReadOnlySeString().ToDalamudString();
            var chatEntry = new XivChatEntry {
                Type = channel,
                Message = builtString,
            };

            // Chat functions are not threadsafe yet. Pending DalamudPR https://github.com/goatcorp/Dalamud/pull/3054
            IFramework.Get().Run(() => chatGui.Print(chatEntry));
        }

        /// <summary>
        /// Prints a formatted chat message with a clickable payload link.
        /// </summary>
        public void PrintMessage(XivChatType channel, ChatLinkPayload? payload, ReadOnlySeString moduleName, ReadOnlySeString message) {
            if (payload is null) {
                IPluginLog.Get().Error("Attempted to print a linked payload that was null.");
                return;
            }

            using var stringBuilder = new RentedSeStringBuilder();

            var builtString = stringBuilder.AppendDailyDutyPrefix(moduleName).AppendLink(message, payload.Payload).ToReadOnlySeString().ToDalamudString();
            var chatEntry = new XivChatEntry {
                Type = channel,
                Message = builtString,
            };

            // Chat functions are not threadsafe yet. Pending DalamudPR https://github.com/goatcorp/Dalamud/pull/3054
            IFramework.Get().Run(() => chatGui.Print(chatEntry));
        }
    }
}
