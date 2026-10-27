using System.Threading.Tasks;
using Dalamud.Excel.Sheets;
using Dalamud.Plugin.Services;
using KamiToolKit.BaseTypes;
using Lumina.Excel;
using Action = System.Action;

namespace DailyDuty.Classes;

public abstract class FeatureBase {
    public abstract ModuleInfo ModuleInfo { get; }
    public string Name => ModuleInfo.DisplayName;

    public Action? OpenConfigAction { get; set; }
    public abstract NodeBase DisplayNode { get; }

    public bool IsEnabled { get; private set; }

    public async Task LoadAsync()
        => await OnFeatureLoad();

    public async Task UnloadAsync()
        => await OnFeatureUnload();

    public async Task Enable() {
        IsEnabled = true;

        await OnFeatureEnable();

        IFramework.Get().Update += Update;
        IClientState.Get().TerritoryChanged += TerritoryChanged;
    }

    public async Task Disable() {
        IsEnabled = false;

        IFramework.Get().Update -= Update;
        IClientState.Get().TerritoryChanged -= TerritoryChanged;

        await OnFeatureDisable();
    }

    private void Update(IFramework framework)
        => OnFeatureUpdate();

    private void TerritoryChanged(RowRef<TerritoryType> rowRef)
        => OnTerritoryChanged();

    protected abstract void OnFeatureUpdate();
    protected virtual void OnTerritoryChanged() { }

    protected abstract Task OnFeatureLoad();
    protected abstract Task OnFeatureUnload();

    protected abstract Task OnFeatureEnable();
    protected abstract Task OnFeatureDisable();

    public virtual void ProcessCommand(string args) { }
}
