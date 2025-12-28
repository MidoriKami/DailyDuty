using DailyDuty.Interfaces;
using Dalamud.Game.Text;

namespace DailyDuty.Classes;

public class ConfigBase : Savable {
    public bool OnLoginMessage = true;
    public bool OnZoneChangeMessage = true;
    public bool ResetMessage;

    public XivChatType MessageChatChannel = Services.PluginInterface.GeneralChatType;
    public string CustomStatusMessage = string.Empty;
    public string CustomResetMessage = string.Empty;

    public bool Suppressed;

    protected override string FileExtension => ".config.json";
}
