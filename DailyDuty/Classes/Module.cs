using Dalamud.Plugin.Services;

namespace DailyDuty.Classes;

public abstract class Module<T, TU> : ModuleBase where T : ConfigBase, new() where TU : DataBase, new() {

    protected T? ModuleConfig { get; private set; }
    protected TU? ModuleData { get; private set; }

    public override void Enable() {
        ModuleConfig = Utilities.Config.LoadCharacterConfig<T>($"{ModuleInfo.FileName}.config.json");
        ModuleConfig.FileName = ModuleInfo.FileName;
        
        ModuleData = Utilities.Data.LoadCharacterData<TU>($"{ModuleInfo.FileName}.data.json");
        ModuleData.FileName = ModuleInfo.FileName;

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
