using DailyDuty.Classes;
using DailyDuty.Classes.TodoList;
using Dalamud.Plugin;
using DailyDuty.Models;
using DailyDuty.Views;
using Dalamud.Plugin.Services;
using KamiLib.CommandManager;
using KamiLib.Window;
using KamiToolKit;

namespace DailyDuty;

public sealed class DailyDutyPlugin : IDalamudPlugin {
    public DailyDutyPlugin(DalamudPluginInterface pluginInterface) {
        pluginInterface.Create<Service>();

        // Load placeholder SystemConfig, we will load the correct one for the player once they log in.
        System.SystemConfig = new SystemConfig();
        System.NativeController = new NativeController(Service.PluginInterface);

        System.TeleporterController = new TeleporterController();
        
        System.CommandManager = new CommandManager(Service.PluginInterface, "dd", "dailyduty");
        System.LocalizationController = new LocalizationController();
        System.PayloadController = new PayloadController();
        
        System.ModuleController = new ModuleController();
        System.TodoListController = new TodoListController();

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
        System.TodoListController.Dispose();
        
        System.NativeController.Dispose();
    }
    
    private void OnFrameworkUpdate(IFramework framework) {
        if (Service.ClientState.IsPvP) return;
        if (!Service.ClientState.IsLoggedIn) return;

        // Check for reset, and reset modules that need it 
        System.ModuleController.ResetModules();
        
        // Update All Modules
        System.ModuleController.UpdateModules();
    }
    
    private void OnLogin() {
        System.SystemConfig = SystemConfig.Load();
        
        System.ModuleController.LoadModules();
        
        System.TodoListController.Load();
    }
    
    private void OnLogout() {
        System.ModuleController.UnloadModules();
        
        System.TodoListController.Unload();
    }
    
    private void OnZoneChange(ushort territoryTypeId)
    {
        if (Service.ClientState.IsPvP) return;
        if (!Service.ClientState.IsLoggedIn) return;
        
        System.ModuleController.ZoneChange(territoryTypeId);
    }
    
    private void OnLeavePvP()
        => System.TodoListController.Refresh();

    private void OnEnterPvP()
        => System.TodoListController.Refresh();
}