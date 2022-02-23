using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;

namespace DailyDuty.Utilities
{
    internal static class Chat
    {
        public static void Print(string tag, string message)
        {
            var debugEnabled = Service.Configuration.System.EnableDebugOutput;
            if (debugEnabled == false && tag.ToLower() == "debug") return;

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
            if (Service.Configuration.System.ClickableLinks == false)
            {
                Print(tag, message);
                return;
            }

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

        public static void Debug(string data)
        {
            Print("Debug", data);
        }
    }
}