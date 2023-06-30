using Dalamud.Game.Text;
using KamiLib.AutomaticUserInterface;

namespace DailyDuty.Models;

[Category("NotificationCustomization", 10)]
public interface INotificationCustomization
{
    [BoolConfig("EnableCustomChannel")]
    public bool UseCustomChannel { get; set; }
    
    [EnumConfig]
    public XivChatType MessageChatChannel { get; set; }
    
    [BoolConfig("EnableCustomStatusMessage")]
    public bool UseCustomStatusMessage { get; set; }

    [StringConfig("StatusMessage")]
    public string CustomStatusMessage { get; set; }
    
    [BoolConfig("EnableCustomResetMessage")]
    public bool UseCustomResetMessage { get; set; }

    [StringConfig("ResetMessage")]
    public string CustomResetMessage { get; set; }
}