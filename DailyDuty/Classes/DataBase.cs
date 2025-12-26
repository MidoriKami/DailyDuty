using System;
using System.Text.Json.Serialization;

namespace DailyDuty.Classes;

public class DataBase {
    [JsonIgnore] public string FileName = string.Empty;
    
    public DateTime NextReset;
    
    [JsonIgnore] public bool SavePending;

    public void Save() {
        SavePending = false;
        
        if (FileName == string.Empty) {
            Services.PluginLog.Error("Tried to save a config with no file name set");
            return;
        }

        Utilities.Config.SaveCharacterConfig(this, $"{FileName}.config.json");
    }
}
