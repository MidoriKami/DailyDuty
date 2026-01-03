using System;
using System.IO;
using System.Numerics;
using DailyDuty.Enums;
using DailyDuty.Utilities;
using DailyDuty.Windows;
using Dalamud.Utility;
using Newtonsoft.Json.Linq;
using Data = DailyDuty.Utilities.Data;

namespace DailyDuty.Classes;

public abstract class Module<T, TU> : ModuleBase where T : ConfigBase, new() where TU : DataBase, new() {

    private ModuleConfigWindow<Module<T, TU>>? configWindow;

    public T ModuleConfig { get; private set; } = null!;
    public TU ModuleData { get; private set; } = null!;

    public override ConfigBase ConfigBase => ModuleConfig;
    public override DataBase DataBase => ModuleData;
    
    protected abstract CompletionStatus GetCompletionStatus();
    protected virtual void OnModuleEnable() { }
    protected virtual void OnModuleDisable() { }
    protected virtual void OnModuleUpdate() { }

    protected virtual T? MigrateConfig(JObject objectData) => null;

    protected sealed override void OnFeatureLoad() {
        if (!TryMigrateConfig()) {
            ModuleConfig = Config.LoadCharacterConfig<T>($"{ModuleInfo.FileName}.config.json");
            if (ModuleConfig is null) throw new Exception("Failed to load config file");
        
            ModuleConfig.FileName = ModuleInfo.FileName;
        }
        
        ModuleData = Data.LoadCharacterData<TU>($"{ModuleInfo.FileName}.data.json");
        if (ModuleData is null) throw new Exception("Failed to load data file");
        
        ModuleData.FileName = ModuleInfo.FileName;
    }

    // If the config contains a key "ModuleEnabled" then it's from the previous version of DailyDuty and needs to be migrated.
    // This version of DailyDuty stores the enabled state elsewhere.
    private bool TryMigrateConfig() {
        try {
            var fileInfo = new FileInfo(Path.Combine(Config.CharacterConfigPath, $"{ModuleInfo.FileName}.config.json"));
        
            if (fileInfo is { Exists: true }) {
                var fileText = File.ReadAllText(fileInfo.FullName);
                var jObject = JObject.Parse(fileText);

                // Note if MigrationResult is null, the users old config will be nuked.
                if (jObject.ContainsKey("ModuleEnabled") && MigrateConfig(jObject) is { } migrationResult) {
                    Services.PluginLog.Debug($"[{ModuleInfo.DisplayName}] Successfully migrated config file");
                
                    ModuleConfig = migrationResult;
                    ModuleConfig.FileName = ModuleInfo.FileName;
                    ModuleConfig.Save();
                    return true;
                }
            }
        }
        catch (Exception e) {
            Services.PluginLog.Error(e, $"Failed to migrate config file for {ModuleInfo.DisplayName}");
        }

        return false;
    }

    protected sealed override void OnFeatureUnload() {
        ChangelogWindow?.Dispose();
        ChangelogWindow = null;
        
        ModuleData = null!;
        ModuleConfig = null!;
    }

    protected sealed override void OnFeatureEnable() {
        OnModuleEnable();
        OnFeatureUpdate();
        SendLoginMessage();

        OpenConfigAction = () => {
            configWindow ??= new ModuleConfigWindow<Module<T, TU>> {
                Module = this,
                InternalName = $"{GetType().Name}ConfigWindow",
                Title = $"{ModuleInfo.DisplayName} Config",
                Size = new Vector2(400.0f, 500.0f),
            };
            
            configWindow.Toggle();
        };
    }

    protected sealed override void OnFeatureDisable() {
        OnModuleDisable();
        
        OpenConfigAction = null;
        
        configWindow?.Dispose();
        configWindow = null;
    }
    
    protected sealed override void OnModuleBaseUpdate() {
        TryReset();
        
        OnModuleUpdate();
    }

    protected sealed override void OnTerritoryChanged() {
        if (!ModuleConfig.OnZoneChangeMessage) return;
        if (ModuleInfo.Type is ModuleType.GeneralFeatures) return;
        if (ModuleStatus is not (CompletionStatus.Incomplete or CompletionStatus.Unknown)) return;
        if (Services.Condition.IsBoundByDuty) return;
        
        PrintStatusMessage(StatusMessageType.ZoneChanged);
    }

    private void SendLoginMessage() {
        if (!ModuleConfig.OnLoginMessage) return;
        if (ModuleInfo.Type is ModuleType.GeneralFeatures) return;
        if (ModuleStatus is not (CompletionStatus.Incomplete or CompletionStatus.Unknown)) return;
        if (Services.Condition.IsBoundByDuty) return;
        
        PrintStatusMessage(StatusMessageType.Login);
    }
    
    private void TryReset() {
        if (ModuleInfo.Type is ModuleType.GeneralFeatures) return;
        if (DateTime.UtcNow <= ModuleData.NextReset) return;

        if (ModuleConfig.ResetMessage) {
            PrintStatusMessage(StatusMessageType.Reset);
        }

        Reset();
        
        var nextReset = GetNextResetDateTime();
        Services.PluginLog.Debug($"Resetting {ModuleInfo.DisplayName}, next reset at {nextReset.ToLocalTime().GetDisplayString()}");
        
        ModuleData.NextReset = nextReset;
        ModuleData.Save();

        ModuleConfig.Suppressed = false;
        ModuleConfig.Save();
    }

    private void PrintStatusMessage(StatusMessageType type) {
        var customMessage = type switch {
            StatusMessageType.Login => ModuleConfig.CustomStatusMessage,
            StatusMessageType.ZoneChanged => ModuleConfig.CustomStatusMessage,
            StatusMessageType.Reset => ModuleConfig.CustomResetMessage,
            _ => string.Empty,
        };

        var statusMessage = ModuleStatusMessage;
        
        if (!customMessage.IsNullOrEmpty()) {
            statusMessage.Message = customMessage;
        }
        
        Services.PluginLog.Debug($"[{ModuleInfo.DisplayName}] Sending {type.ToString()} Message");
        Services.ChatGui.PrintPayloadMessage(
            ModuleConfig.MessageChatChannel, 
            statusMessage.PayloadId, 
            ModuleInfo.DisplayName, 
            statusMessage.Message
        );
    }

    protected override CompletionStatus GetModuleStatus() {
        if (!IsEnabled) return CompletionStatus.Disabled;
        if (ModuleConfig.Suppressed) return CompletionStatus.Suppressed;

        return GetCompletionStatus();
    }
}
