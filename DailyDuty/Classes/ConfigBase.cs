using DailyDuty.Interfaces;
using Dalamud.Game.Chat;

namespace DailyDuty.Classes;

public class ConfigBase : Savable {
    public bool OnLoginMessage = true;
    public bool OnZoneChangeMessage = true;
    public bool ResetMessage;

    public XivChatType MessageChatChannel = DailyDutyPlugin.PluginInterface.GeneralChatType;
    public string CustomStatusMessage = string.Empty;
    public string CustomResetMessage = string.Empty;

    public bool Suppressed;

    protected override string FileExtension => ".config.json";
}
