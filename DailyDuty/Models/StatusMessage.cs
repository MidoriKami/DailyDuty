using DailyDuty.Interfaces;
using DailyDuty.Models.Attributes;
using DailyDuty.Models.Enums;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;

namespace DailyDuty.Models;

public class StatusMessage : IStatusMessage
{
    public ModuleName SourceModule { get; init; } = ModuleName.Unknown;
    public string Message { get; set; } = string.Empty;

    public XivChatType MessageChannel { get; init; } = Service.PluginInterface.GeneralChatType;
    
    public void PrintMessage()
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