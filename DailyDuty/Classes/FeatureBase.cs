using System;
using Dalamud.Plugin.Services;
using KamiToolKit;

namespace DailyDuty.Classes;

public abstract class FeatureBase {
    public abstract ModuleInfo ModuleInfo { get; }
    public string Name => ModuleInfo.DisplayName;
    
    public Action? OpenConfigAction { get; set; }
    public abstract NodeBase DisplayNode { get; }
    
    protected bool IsEnabled { get; private set; }

    public void Load()
        => OnFeatureLoad();

    public void Unload()
        => OnFeatureUnload();

    public void Enable() {
        IsEnabled = true;
        
        OnFeatureEnable();

        Services.Framework.Update += Update;
    }

    public void Disable() {
        IsEnabled = false;
        
        OnFeatureDisable();
        
        Services.Framework.Update -= Update;
    }
    
    private void Update(IFramework framework)
        => OnFeatureUpdate();

    protected abstract void OnFeatureUpdate();
    
    protected abstract void OnFeatureLoad();
    protected abstract void OnFeatureUnload();
    
    protected abstract void OnFeatureEnable();
    protected abstract void OnFeatureDisable();
    
    public virtual void ProcessCommand(string args) { }
}
