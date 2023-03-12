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
    public abstract ModuleDataBase ModuleData { get; protected set; }
    public abstract ModuleConfigBase ModuleConfig { get; protected set; }
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
        ModuleData = LoadData();
        ModuleConfig = LoadConfig();

    }

    public virtual void Unload()
    {
        PluginLog.Debug($"Unloading module: {ModuleName}");
    }

    public virtual void Reset()
    {
        ModuleData.NextReset = GetNextReset();
    }

    public virtual void ZoneChange(uint newZone)
    {
        
    }

    protected ModuleDataBase LoadData()
    {
        try
        {
            PluginLog.Debug($"Loading data for module: {ModuleName}");
            
            var contentId = PlayerState.Instance()->ContentId;
            var configDirectory = GetCharacterDirectory(contentId);
            var dataFile = new FileInfo(Path.Combine(configDirectory.FullName, GetDataFileName()));

            if (dataFile is { Exists: false })
            {
                SaveData(ModuleData);
                return ModuleData;
            }
            
            var jsonString = File.ReadAllText(dataFile.FullName);
            return JsonConvert.DeserializeObject<ModuleDataBase>(jsonString)!;
        }
        catch (Exception exception)
        {
            PluginLog.Error(exception, $"Failed to load data for module: {ModuleName}");
            return new ModuleDataBase();
        }
    }
    
    protected ModuleConfigBase LoadConfig()
    {
        try
        {
            PluginLog.Debug($"Loading config for module: {ModuleName}");
            
            var contentId = PlayerState.Instance()->ContentId;
            var configDirectory = GetCharacterDirectory(contentId);
            var configFile = new FileInfo(Path.Combine(configDirectory.FullName, GetConfigFileName()));

            if (configFile is { Exists: false })
            {
                SaveConfig(ModuleConfig);
                return ModuleConfig;
            }
            
            var jsonString = File.ReadAllText(configFile.FullName);
            return JsonConvert.DeserializeObject<ModuleConfigBase>(jsonString)!;
        }
        catch (Exception exception)
        {
            PluginLog.Error(exception, $"Failed to load data for module: {ModuleName}");
            return new ModuleConfigBase();
        }
    }

    protected void SaveData(ModuleDataBase data)
    {
        try
        {
            PluginLog.Debug($"Saving data for module: {ModuleName}");
            
            var contentId = PlayerState.Instance()->ContentId;
            var configDirectory = GetCharacterDirectory(contentId);
            var dataFile = new FileInfo(Path.Combine(configDirectory.FullName, GetDataFileName()));

            var jsonString = JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
            File.WriteAllText(dataFile.FullName, jsonString);
        }
        catch (Exception exception)
        {
            PluginLog.Error(exception, $"Failed to load data for module: {ModuleName}");
        }
    }

    protected void SaveConfig(ModuleConfigBase config)
    {
        try
        {
            PluginLog.Debug($"Saving config for module: {ModuleName}");

            var contentId = PlayerState.Instance()->ContentId;
            var configDirectory = GetCharacterDirectory(contentId);
            var configFile = new FileInfo(Path.Combine(configDirectory.FullName, GetConfigFileName()));

            var jsonString = JsonConvert.SerializeObject(config, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
            File.WriteAllText(configFile.FullName, jsonString);
        }
        catch (Exception exception)
        {
            PluginLog.Error(exception, $"Failed to load data for module: {ModuleName}");
        }
    }

    public void SaveConfig()
    {
        PluginLog.Debug($"Saving config for module: {ModuleName}");
        SaveConfig(ModuleConfig);
    }
    
    public void SaveData()
    {
        PluginLog.Debug($"Saving data for module: {ModuleName}");
        SaveData(ModuleData);
    }

    private string GetDataFileName() => $"{ModuleName.ToString()}.data.json";
    private string GetConfigFileName() => $"{ModuleName.ToString()}.config.json";
    private DirectoryInfo PluginConfigDirectory => Service.PluginInterface.ConfigDirectory;
    private DirectoryInfo GetCharacterDirectory(ulong contentId) => new(Path.Combine(PluginConfigDirectory.FullName, contentId.ToString()));
}