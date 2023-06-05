using DailyDuty.Models;
using Dalamud.Game.Text;
using KamiLib.AutomaticUserInterface;

namespace DailyDuty.Abstracts;

public class ModuleConfigBase
{
    [DrawCategory("ModuleEnable", 0)]
    [BoolConfigOption("Enable")]
    public bool ModuleEnabled = false;
    
    [DrawCategory("NotificationOptions", 9)]
    [BoolConfigOption("SendStatusOnLogin", "SendStatusOnLoginHelp")]
    public bool OnLoginMessage = true;
    
    [DrawCategory("NotificationOptions", 9)]
    [BoolConfigOption("SendStatusOnZoneChange", "SendStatusOnZoneChangeHelp")]
    public bool OnZoneChangeMessage = true;
    
    [DrawCategory("NotificationOptions", 9)]
    [BoolConfigOption("SendMessageOnReset", "SendMessageOnResetHelp")]
    public bool ResetMessage = false;
    
    [DrawCategory("NotificationCustomization", 10)]
    [BoolConfigOption("EnableCustomChannel")]
    public bool UseCustomChannel = false;
    
    [DrawCategory("NotificationCustomization", 10)]
    [EnumConfigOption]
    public XivChatType MessageChatChannel = Service.PluginInterface.GeneralChatType;
    
    [DrawCategory("NotificationCustomization", 10)]
    [BoolConfigOption("EnableCustomStatusMessage")]
    public bool UseCustomStatusMessage = false;

    [DrawCategory("NotificationCustomization", 10)]
    [StringConfigOption("StatusMessage")]
    public string CustomStatusMessage = string.Empty;
    
    [DrawCategory("NotificationCustomization", 10)]
    [BoolConfigOption("EnableCustomResetMessage")]
    public bool UseCustomResetMessage = false;

    [DrawCategory("NotificationCustomization", 10)]
    [StringConfigOption("ResetMessage")]
    public string CustomResetMessage = string.Empty;

    public ModuleTodoOptions TodoOptions = new();

    public bool Suppressed = false;
}