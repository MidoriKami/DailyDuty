using Dalamud.Game.Text;

namespace DailyDuty.Abstracts;

public class ModuleConfigBase
{
    public bool ModuleEnabled = false;

    public XivChatType MessageChatChannel = Service.PluginInterface.GeneralChatType;
    public bool UseCustomChannel = false;
    
    public bool OnLoginMessage = false;
    public bool OnZoneChangeMessage = false;
    public bool ResetMessage = false;

    public bool UseCustomStatusMessage = false;
    public string CustomStatusMessage = string.Empty;

    public bool UseCustomResetMessage = false;
    public string CustomResetMessage = string.Empty;

    public bool Suppressed = false;
}