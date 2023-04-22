using DailyDuty.Models;
using Dalamud.Game.Text;

namespace DailyDuty.Abstracts;

public class ModuleConfigBase
{
    public bool ModuleEnabled = false;

    public XivChatType MessageChatChannel = Service.PluginInterface.GeneralChatType;
    public bool UseCustomChannel = false;
    
    public bool OnLoginMessage = true;
    public bool OnZoneChangeMessage = true;
    public bool ResetMessage = false;

    public bool UseCustomStatusMessage = false;
    public string CustomStatusMessage = string.Empty;

    public bool UseCustomResetMessage = false;
    public string CustomResetMessage = string.Empty;

    public ModuleTodoOptions TodoOptions = new();

    public bool Suppressed = false;
}