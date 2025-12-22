using System;
using DailyDuty.Enums;
using KamiToolKit;
using Lumina.Text.ReadOnly;

namespace DailyDuty.Classes;

public abstract class ModuleBase {
    public abstract ModuleInfo ModuleInfo { get; }
    public string Name => ModuleInfo.DisplayName;

    public virtual void ProcessCommand(string args) { }

    public abstract void Enable();
    public abstract void Disable();
    
    public Action? OpenConfigAction { get; set; }

    public abstract NodeBase GetStatusDisplayNode();
    
    protected abstract void OnEnable();
    protected abstract void OnDisable();
    protected virtual void Update() { }

    public abstract CompletionStatus? GetModuleStatus();
    public abstract ReadOnlySeString? GetStatusMessage();
}
