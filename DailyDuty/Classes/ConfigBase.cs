using System.Text.Json.Serialization;
using DailyDuty.Interfaces;
using Dalamud.Game.Text;

namespace DailyDuty.Classes;

public class ConfigBase : ISavable {
    [JsonIgnore] public string FileName = string.Empty;

    public bool OnLoginMessage = true;
    public bool OnZoneChangeMessage = true;
    public bool ResetMessage;

    public XivChatType MessageChatChannel = Services.PluginInterface.GeneralChatType;
    public string CustomStatusMessage = string.Empty;
    public string CustomResetMessage = string.Empty;

    public bool Suppressed;
    
    [JsonIgnore] public bool ConfigChanged;
    
    public void Save() {
        if (FileName == string.Empty) {
            Services.PluginLog.Error("Tried to save a config with no file name set");
            return;
        }

        Utilities.Config.SaveCharacterConfig(this, $"{FileName}.config.json");
    }
}
