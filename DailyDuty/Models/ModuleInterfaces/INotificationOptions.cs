using KamiLib.AutomaticUserInterface;

namespace DailyDuty.Models;

[Category("NotificationOptions", 9)]
public interface INotificationOptions
{
    [BoolConfig("SendStatusOnLogin", "SendStatusOnLoginHelp")]
    public bool OnLoginMessage { get; set; }
    
    [BoolConfig("SendStatusOnZoneChange", "SendStatusOnZoneChangeHelp")]
    public bool OnZoneChangeMessage { get; set; }
    
    [BoolConfig("SendMessageOnReset", "SendMessageOnResetHelp")]
    public bool ResetMessage { get; set; }
}