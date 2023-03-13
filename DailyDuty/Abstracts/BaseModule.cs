using System;
using System.IO;
using System.Linq;
using System.Reflection;
using DailyDuty.Interfaces;
using DailyDuty.Models.Attributes;
using DailyDuty.Models.Enums;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using ImGuiNET;
using Newtonsoft.Json;

namespace DailyDuty.Abstracts;

public abstract unsafe class BaseModule : IDisposable
{
    public abstract ModuleDataBase ModuleData { get; protected set; }
    public abstract ModuleConfigBase ModuleConfig { get; protected set; }
    public abstract ModuleName ModuleName { get; }
    public abstract ModuleType ModuleType { get; }
    public abstract DateTime GetNextReset();
    public abstract ModuleStatus GetModuleStatus();
    public abstract IStatusMessage GetStatusMessage();
    
    public virtual void Dispose() { }

    public void DrawConfig()
    {
        var fields = ModuleConfig
            .GetType()
            .GetFields()
            .Where(field => field.GetCustomAttribute(typeof(ConfigOption)) is not null)
            .Select(field => (field,  (ConfigOption) field.GetCustomAttribute(typeof(ConfigOption))!))
            ;//.OrderBy(a => a.field.Name);
        
        ImGuiHelpers.ScaledIndent(15.0f);
        
        foreach (var (field, attribute) in fields)
        {
            switch (Type.GetTypeCode(field.FieldType))
            {
                case TypeCode.Boolean:
                    var boolValue = (bool) field.GetValue(ModuleConfig)!;
                    if (ImGui.Checkbox(attribute.Name, ref boolValue))
                    {
                        field.SetValue(ModuleConfig, boolValue);
                        SaveConfig(ModuleConfig);
                    }
                    break;
                
                case TypeCode.String:
                    var stringValue = (string) field.GetValue(ModuleConfig)!;
                    ImGui.InputText(attribute.Name + $"##{field.Name}", ref stringValue, 2048);
                    if (ImGui.IsItemDeactivatedAfterEdit())
                    {
                        field.SetValue(ModuleConfig, stringValue);
                        SaveConfig(ModuleConfig);
                    }
                    break;
            }

            if (attribute.HelpText is not null)
            {
                ImGuiComponents.HelpMarker(attribute.HelpText);
            }
        }
        
        ImGuiHelpers.ScaledIndent(-15.0f);
    }
    
    public virtual void Update()
    {
        // PluginLog.Debug($"Updating module: {ModuleName}");
    }

    public virtual void Load()
    {
        PluginLog.Debug($"[{ModuleName}] Loading Module");
        ModuleData = LoadData();
        ModuleConfig = LoadConfig();

        if (!ModuleConfig.OnLoginMessage) return;
        
        GetStatusMessage().PrintMessage();
    }

    public virtual void Unload()
    {
        PluginLog.Debug($"[{ModuleName}] Unloading Module");
    }

    public virtual void Reset()
    {
        ModuleData.NextReset = GetNextReset();
    }

    public virtual void ZoneChange(uint newZone)
    {
        if (!ModuleConfig.OnZoneChangeMessage) return;

        GetStatusMessage().PrintMessage();
    }

    protected ModuleDataBase LoadData()
    {
        try
        {
            PluginLog.Debug($"[{ModuleName}] Loading {ModuleName}.data.json");
            
            var contentId = PlayerState.Instance()->ContentId;
            var configDirectory = GetCharacterDirectory(contentId);
            var dataFile = new FileInfo(Path.Combine(configDirectory.FullName, GetDataFileName()));

            if (dataFile is { Exists: false })
            {
                SaveData(ModuleData);
                return ModuleData;
            }
            
            var jsonString = File.ReadAllText(dataFile.FullName);
            return (ModuleDataBase) JsonConvert.DeserializeObject(jsonString, ModuleData.GetType())!;
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
            PluginLog.Debug($"[{ModuleName}] Loading {ModuleName}.config.json");
            
            var contentId = PlayerState.Instance()->ContentId;
            var configDirectory = GetCharacterDirectory(contentId);
            var configFile = new FileInfo(Path.Combine(configDirectory.FullName, GetConfigFileName()));

            if (configFile is { Exists: false })
            {
                SaveConfig(ModuleConfig);
                return ModuleConfig;
            }
            
            var jsonString = File.ReadAllText(configFile.FullName);
            return (ModuleConfigBase) JsonConvert.DeserializeObject(jsonString, ModuleConfig.GetType())!;
        }
        catch (Exception exception)
        {
            PluginLog.Error(exception, $"Failed to load config for module: {ModuleName}");
            return new ModuleConfigBase();
        }
    }

    protected void SaveData(ModuleDataBase data)
    {
        try
        {
            PluginLog.Debug($"[{ModuleName}] Saving {ModuleName}.data.json");
            
            var contentId = PlayerState.Instance()->ContentId;
            var configDirectory = GetCharacterDirectory(contentId);
            var dataFile = new FileInfo(Path.Combine(configDirectory.FullName, GetDataFileName()));

            var jsonString = JsonConvert.SerializeObject(data, data.GetType(), new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            });
            File.WriteAllText(dataFile.FullName, jsonString);
        }
        catch (Exception exception)
        {
            PluginLog.Error(exception, $"Failed to save data for module: {ModuleName}");
        }
    }

    protected void SaveConfig(ModuleConfigBase config)
    {
        try
        {
            PluginLog.Debug($"[{ModuleName}] Saving {ModuleName}.config.json");

            var contentId = PlayerState.Instance()->ContentId;
            var configDirectory = GetCharacterDirectory(contentId);
            var configFile = new FileInfo(Path.Combine(configDirectory.FullName, GetConfigFileName()));

            var jsonString = JsonConvert.SerializeObject(config, config.GetType(), new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            });
            File.WriteAllText(configFile.FullName, jsonString);
        }
        catch (Exception exception)
        {
            PluginLog.Error(exception, $"Failed to save config for module: {ModuleName}");
        }
    }

    public void SaveConfig() => SaveConfig(ModuleConfig);
    public void SaveData() => SaveData(ModuleData);
    private string GetDataFileName() => $"{ModuleName.ToString()}.data.json";
    private string GetConfigFileName() => $"{ModuleName.ToString()}.config.json";
    private DirectoryInfo PluginConfigDirectory => Service.PluginInterface.ConfigDirectory;
    private DirectoryInfo GetCharacterDirectory(ulong contentId)
    {
        var directoryInfo = new DirectoryInfo(Path.Combine(PluginConfigDirectory.FullName, contentId.ToString()));

        if (directoryInfo is { Exists: false })
        {
            directoryInfo.Create();
        }

        return directoryInfo;
    }

}