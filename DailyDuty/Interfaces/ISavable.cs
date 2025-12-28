using System.Text.Json.Serialization;

namespace DailyDuty.Interfaces;

public abstract class Savable {
    [JsonIgnore] public string FileName { get; set; } = string.Empty;
    [JsonIgnore] public bool SavePending;

    protected abstract string FileExtension { get; }

    public void MarkDirty()
        => SavePending = true;

    public virtual void Save() {
        SavePending = false;
        
        if (FileName == string.Empty) {
            Services.PluginLog.Error("Tried to save a config with no file name set");
            return;
        }

        Utilities.Config.SaveCharacterConfig(this, $"{FileName}{FileExtension}");
    }
}
