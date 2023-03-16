using System;
using System.IO;
using System.Linq;
using System.Reflection;
using DailyDuty.Models;
using DailyDuty.Models.Attributes;
using DailyDuty.Models.Enums;
using DailyDuty.System.Localization;
using DailyDuty.Views.Components;
using Dalamud.Game.Text;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Newtonsoft.Json;

namespace DailyDuty.Abstracts;

public abstract unsafe class BaseModule : IDisposable
{
    public abstract ModuleDataBase ModuleData { get; protected set; }
    public abstract ModuleConfigBase ModuleConfig { get; protected set; }
    public abstract ModuleName ModuleName { get; }
    public abstract ModuleType ModuleType { get; }
    public ModuleStatus ModuleStatus => ModuleConfig.Suppressed ? ModuleStatus.Suppressed : GetModuleStatus();
    protected abstract DateTime GetNextReset();
    public abstract TimeSpan GetResetPeriod();
    public virtual void Dispose() { }
    
    protected abstract ModuleStatus GetModuleStatus();
    protected abstract StatusMessage GetStatusMessage();
    private XivChatType GetChatChannel() => ModuleConfig.UseCustomChannel ? ModuleConfig.MessageChatChannel : Service.PluginInterface.GeneralChatType;
    
    public void DrawConfig()
    {
        var fields = ModuleConfig
            .GetType()
            .GetFields();
        
        var configAttributes = fields
            .Where(field => field.GetCustomAttribute(typeof(ConfigOption)) is not null)
            .Select(field => (field, (ConfigOption) field.GetCustomAttribute(typeof(ConfigOption))!))
            .ToList();

        var clickableLinks = fields
            .Where(field => field.GetCustomAttribute(typeof(ClickableLink)) is not null)
            .Select(field => (field, (ClickableLink) field.GetCustomAttribute(typeof(ClickableLink))!))
            .ToList();

        var taskSelection = fields
            .Where(field => field.GetCustomAttribute(typeof(SelectableTasks)) is not null)
            .FirstOrDefault();
        
        ModuleEnableView.Draw(ModuleConfig, SaveConfig);
        ModuleClickableLinkConfigView.Draw(clickableLinks, ModuleConfig, SaveConfig);
        ModuleConfigView.Draw(configAttributes, ModuleConfig, SaveConfig);
        ModuleSelectableTaskView.DrawConfig(taskSelection, ModuleConfig, SaveConfig);
        ModuleNotificationOptionsView.Draw(ModuleConfig, SaveConfig);
    }

    public void DrawData()
    {
        var fields = ModuleData
            .GetType()
            .GetFields();
        
        var dataDisplay = fields
            .Where(field => field.GetCustomAttribute(typeof(DataDisplay)) is not null)
            .Select(field => (field, (DataDisplay) field.GetCustomAttribute(typeof(DataDisplay))!))
            .ToList(); //.OrderBy(a => a.field.Name);
        
        var taskSelection = fields
            .Where(field => field.GetCustomAttribute(typeof(SelectableTasks)) is not null)
            .FirstOrDefault();
        
        ModuleStatusView.Draw(this);
        ModuleResetView.Draw(ModuleData);
        ModuleDataView.Draw(dataDisplay, ModuleData);
        ModuleSelectableTaskView.DrawData(taskSelection, ModuleData);
        ModuleSuppressionView.Draw(ModuleConfig, SaveConfig);
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

        if (ModuleConfig is not { OnZoneChangeMessage: true, ModuleEnabled: true, Suppressed: false }) return;
        if (GetModuleStatus() is not (ModuleStatus.Incomplete or ModuleStatus.Unknown)) return;
            
        SendStatusMessage();
    }

    public virtual void Unload()
    {
        PluginLog.Debug($"[{ModuleName}] Unloading Module");
    }

    public virtual void Reset()
    {
        PluginLog.Debug($"[{ModuleName}] Resetting Module, Next Reset: {GetNextReset().ToLocalTime()}");
        
        if (ModuleConfig is { ResetMessage: true, ModuleEnabled: true, Suppressed: false })
        {
            if (DateTime.UtcNow - ModuleData.NextReset < TimeSpan.FromMinutes(5))
            {
                SendResetMessage();
            }
        }
        
        ModuleData.NextReset = GetNextReset();
        SaveData();
        
        ModuleConfig.Suppressed = false;
        SaveConfig();
    }

    public virtual void ZoneChange(uint newZone)
    {
        if (ModuleConfig is not { OnZoneChangeMessage: true, ModuleEnabled: true, Suppressed: false }) return;
        if (GetModuleStatus() is not (ModuleStatus.Incomplete or ModuleStatus.Unknown)) return;
        
        SendStatusMessage();
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
    private void SendStatusMessage()
    {
        var statusMessage = GetStatusMessage();
        if (ModuleConfig.UseCustomStatusMessage && GetModuleStatus() != ModuleStatus.Unknown)
        {
            statusMessage.Message = ModuleConfig.CustomStatusMessage;
        }
        statusMessage.SourceModule = ModuleName;
        statusMessage.MessageChannel = GetChatChannel();
        statusMessage.PrintMessage();
    }
    private void SendResetMessage()
    {
        var statusMessage = GetStatusMessage();
        statusMessage.Message = ModuleConfig.UseCustomResetMessage ? ModuleConfig.CustomResetMessage : Strings.ModuleReset;
        statusMessage.SourceModule = ModuleName;
        statusMessage.MessageChannel = GetChatChannel();
        statusMessage.PrintMessage();
    }
}