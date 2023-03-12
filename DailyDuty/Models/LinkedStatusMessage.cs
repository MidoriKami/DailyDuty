using DailyDuty.Models.Attributes;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;

namespace DailyDuty.Models;

public class LinkedStatusMessage : StatusMessage
{
    public DalamudLinkPayload Payload { get; init; }
    
    public new void PrintMessage()
    {
        var message = new XivChatEntry
        {
            Type = XivChatType.Party,

            Message = new SeStringBuilder()
                .AddUiForeground($"[DailyDuty] ", 45)
                .AddUiForeground($"[{SourceModule.GetLabel()}] ", 62)
                .Add(Payload)
                .AddText(Message)
                .Add(RawPayload.LinkTerminator)
                .Build()
        };
        
        Service.Chat.PrintChat(message);
    }
}