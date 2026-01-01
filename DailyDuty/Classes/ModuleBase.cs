using System;
using DailyDuty.CustomNodes;
using DailyDuty.Enums;
using DailyDuty.Windows;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using KamiToolKit;
using Lumina.Text.ReadOnly;

namespace DailyDuty.Classes;

public abstract unsafe class ModuleBase : FeatureBase {
    
    public abstract ConfigBase ConfigBase { get; }
    public abstract DataBase DataBase { get; }

    public ChangelogWindow? ChangelogWindow { get; set; }

    public override NodeBase DisplayNode => DataNode;
    public virtual DataNodeBase DataNode => new GenericDataNodeBase(this);
    public virtual ConfigNodeBase ConfigNode => new GenericConfigNodeBase(this);
    
    public CompletionStatus ModuleStatus { get; private set; }
    public StatusMessage ModuleStatusMessage { get; private set; } = new();
    
    protected override void OnFeatureEnable() { }
    protected override void OnFeatureDisable() { }

    protected abstract CompletionStatus GetModuleStatus();
    protected abstract StatusMessage GetStatusMessage();
    public abstract DateTime GetNextResetDateTime();
    public abstract TimeSpan GetResetPeriod();
    public virtual void Reset() { }
    public virtual ReadOnlySeString? GetTooltip() => null;

    public virtual void OnNpcInteract(EventFramework* thisPtr, GameObject* gameObject, EventId eventId, short scene, ulong sceneFlags, uint* sceneData, byte sceneDataCount) { }
    
    protected abstract void OnModuleBaseUpdate();
    
    protected sealed override void OnFeatureUpdate() {
        ModuleStatus = GetModuleStatus();

        if (ModuleStatus is not CompletionStatus.Complete) {
            ModuleStatusMessage = GetStatusMessage();
        }
        
        if (ConfigBase.SavePending) {
            Services.PluginLog.Debug($"Saving {ModuleInfo.DisplayName} config");
            ConfigBase.Save();
        }

        if (DataBase.SavePending) {
            Services.PluginLog.Debug($"Saving {ModuleInfo.DisplayName} data");
            DataBase.Save();
        }
        
        OnModuleBaseUpdate();
    }
}
