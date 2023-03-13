using DailyDuty.Models.Attributes;
using Dalamud.Game.Text;

namespace DailyDuty.Abstracts;

public class ModuleConfigBase
{
    [ConfigOption("Enable")]
    public bool ModuleEnabled = false;

    public XivChatType MessageChatChannel = Service.PluginInterface.GeneralChatType;
    
    [ConfigOption("Use Custom Chat Channel", "If enabled, sends messages to the chat channel set for this module")]
    public bool UseCustomChannel = false;
    
    [ConfigOption("Enable Chat Link")]
    public bool LinkEnabled = false;
    
    [ConfigOption("LoginMessage")]
    public bool OnLoginMessage = false;
    
    [ConfigOption("ZoneChangeMessage")]
    public bool OnZoneChangeMessage = false;
}