using DailyDuty.Models.Attributes;
using DailyDuty.Models.Enums;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;

namespace DailyDuty.Models;

public class StatusMessage
{
    public ModuleName SourceModule { get; set; }
    public XivChatType MessageChannel { get; set; }
    public string Message { get; set; } = string.Empty;

    public virtual void PrintMessage()
    {
        var message = new XivChatEntry
        {
            Type = MessageChannel,
            Message = new SeStringBuilder()
                .AddUiForeground($"[DailyDuty] ", 45)
                .AddUiForeground($"[{SourceModule.GetLabel()}] ", 62)
                .AddText(Message)
                .Build()
        };
        
        Service.Chat.PrintChat(message);
    }
}