using System;
using DailyDuty.Classes.Nodes;
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
    
    protected virtual void OnEnable() { }
    protected virtual void OnDisable() { }

    public override NodeBase DisplayNode => DataNode;
    public virtual DataNodeBase DataNode => new GenericDataNodeBase(this);
    public virtual ConfigNodeBase ConfigNode => new GenericConfigNodeBase(this);

    protected virtual void Update() {
        ModuleStatus = GetModuleStatus();

        if (ModuleStatus is not CompletionStatus.Complete) {
            ModuleStatusMessage = GetStatusMessage();
        }
    }

    public CompletionStatus ModuleStatus { get; private set; }
    public StatusMessage ModuleStatusMessage { get; private set; } = new();
    
    protected abstract CompletionStatus GetModuleStatus();
    protected abstract StatusMessage GetStatusMessage();
    public abstract DateTime GetNextResetDateTime();
    public abstract TimeSpan GetResetPeriod();
    public virtual void Reset() { }
    public virtual ReadOnlySeString? GetTooltip() => null;

    public virtual void OnNpcInteract(EventFramework* thisPtr, GameObject* gameObject, EventId eventId, short scene, ulong sceneFlags, uint* sceneData, byte sceneDataCount) { }
}
