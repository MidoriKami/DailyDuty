using System;
using DailyDuty.Classes.Nodes;
using DailyDuty.Enums;
using Lumina.Text.ReadOnly;

namespace DailyDuty.Classes;

public abstract class ModuleBase {
    public abstract ModuleInfo ModuleInfo { get; }
    public string Name => ModuleInfo.DisplayName;
    
    public abstract ConfigBase ConfigBase { get; }
    public abstract DataBase DataBase { get; }
    
    public virtual void ProcessCommand(string args) { }

    public abstract void Load();
    public abstract void Unload();

    public abstract void Enable();
    public abstract void Disable();
    
    public Action? OpenConfigAction { get; set; }

    public abstract DataNodeBase GetDataNode();
    public abstract ConfigNodeBase GetConfigNode();
    
    protected abstract void OnEnable();
    protected abstract void OnDisable();

    protected virtual void Update() {
        ModuleStatus = GetModuleStatus();
        ModuleStatusMessage = GetStatusMessage();
    }

    public CompletionStatus ModuleStatus { get; private set; }
    public ReadOnlySeString ModuleStatusMessage { get; private set; }
    
    protected abstract CompletionStatus GetModuleStatus();
    protected abstract ReadOnlySeString GetStatusMessage();
    public abstract DateTime GetNextResetDateTime();
    public abstract void Reset();
}
