using DailyDuty.Models.Attributes;
using DailyDuty.Models.Enums;
using DailyDuty.System;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;

namespace DailyDuty.Models;

public class LinkedStatusMessage : StatusMessage
{
    public PayloadId Payload { get; init; }
    
    public override void PrintMessage()
    {
        var messagePayload = PayloadController.Instance.GetPayload(Payload);
        
        var message = new XivChatEntry
        {
            Type = MessageChannel,
            Message = new SeStringBuilder()
                .AddUiForeground($"[DailyDuty] ", 45)
                .AddUiForeground($"[{SourceModule.GetLabel()}] ", 62)
                .Add(messagePayload)
                .AddUiForeground(Message, 576)
                .Add(RawPayload.LinkTerminator)
                .Build()
        };
        
        Service.Chat.PrintChat(message);
    }
}