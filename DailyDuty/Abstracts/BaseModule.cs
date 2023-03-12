using System;
using System.IO;
using DailyDuty.Interfaces;
using DailyDuty.Models.Enums;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Newtonsoft.Json;

namespace DailyDuty.Abstracts;

public abstract unsafe class BaseModule
{
    public ModuleDataBase Data { get; protected set; }
    public ModuleConfigBase Config { get; protected set; }
    public abstract ModuleName ModuleName { get; }
    public abstract ModuleType ModuleType { get; }
    public abstract DateTime GetNextReset();
    public abstract ModuleStatus GetModuleStatus();
    public abstract IStatusMessage GetStatusMessage();
    
    public virtual void Update()
    {
        // PluginLog.Debug($"Updating module: {ModuleName}");
    }

    public virtual void Load()
    {
        PluginLog.Debug($"Loading module: {ModuleName}");
    }

    public virtual void Unload()
    {
        PluginLog.Debug($"Unloading module: {ModuleName}");
    }

    public virtual void Reset()
    {
        Data.NextReset = GetNextReset();
    }

    public virtual void ZoneChange(uint newZone)
    {
        
    }

    protected T? LoadData<T>() where T : ModuleDataBase
    {
        try
        {
            PluginLog.Debug($"Loading data for module: {ModuleName}");
            
            var contentId = PlayerState.Instance()->ContentId;
            var configDirectory = GetCharacterDirectory(contentId);
            var dataFile = new FileInfo(Path.Combine(configDirectory.FullName, GetDataFileName()));

            if (dataFile is { Exists: false })
            {
                T? newDataFile = default;
                SaveData(newDataFile);
                return newDataFile;
            }
            
            var jsonString = File.ReadAllText(dataFile.FullName);
            return JsonConvert.DeserializeObject<T>(jsonString)!;
        }
        catch (Exception exception)
        {
            PluginLog.Error(exception, $"Failed to load data for module: {ModuleName}");
            return default;
        }
    }
    
    protected T? LoadConfig<T>() where T : ModuleConfigBase
    {
        try
        {
            PluginLog.Debug($"Loading config for module: {ModuleName}");
            
            var contentId = PlayerState.Instance()->ContentId;
            var configDirectory = GetCharacterDirectory(contentId);
            var configFile = new FileInfo(Path.Combine(configDirectory.FullName, GetConfigFileName()));

            if (configFile is { Exists: false })
            {
                T? newConfigFile = default;
                SaveConfig(newConfigFile);
                return newConfigFile;
            }
            
            var jsonString = File.ReadAllText(configFile.FullName);
            return JsonConvert.DeserializeObject<T>(jsonString)!;
        }
        catch (Exception exception)
        {
            PluginLog.Error(exception, $"Failed to load data for module: {ModuleName}");
            return default;
        }
    }

    protected void SaveData<T>(T data) where T : ModuleDataBase?
    {
        try
        {
            PluginLog.Debug($"Saving data for module: {ModuleName}");
            
            var contentId = PlayerState.Instance()->ContentId;
            var configDirectory = GetCharacterDirectory(contentId);
            var dataFile = new FileInfo(Path.Combine(configDirectory.FullName, GetDataFileName()));

            var jsonString = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(dataFile.FullName, jsonString);
        }
        catch (Exception exception)
        {
            PluginLog.Error(exception, $"Failed to load data for module: {ModuleName}");
        }
    }

    protected void SaveConfig<T>(T config) where T : ModuleConfigBase?
    {
        try
        {
            PluginLog.Debug($"Saving config for module: {ModuleName}");

            var contentId = PlayerState.Instance()->ContentId;
            var configDirectory = GetCharacterDirectory(contentId);
            var configFile = new FileInfo(Path.Combine(configDirectory.FullName, GetConfigFileName()));

            var jsonString = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText(configFile.FullName, jsonString);
        }
        catch (Exception exception)
        {
            PluginLog.Error(exception, $"Failed to load data for module: {ModuleName}");
        }
    }
    
    public void Save()
    {
        PluginLog.Debug($"Saving module: {ModuleName}");
    }

    private string GetDataFileName() => $"{ModuleName.ToString()}.data.json";
    private string GetConfigFileName() => $"{ModuleName.ToString()}.config.json";
    private DirectoryInfo PluginConfigDirectory => Service.PluginInterface.ConfigDirectory;
    private DirectoryInfo GetCharacterDirectory(ulong contentId) => new(Path.Combine(PluginConfigDirectory.FullName, contentId.ToString()));
}