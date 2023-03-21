using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using DailyDuty.Models;
using DailyDuty.Models.Attributes;
using DailyDuty.Models.Enums;
using DailyDuty.System;
using DailyDuty.System.Localization;
using DailyDuty.Views.Components;
using Dalamud.Game.Text;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using KamiLib.GameState;
using Newtonsoft.Json;

namespace DailyDuty.Abstracts;

public abstract unsafe class BaseModule : IDisposable
{
    public abstract ModuleDataBase ModuleData { get; protected set; }
    public abstract ModuleConfigBase ModuleConfig { get; protected set; }
    public abstract ModuleName ModuleName { get; }
    public abstract ModuleType ModuleType { get; }
    public ModuleStatus ModuleStatus => ModuleConfig.Suppressed ? ModuleStatus.Suppressed : GetModuleStatus();
    public abstract TimeSpan GetResetPeriod();
    public virtual void Dispose() { }
    
    protected abstract DateTime GetNextReset();
    protected abstract ModuleStatus GetModuleStatus();
    protected abstract StatusMessage GetStatusMessage();
    protected bool DataChanged;
    protected bool ConfigChanged;
    
    protected XivChatType GetChatChannel() => ModuleConfig.UseCustomChannel ? ModuleConfig.MessageChatChannel : Service.PluginInterface.GeneralChatType;
    private readonly Stopwatch statusMessageLockout = new();
    
    public virtual void AddonPreSetup(AddonArgs addonInfo) { }
    public virtual void AddonPostSetup(AddonArgs addonInfo) { }
    public virtual void AddonFinalize(AddonArgs addonInfo) { }
    
    
    public void DrawConfig()
    {
        var fields = ModuleConfig
            .GetType()
            .GetFields();
        
        var configOptions = fields
            .Where(field => field.GetCustomAttribute(typeof(ConfigOption)) is not null)
            .Select(field => (field, (ConfigOption) field.GetCustomAttribute(typeof(ConfigOption))!))
            .ToList();

        var clickableLinks = fields
            .Where(field => field.GetCustomAttribute(typeof(ClickableLink)) is not null)
            .Select(field => (field, (ClickableLink) field.GetCustomAttribute(typeof(ClickableLink))!))
            .ToList();

        var selectableTasks = fields
            .Where(field => field.GetCustomAttribute(typeof(SelectableTasks)) is not null)
            .FirstOrDefault();
        
        ModuleEnableView.Draw(ModuleConfig, SaveConfig);
        ModuleClickableLinkConfigView.Draw(clickableLinks, ModuleConfig, SaveConfig);
        ModuleConfigView.Draw(configOptions, ModuleConfig, SaveConfig);
        ModuleSelectableTaskView.DrawConfig(selectableTasks, ModuleConfig, SaveConfig);
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
        if(DataChanged) SaveData();
        if(ConfigChanged) SaveConfig();

        DataChanged = false;
        ConfigChanged = false;
    }

    public virtual void Load()
    {
        PluginLog.Debug($"[{ModuleName}] Loading Module");
        ModuleData = LoadData();
        ModuleConfig = LoadConfig();

        if (DateTime.UtcNow > ModuleData.NextReset)
        {
            Reset();
        }
        
        SendStatusMessage();
    }

    public virtual void Unload()
    {
        PluginLog.Debug($"[{ModuleName}] Unloading Module");
        
        statusMessageLockout.Stop();
        statusMessageLockout.Reset();
    }

    public virtual void Reset()
    {
        PluginLog.Debug($"[{ModuleName}] Resetting Module, Next Reset: {GetNextReset().ToLocalTime()}");

        SendResetMessage();
        
        ModuleData.NextReset = GetNextReset();
        SaveData();
        
        ModuleConfig.Suppressed = false;
        SaveConfig();
    }

    public virtual void ZoneChange(uint newZone)
    {
        SendStatusMessage();
    }

    private ModuleDataBase LoadData()
    {
        try
        {
            PluginLog.Debug($"[{ModuleName}] Loading {ModuleName}.data.json");
            var dataFile = GetDataFileInfo();
            
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
    
    private ModuleConfigBase LoadConfig()
    {
        try
        {
            PluginLog.Debug($"[{ModuleName}] Loading {ModuleName}.config.json");
            var configFile = GetConfigFileInfo();

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

    private void SaveData(ModuleDataBase data)
    {
        try
        {
            PluginLog.Debug($"[{ModuleName}] Saving {ModuleName}.data.json");
            var dataFile = GetDataFileInfo();

            var jsonString = JsonConvert.SerializeObject(data, data.GetType(), new JsonSerializerSettings { Formatting = Formatting.Indented });
            File.WriteAllText(dataFile.FullName, jsonString);
        }
        catch (Exception exception)
        {
            PluginLog.Error(exception, $"Failed to save data for module: {ModuleName}");
        }
    }

    private void SaveConfig(ModuleConfigBase config)
    {
        try
        {
            PluginLog.Debug($"[{ModuleName}] Saving {ModuleName}.config.json");
            var configFile = GetConfigFileInfo();

            var jsonString = JsonConvert.SerializeObject(config, config.GetType(), new JsonSerializerSettings { Formatting = Formatting.Indented });
            File.WriteAllText(configFile.FullName, jsonString);
        }
        catch (Exception exception)
        {
            PluginLog.Error(exception, $"Failed to save config for module: {ModuleName}");
        }
    }

    private FileInfo GetConfigFileInfo()
    {
        var contentId = PlayerState.Instance()->ContentId;
        var configDirectory = GetCharacterDirectory(contentId);
        return new FileInfo(Path.Combine(configDirectory.FullName, GetConfigFileName()));
    }

    private FileInfo GetDataFileInfo()
    {
        var contentId = PlayerState.Instance()->ContentId;
        var configDirectory = GetCharacterDirectory(contentId);
        return new FileInfo(Path.Combine(configDirectory.FullName, GetDataFileName()));
    }

    public void SaveConfig() => SaveConfig(ModuleConfig);
    public void SaveData() => SaveData(ModuleData);
    private string GetDataFileName() => $"{ModuleName.ToString()}.data.json";
    private string GetConfigFileName() => $"{ModuleName.ToString()}.config.json";
    private DirectoryInfo GetCharacterDirectory(ulong contentId)
    {
        var directoryInfo = new DirectoryInfo(Path.Combine(Service.PluginInterface.ConfigDirectory.FullName, contentId.ToString()));

        if (directoryInfo is { Exists: false })
        {
            directoryInfo.Create();
        }

        return directoryInfo;
    }
    
    private void SendStatusMessage()
    {
        if (ModuleConfig is not { OnLoginMessage: true, ModuleEnabled: true, Suppressed: false }) return;
        if (GetModuleStatus() is not (ModuleStatus.Incomplete or ModuleStatus.Unknown)) return;
        if (Condition.IsBoundByDuty()) return;
        if (statusMessageLockout.Elapsed < TimeSpan.FromMinutes(5) && statusMessageLockout.IsRunning)
        {
            PluginLog.Debug($"[{ModuleName}] Suppressing Status Message: {TimeSpan.FromMinutes(5) - statusMessageLockout.Elapsed}");
            return;
        }
        
        PluginLog.Debug($"[{ModuleName}] Sending Status Message");
        
        var statusMessage = GetStatusMessage();
        if (ModuleConfig.UseCustomStatusMessage && GetModuleStatus() != ModuleStatus.Unknown)
        {
            statusMessage.Message = ModuleConfig.CustomStatusMessage;
        }
        statusMessage.SourceModule = ModuleName;
        statusMessage.MessageChannel = GetChatChannel();
        statusMessage.PrintMessage();
        
        statusMessageLockout.Restart();
    }
    
    private void SendResetMessage()
    {
        if (ModuleConfig is not { ResetMessage: true, ModuleEnabled: true, Suppressed: false }) return;
        if (DateTime.UtcNow - ModuleData.NextReset >= TimeSpan.FromMinutes(5)) return;
        
        var statusMessage = GetStatusMessage();
        statusMessage.Message = ModuleConfig.UseCustomResetMessage ? ModuleConfig.CustomResetMessage : Strings.ModuleReset;
        statusMessage.SourceModule = ModuleName;
        statusMessage.MessageChannel = GetChatChannel();
        statusMessage.PrintMessage();
    }

    protected void TryUpdateData<T>(ref T value, T newValue) where T : IEquatable<T>
    {
        if (!value.Equals(newValue))
        {
            value = newValue;
            DataChanged = true;
        }
    }
    
    protected static int GetIncompleteCount<T>(IEnumerable<LuminaTaskConfig<T>> config, IEnumerable<LuminaTaskData<T>> data)
    {
        var result = from taskConfig in config
            join taskData in data on taskConfig.RowId equals taskData.RowId
            where taskConfig.TargetCount != -1 ? taskConfig.Enabled && taskData.CurrentCount < taskConfig.TargetCount : taskConfig.Enabled && !taskData.Complete
            select taskData;

        return result.Count();
    }
}