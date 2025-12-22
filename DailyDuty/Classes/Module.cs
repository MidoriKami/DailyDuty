using Dalamud.Plugin.Services;

namespace DailyDuty.Classes;

public abstract class Module<TU, T> : ModuleBase where T: ModuleData<T>, new() where TU : ModuleConfig<TU>, new() {

    protected T? ModuleData { get; private set; }
    protected TU? ModuleConfig { get; private set; }

    public override void Enable() {
        var dataFilePath = new T().GetFileName();
        ModuleData = Utilities.Data.LoadCharacterData<T>(dataFilePath);
        
        var configFilePath = new TU().GetFileName();
        ModuleConfig = Utilities.Config.LoadCharacterConfig<TU>(configFilePath);

        Services.Framework.Update += OnUpdate;
        
        OnEnable();
    }
    
    public override void Disable() {
        OnDisable();
        
        Services.Framework.Update -= OnUpdate;
        
        ModuleData = null;
        ModuleConfig = null;
    }
    
    private void OnUpdate(IFramework framework) {
        Update();
    }
}
