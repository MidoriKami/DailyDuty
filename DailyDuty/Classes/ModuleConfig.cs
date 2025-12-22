using System;
using System.Text.Json.Serialization;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;

namespace DailyDuty.Classes;

public abstract class ModuleConfig<T> : ISavable where T : ModuleConfig<T>, new() {
	protected abstract string FileName { get; }

	public static T Load() {
		var configFileName = new T().FileName;
        
		Services.PluginLog.Debug($"Loading Config {configFileName}.config.json");
		return Config.LoadCharacterConfig<T>($"{configFileName}.config.json");
	} 

	public void Save() {
		Services.PluginLog.Debug($"Saving Config {FileName}.config.json");
		Config.SaveCharacterConfig(this, $"{FileName}.config.json");
		OnSave?.Invoke();
	}
    
    public string GetFileName() => FileName;

	[JsonIgnore] public Action? OnSave { get; set; }
}
