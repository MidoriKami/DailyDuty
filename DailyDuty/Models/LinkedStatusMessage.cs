using System;
using DailyDuty.Models.Enums;
using DailyDuty.System;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using KamiLib.Utilities;

namespace DailyDuty.Models;

public class LinkedStatusMessage : StatusMessage
{
    public PayloadId Payload { get; init; }
    
    public override void PrintMessage()
    {
        var messagePayload = PayloadController.Instance.GetPayload(Payload);
        
        var dailyDutyLabel = DateTime.Today is { Month: 4, Day: 1 } ? "DankDuty" : "DailyDuty"; 
        
        var message = new XivChatEntry
        {
            Type = MessageChannel,
            Message = new SeStringBuilder()
                .AddUiForeground($"[{dailyDutyLabel}] ", 45)
                .AddUiForeground($"[{SourceModule.GetLabel()}] ", 62)
                .Add(messagePayload)
                .AddUiForeground(Message, 576)
                .Add(RawPayload.LinkTerminator)
                .Build()
        };
        
        Service.Chat.Print(message);
    }
}