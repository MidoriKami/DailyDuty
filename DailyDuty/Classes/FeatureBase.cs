using System;
using KamiToolKit;

namespace DailyDuty.Classes;

public abstract class FeatureBase {
    public abstract ModuleInfo ModuleInfo { get; }
    public string Name => ModuleInfo.DisplayName;
    
    public Action? OpenConfigAction { get; set; }
    
    public virtual void ProcessCommand(string args) { }
    
    public bool IsEnabled { get; set; }
    
    public abstract void Load();
    public abstract void Unload();

    public abstract void Enable();
    public abstract void Disable();
    
    public abstract NodeBase DisplayNode { get; }
}
