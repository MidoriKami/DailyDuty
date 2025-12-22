using System;
using DailyDuty.Utilities;

namespace DailyDuty.Classes;

public abstract class ModuleData<T> where T : ModuleData<T>, new() {
    protected abstract string FileName { get; }
    
    public DateTime NextReset;

    public static T Load() {
        var configFileName = new T().FileName;

        Services.PluginLog.Debug($"Loading Data {configFileName}.data.json");
        return Data.LoadCharacterData<T>($"{configFileName}.data.json");
    } 

    public void Save() {
        Services.PluginLog.Debug($"Saving Data {FileName}.data.json");
        Data.SaveCharacterData(this, $"{FileName}.data.json");
    }
    
    public string GetFileName() => FileName;
}
