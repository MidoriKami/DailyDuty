using Dalamud.Game.Text.SeStringHandling;

namespace DailyDuty
{
    internal static class Util
    {
        public static void PrintMessage(string message)
        {
            var stringBuilder = new SeStringBuilder();
            stringBuilder.AddUiForeground(45);
            stringBuilder.AddText("[DailyDuty] ");
            stringBuilder.AddUiForegroundOff();
            stringBuilder.AddText(message);

            Service.Chat.Print(stringBuilder.BuiltString);
        }
    }
}
