using DailyDuty.Models;
using Dalamud.Game.Text;
using KamiLib.AutomaticUserInterface;

namespace DailyDuty.Abstracts;

public class ModuleConfigBase
{
    [BoolConfigOption("Enable", "ModuleEnable", 0)]
    public bool ModuleEnabled = false;
    
    [BoolConfigOption("SendStatusOnLogin", "NotificationOptions", 9, "SendStatusOnLoginHelp")]
    public bool OnLoginMessage = true;
    
    [BoolConfigOption("SendStatusOnZoneChange", "NotificationOptions", 9, "SendStatusOnZoneChangeHelp")]
    public bool OnZoneChangeMessage = true;
    
    [BoolConfigOption("SendMessageOnReset", "NotificationOptions", 9, "SendMessageOnResetHelp")]
    public bool ResetMessage = false;
    
    [BoolConfigOption("EnableCustomChannel", "NotificationCustomization", 10)]
    public bool UseCustomChannel = false;
    
    [EnumConfigOption("NotificationCustomization", 10)]
    public XivChatType MessageChatChannel = Service.PluginInterface.GeneralChatType;
    
    [BoolConfigOption("EnableCustomStatusMessage", "NotificationCustomization", 10)]
    public bool UseCustomStatusMessage = false;

    [StringConfigOption("StatusMessage", "NotificationCustomization", 10)]
    public string CustomStatusMessage = string.Empty;
    
    [BoolConfigOption("EnableCustomResetMessage", "NotificationCustomization", 10)]
    public bool UseCustomResetMessage = false;

    [StringConfigOption("ResetMessage", "NotificationCustomization", 10)]
    public string CustomResetMessage = string.Empty;

    public ModuleTodoOptions TodoOptions = new();

    public bool Suppressed = false;
}