using System;
using System.Diagnostics;
using System.Linq;
using DailyDuty.Models;
using DailyDuty.Models.Attributes;
using DailyDuty.Models.Enums;
using DailyDuty.System;
using DailyDuty.System.Helpers;
using DailyDuty.System.Localization;
using DailyDuty.Views.Components;
using Dalamud.Game.Text;
using Dalamud.Logging;
using KamiLib.GameState;
using Lumina.Excel;

namespace DailyDuty.Abstracts;

public abstract class BaseModule : IDisposable
{
    public abstract ModuleDataBase ModuleData { get; protected set; }
    public abstract ModuleConfigBase ModuleConfig { get; protected set; }
    public abstract ModuleName ModuleName { get; }
    public abstract ModuleType ModuleType { get; }
    public ModuleStatus ModuleStatus => ModuleConfig.Suppressed ? ModuleStatus.Suppressed : GetModuleStatus();
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
    protected virtual void UpdateTaskLists() { }

    public void DrawConfig()
    {
        var configOptions = AttributeHelper.GetFieldAttributes<ConfigOption>(ModuleConfig);
        var clickableLinks = AttributeHelper.GetFieldAttributes<ClickableLink>(ModuleConfig);
        var selectableTasks = AttributeHelper.GetFieldAttributes<SelectableTasks>(ModuleConfig);
        var todoOptions = AttributeHelper.GetFieldAttributes<ConfigOption>(ModuleConfig.TodoOptions);
        
        ModuleEnableView.Draw(ModuleConfig, SaveConfig);
        ModuleClickableLinkConfigView.Draw(clickableLinks, ModuleConfig, SaveConfig);
        GenericConfigView.Draw(configOptions, ModuleConfig, SaveConfig, Strings.ModuleConfiguration);
        ModuleSelectableTaskView.DrawConfig(selectableTasks.FirstOrDefault().Item1, ModuleConfig, SaveConfig);
        ModuleNotificationOptionsView.Draw(ModuleConfig, SaveConfig);
        GenericConfigView.Draw(todoOptions, ModuleConfig.TodoOptions, () => {
            SaveConfig();
            ModuleConfig.TodoOptions.StyleChanged = true;
        }, Strings.TodoConfiguration);
    }

    public void DrawData()
    {
        var dataDisplay = AttributeHelper.GetFieldAttributes<DataDisplay>(ModuleData);
        var taskSelection = AttributeHelper.GetFieldAttributes<SelectableTasks>(ModuleData);

        ModuleStatusView.Draw(this);
        ModuleResetView.Draw(ModuleData);
        ModuleDataView.Draw(dataDisplay, ModuleData);
        ModuleSelectableTaskView.DrawData(taskSelection.FirstOrDefault().Item1, ModuleData);
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
        
        UpdateTaskLists();
        
        Update();
        
        if (ModuleConfig is { OnLoginMessage: true, ModuleEnabled: true, Suppressed: false })
        {
            SendStatusMessage();
        }
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
        if (ModuleConfig is { OnZoneChangeMessage: true, ModuleEnabled: true, Suppressed: false })
        {
            SendStatusMessage();
        }
    }
    
    private ModuleConfigBase LoadConfig() => (ModuleConfigBase) FileController.LoadFile($"{ModuleName}.config.json", ModuleConfig);
    private ModuleDataBase LoadData() => (ModuleDataBase) FileController.LoadFile($"{ModuleName}.data.json", ModuleData);
    public void SaveConfig() => FileController.SaveFile($"{ModuleName}.config.json", ModuleConfig.GetType(), ModuleConfig);
    public void SaveData() => FileController.SaveFile($"{ModuleName}.data.json", ModuleData.GetType(), ModuleData);

    private void SendStatusMessage()
    {
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
    
    protected static int GetIncompleteCount<T>(LuminaTaskConfigList<T> config, LuminaTaskDataList<T> data) where T : ExcelRow
    {
        if (config.Count != data.Count) throw new Exception("Task and Data array size are mismatched. Unable to calculate IncompleteCount");

        var queryResult = 
            from configTask in config
            join dataTask in data on configTask.RowId equals dataTask.RowId
            where 
                (configTask.Enabled && configTask.TargetCount is not 0 && dataTask.CurrentCount < configTask.TargetCount) ||
                (configTask.Enabled && configTask.TargetCount is 0 && !dataTask.Complete)
            select configTask;

        return queryResult.Count();
    }
}