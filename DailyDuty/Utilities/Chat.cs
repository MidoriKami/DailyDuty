using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Logging;

namespace DailyDuty.Utilities
{
    internal static class Chat
    {
        public static void Print(string tag, string message)
        {
            var stringBuilder = new SeStringBuilder();
            stringBuilder.AddUiForeground(45);
            stringBuilder.AddText($"[DailyDuty] ");
            stringBuilder.AddUiForegroundOff();
            stringBuilder.AddUiForeground(62);
            stringBuilder.AddText($"[{tag}] ");
            stringBuilder.AddUiForegroundOff();
            stringBuilder.AddText(message);

            Service.Chat.Print(stringBuilder.BuiltString);
        }

        public static void Print(string tag, string message, DalamudLinkPayload payload)
        {
            var stringBuilder = new SeStringBuilder();
            stringBuilder.AddUiForeground(45);
            stringBuilder.AddText($"[DailyDuty] ");
            stringBuilder.AddUiForegroundOff();
            stringBuilder.AddUiForeground(62);
            stringBuilder.AddText($"[{tag}] ");
            stringBuilder.AddUiForegroundOff();
            stringBuilder.Add(payload);
            stringBuilder.AddUiForeground(35);
            stringBuilder.AddText(message);
            stringBuilder.AddUiForegroundOff();
            stringBuilder.Add(RawPayload.LinkTerminator);

            Service.Chat.Print(stringBuilder.BuiltString);
        }

        public static void Debug(string message)
        {
            Print("Debug", message);
        }

        public static void Log(string tag, string message)
        {
            if (Service.SystemConfiguration.DeveloperMode)
            {
                PluginLog.Information(message);

                if (Service.SystemConfiguration.EchoLogToChat)
                {
                    Print(tag, message);
                }
            }
        }

        public static void Warning(string tag, string message)
        {
            if (Service.SystemConfiguration.DeveloperMode)
            {
                PluginLog.Warning(message);

                if (Service.SystemConfiguration.EchoLogToChat)
                {
                    Print(tag, message);
                }
            }
        }
    }
}