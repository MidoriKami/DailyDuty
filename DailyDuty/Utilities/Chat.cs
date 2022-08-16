using Dalamud.Game.Text.SeStringHandling;

namespace DailyDuty.Utilities;

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

    public static void PrintError(string message)
    {
        var stringBuilder = new SeStringBuilder();
        stringBuilder.AddUiForeground(45);
        stringBuilder.AddText($"[DailyDuty] ");
        stringBuilder.AddUiForegroundOff();
        stringBuilder.AddText(message);

        Service.Chat.PrintError(stringBuilder.BuiltString);
    }
}