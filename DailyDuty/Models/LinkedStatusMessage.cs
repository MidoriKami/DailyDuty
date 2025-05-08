using System;
using DailyDuty.Classes;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using KamiLib.Extensions;

namespace DailyDuty.Models;

public class LinkedStatusMessage : StatusMessage {
    public PayloadId Payload { get; init; }
    
    public required bool LinkEnabled { get; init; }
    
    public override void PrintMessage() {
        var messagePayload = System.PayloadController.GetPayload(Payload);
        
        var dailyDutyLabel = DateTime.Today is { Month: 4, Day: 1 } ? "DankDuty" : "DailyDuty";

        var messageBuilder = new SeStringBuilder()
            .AddUiForeground($"[{dailyDutyLabel}] ", 45)
            .AddUiForeground($"[{SourceModule.GetDescription()}] ", 62);
        
        var chatEntry = new XivChatEntry {
            Type = MessageChannel,
        };
        
        if (LinkEnabled) {
            chatEntry.Message = messageBuilder
                .Add(messagePayload)
                .AddUiForeground(Message, 576)
                .Add(RawPayload.LinkTerminator)
                .Build();
        }
        else {
            chatEntry.Message = messageBuilder
                .AddUiForeground(Message, 576)
                .Build();
        }
        
        Service.Chat.Print(chatEntry);
    }
}