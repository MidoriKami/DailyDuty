using System;
using DailyDuty.UI;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Plugin.Services;
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
            // Chat functions are not threadsafe yet. Pending DalamudPR https://github.com/goatcorp/Dalamud/pull/3054
            IFramework.Get().Run(() => {
                var chatEntry = new XivChatEntry {
                    Type = channel,
                    Message = new SeStringBuilder()
                        .AddUiForeground($"[{DailyDutyTag}] ", 45)
                        .AddUiForeground($"[{moduleName.ExtractText()}] ", 62)
                        .AddText(message.ExtractText())
                        .Build(),
                };

                chatGui.Print(chatEntry);
            });
        }

        /// <summary>
        /// Prints a formatted chat message with a clickable payload link.
        /// </summary>
        public void PrintMessage(XivChatType channel, ChatLinkPayload? payload, ReadOnlySeString moduleName, ReadOnlySeString message) {
            if (payload is null) {
                IPluginLog.Get().Error("Attempted to print a linked payload that was null.");
                return;
            }

            // Chat functions are not threadsafe yet. Pending DalamudPR https://github.com/goatcorp/Dalamud/pull/3054
            IFramework.Get().Run(() => {
                var chatEntry = new XivChatEntry {
                    Type = channel,
                    Message = new SeStringBuilder()
                        .AddUiForeground($"[{DailyDutyTag}] ", 45)
                        .AddUiForeground($"[{moduleName.ExtractText()}] ", 62)
                        .Add(payload.Payload)
                        .AddUiForeground(message.ExtractText(), 576)
                        .Add(RawPayload.LinkTerminator)
                        .Build(),
                };

                chatGui.Print(chatEntry);
            });
        }
    }

    private static string DailyDutyTag => DateTime.Today is { Month: 4, Day: 1 } ? "DankDuty" : "DailyDuty";
}
