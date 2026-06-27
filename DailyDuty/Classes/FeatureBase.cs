using System;
using System.Threading.Tasks;
using Dalamud.Plugin.Services;
using KamiToolKit.BaseTypes;

namespace DailyDuty.Classes;

public abstract class FeatureBase {
    public abstract ModuleInfo ModuleInfo { get; }
    public string Name => ModuleInfo.DisplayName;

    public Action? OpenConfigAction { get; set; }
    public abstract NodeBase DisplayNode { get; }

    public bool IsEnabled { get; private set; }

    public async Task Load()
        => await OnFeatureLoad();

    public async Task Unload()
        => await OnFeatureUnload();

    public async Task Enable() {
        IsEnabled = true;

        await OnFeatureEnable();

        Services.Framework.Update += Update;
        Services.ClientState.TerritoryChanged += TerritoryChanged;
    }

    public async Task Disable() {
        IsEnabled = false;

        Services.Framework.Update -= Update;
        Services.ClientState.TerritoryChanged -= TerritoryChanged;

        await OnFeatureDisable();
    }

    private void Update(IFramework framework)
        => OnFeatureUpdate();

    private void TerritoryChanged(uint u)
        => OnTerritoryChanged();

    protected abstract void OnFeatureUpdate();
    protected virtual void OnTerritoryChanged() { }

    protected abstract Task OnFeatureLoad();
    protected abstract Task OnFeatureUnload();

    protected abstract Task OnFeatureEnable();
    protected abstract Task OnFeatureDisable();

    public virtual void ProcessCommand(string args) { }
}
