using System;
using System.Numerics;
using DailyDuty.Enums;
using DailyDuty.Extensions;
using DailyDuty.Utilities;
using DailyDuty.Windows;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using Lumina.Text.ReadOnly;

namespace DailyDuty.Classes;

public abstract class Module<T, TU> : ModuleBase where T : ConfigBase, new() where TU : DataBase, new() {

    private ModuleConfigWindow<Module<T, TU>>? configWindow;

    public T ModuleConfig { get; private set; } = null!;
    public TU ModuleData { get; private set; } = null!;

    public override ConfigBase ConfigBase => ModuleConfig;
    public override DataBase DataBase => ModuleData;

    private bool isEnabled;

    public sealed override void Load() {
        ModuleConfig = Config.LoadCharacterConfig<T>($"{ModuleInfo.FileName}.config.json");
        if (ModuleConfig is null) throw new Exception("Failed to load config file");
        
        ModuleConfig.FileName = ModuleInfo.FileName;
        
        ModuleData = Data.LoadCharacterData<TU>($"{ModuleInfo.FileName}.data.json");
        if (ModuleData is null) throw new Exception("Failed to load data file");
        
        ModuleData.FileName = ModuleInfo.FileName;

        Services.Framework.Update += OnUpdate;
    }

    public sealed override void Unload() {
        Services.Framework.Update -= OnUpdate;
        
        ChangelogWindow?.Dispose();
        ChangelogWindow = null;
        
        ModuleData = null!;
        ModuleConfig = null!;
    }

    public sealed override void Enable() {
        isEnabled = true;

        OnUpdate(Services.Framework);

        OpenConfigAction = () => {
            configWindow ??= new ModuleConfigWindow<Module<T, TU>> {
                Module = this,
                InternalName = $"{GetType().Name}ConfigWindow",
                Title = $"{ModuleInfo.DisplayName} Config",
                Size = new Vector2(400.0f, 500.0f),
            };
            
            configWindow.Toggle();
        };
        
        OnEnable();
        Update();
        SendStatusMessage();
    }
    
    public sealed override void Disable() {
        isEnabled = false;

        OpenConfigAction = null;
        
        configWindow?.Dispose();
        configWindow = null;
        
        OnDisable();
    }
    
    private void OnUpdate(IFramework framework) {
        TryReset();
        
        Update();

        if (ModuleConfig.SavePending) {
            Services.PluginLog.Debug($"Saving {ModuleInfo.DisplayName} config");
            ModuleConfig.Save();
        }

        if (ModuleData.SavePending) {
            Services.PluginLog.Debug($"Saving {ModuleInfo.DisplayName} data");
            ModuleData.Save();
        }
    }

    private void SendStatusMessage() {
        if (ModuleInfo.Type is ModuleType.GeneralFeatures) return;
        if (ModuleStatus is not (CompletionStatus.Incomplete or CompletionStatus.Unknown)) return;
        if (Services.Condition.IsBoundByDuty) return;

        ReadOnlySeString statusMessage;
        if (ModuleConfig.CustomStatusMessage.IsNullOrEmpty()) {
            statusMessage = GetStatusMessage();
        }
        else {
            statusMessage = ModuleConfig.CustomStatusMessage;
        }
        
        Services.PluginLog.Debug($"[{ModuleInfo.DisplayName}] Sending Status Message");
        Services.ChatGui.PrintPayloadMessage(
            ModuleConfig.MessageChatChannel, 
            ModuleInfo.MessageClickAction, 
            ModuleInfo.DisplayName, 
            statusMessage.ToString()
        );
    }
    
    private void TryReset() {
        if (ModuleInfo.Type is ModuleType.GeneralFeatures) return;
        if (DateTime.UtcNow <= ModuleData.NextReset) return;

        if (ModuleConfig.ResetMessage) {
            Services.ChatGui.PrintMessage(ModuleConfig.MessageChatChannel, ModuleInfo.DisplayName, GetResetMessage());
        }

        Reset();
        
        var nextReset = GetNextResetDateTime();
        Services.PluginLog.Debug($"Resetting {ModuleInfo.DisplayName}, next reset at {nextReset.ToLocalTime().GetDisplayString()}");
        
        ModuleData.NextReset = nextReset;
        ModuleData.Save();

        ModuleConfig.Suppressed = false;
        ModuleConfig.Save();
    }

    private string GetResetMessage() {
        if (ModuleConfig.CustomResetMessage is { Length: > 0 }) return ModuleConfig.CustomResetMessage;

        return $"Resetting {ModuleInfo.DisplayName} module";
    }

    protected override CompletionStatus GetModuleStatus() {
        if (!isEnabled) return CompletionStatus.Disabled;
        if (ModuleConfig.Suppressed) return CompletionStatus.Suppressed;

        return GetCompletionStatus();
    }

    protected abstract CompletionStatus GetCompletionStatus();
}
