using System;
using DailyDuty.Enums;
using DailyDuty.Extensions;
using DailyDuty.Utilities;
using Dalamud.Game.Text;
using Dalamud.Plugin.Services;

namespace DailyDuty.Classes;

public abstract class Module<T, TU> : ModuleBase where T : ConfigBase, new() where TU : DataBase, new() {

    // These are effectively not nullable, for the lifetime of a user session they are expected to be valid.
    public T ModuleConfig { get; private set; } = null!;
    public TU ModuleData { get; private set; } = null!;

    public override void Enable() {
        ModuleConfig = Config.LoadCharacterConfig<T>($"{ModuleInfo.FileName}.config.json");
        if (ModuleConfig is null) throw new Exception("Failed to load config file");
        
        ModuleConfig.FileName = ModuleInfo.FileName;
        
        ModuleData = Data.LoadCharacterData<TU>($"{ModuleInfo.FileName}.data.json");
        if (ModuleData is null) throw new Exception("Failed to load data file");
        
        ModuleData.FileName = ModuleInfo.FileName;

        Services.Framework.Update += OnUpdate;
        
        OnEnable();
        SendStatusMessage();
    }
    
    public override void Disable() {
        OnDisable();
        
        Services.Framework.Update -= OnUpdate;
        
        ModuleData = null!;
        ModuleConfig = null!;
    }
    
    private void OnUpdate(IFramework framework) {
        TryReset();
        
        Update();
    }

    private void SendStatusMessage() {
        var moduleStatus = GetModuleStatus();

        if (moduleStatus is not (CompletionStatus.Incomplete or CompletionStatus.Unknown)) return;
        if (Services.Condition.IsBoundByDuty) return;
        
        var statusMessage = ModuleConfig.CustomStatusMessage != string.Empty ? ModuleConfig.CustomStatusMessage : GetStatusMessage();
        
        Services.PluginLog.Debug($"[{ModuleInfo.DisplayName}] Sending Status Message");
        Services.ChatGui.PrintMessage(ModuleConfig.MessageChatChannel, ModuleInfo.DisplayName, statusMessage.ExtractText());
    }
    
    private void TryReset() {
        if (DateTime.UtcNow <= ModuleData.NextReset) return;
        if (GetResetMessage() is not {} message) return;
        
        Services.ChatGui.PrintMessage(GetMessageChannel(), ModuleInfo.DisplayName, message);
        Reset();
        
        ModuleData.NextReset = GetNextResetDateTime();
        ModuleData.Save();

        ModuleConfig.Suppressed = false;
        ModuleConfig.Save();
    }

    public override string GetResetMessage()
        => ModuleConfig.CustomResetMessage != string.Empty ? ModuleConfig.CustomResetMessage : $"Resetting {ModuleInfo.DisplayName} module";

    public override XivChatType GetMessageChannel()
        => ModuleConfig.MessageChatChannel;

    public override CompletionStatus GetModuleStatus()
        => ModuleConfig.Suppressed ? CompletionStatus.Suppressed : GetCompletionStatus();

    protected abstract CompletionStatus GetCompletionStatus();

    public override DateTime GetCurrentResetDateTime()
        => ModuleData.NextReset;
}
