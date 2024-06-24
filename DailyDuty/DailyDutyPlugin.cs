using DailyDuty.Classes;
using Dalamud.Plugin;
using DailyDuty.Models;
using DailyDuty.Views;
using Dalamud.Plugin.Services;
using KamiLib.CommandManager;
using KamiLib.Window;

namespace DailyDuty;

public sealed class DailyDutyPlugin : IDalamudPlugin {
    public DailyDutyPlugin(DalamudPluginInterface pluginInterface) {
        pluginInterface.Create<Service>();

        // Load placeholder SystemConfig, we will load the correct one for the player once they log in.
        System.SystemConfig = new SystemConfig();

        System.TeleporterController = new TeleporterController();
        
        System.CommandManager = new CommandManager(Service.PluginInterface, "dd", "dailyduty");

        System.LocalizationController = new LocalizationController();
        System.PayloadController = new PayloadController();
        System.ModuleController = new ModuleController();
        
        System.ConfigurationWindow = new ConfigurationWindow();

        System.WindowManager = new WindowManager(Service.PluginInterface);
        System.WindowManager.AddWindow(System.ConfigurationWindow, WindowFlags.IsConfigWindow | WindowFlags.RequireLoggedIn | WindowFlags.OpenImmediately);
        
        if (Service.ClientState.IsLoggedIn) {
            OnLogin();
        }
        
        Service.Framework.Update += OnFrameworkUpdate;
        Service.ClientState.Login += OnLogin;
        Service.ClientState.Logout += OnLogout;
        Service.ClientState.TerritoryChanged += OnZoneChange;
        Service.ClientState.EnterPvP += OnEnterPvP;
        Service.ClientState.LeavePvP += OnLeavePvP;
    }

    public void Dispose() {
        Service.Framework.Update -= OnFrameworkUpdate;
        Service.ClientState.Login -= OnLogin;
        Service.ClientState.Logout -= OnLogout;
        Service.ClientState.TerritoryChanged -= OnZoneChange;
        Service.ClientState.EnterPvP -= OnEnterPvP;
        Service.ClientState.LeavePvP -= OnLeavePvP;
        
        System.LocalizationController.Dispose();
        System.PayloadController.Dispose();
        System.ModuleController.Dispose();
        System.WindowManager.Dispose();
        System.CommandManager.Dispose();
    }
    
    private void OnFrameworkUpdate(IFramework framework) {
        if (Service.ClientState.IsPvP) return;
        if (!Service.ClientState.IsLoggedIn) return;

        // Check for reset, and reset modules that need it 
        System.ModuleController.ResetModules();
        
        // Update All Modules
        System.ModuleController.UpdateModules();
        
        // Update TodoDisplay
        // TodoController.Update(); // todo: this
    }
    
    private void OnLogin() {
        System.SystemConfig = SystemConfig.Load();
        
        System.ModuleController.LoadModules();
        
        // TodoController.OnLogin(); // todo: this
    }
    
    private void OnLogout() {
        System.ModuleController.UnloadModules();
        
        // TodoController.OnLogout();  // todo: this
    }
    
    private void OnZoneChange(ushort territoryTypeId)
    {
        if (Service.ClientState.IsPvP) return;
        if (!Service.ClientState.IsLoggedIn) return;
        
        System.ModuleController.ZoneChange(territoryTypeId);
    }
    
    private void OnLeavePvP() {
        // TodoController.Show(); // todo: this
    }

    private void OnEnterPvP() {
        // TodoController.Hide(); // todo: this
    }

}