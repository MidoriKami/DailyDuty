using System;
using DailyDuty.Models.Enums;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using KamiLib.Utilities;

namespace DailyDuty.Models;

public class StatusMessage
{
    public ModuleName SourceModule { get; set; }
    public XivChatType MessageChannel { get; set; }
    public string Message { get; set; } = string.Empty;

    public virtual void PrintMessage()
    {
        var dailyDutyLabel = DateTime.Today is { Month: 4, Day: 1 } ? "DankDuty" : "DailyDuty"; 
        
        var message = new XivChatEntry
        {
            Type = MessageChannel,
            Message = new SeStringBuilder()
                .AddUiForeground($"[{dailyDutyLabel}] ", 45)
                .AddUiForeground($"[{SourceModule.GetLabel()}] ", 62)
                .AddText(Message)
                .Build()
        };
        
        Service.Chat.PrintChat(message);
    }
}