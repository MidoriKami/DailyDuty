using DailyDuty.Classes;
using DailyDuty.Classes.Timers;
using DailyDuty.Classes.TodoList;
using Dalamud.Plugin;
using DailyDuty.Models;
using DailyDuty.Windows;
using Dalamud.Plugin.Services;
using KamiLib.CommandManager;
using KamiLib.Window;
using KamiToolKit;

namespace DailyDuty;

public sealed class DailyDutyPlugin : IDalamudPlugin {
    public DailyDutyPlugin(IDalamudPluginInterface pluginInterface) {
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
        System.TimersController = new TimersController();

        System.ConfigurationWindow = new ConfigurationWindow();
        System.WindowManager = new WindowManager(Service.PluginInterface);
        System.WindowManager.AddWindow(System.ConfigurationWindow, WindowFlags.IsConfigWindow | WindowFlags.RequireLoggedIn | WindowFlags.OpenImmediately);
        System.WindowManager.AddWindow(new WonderousTailsDebugWindow());

        if (Service.ClientState.IsLoggedIn) {
            Service.Framework.RunOnFrameworkThread(OnLogin);
        }
        
        Service.Framework.Update += OnFrameworkUpdate;
        Service.ClientState.Login += OnLogin;
        Service.ClientState.Logout += OnLogout;
        Service.ClientState.TerritoryChanged += OnZoneChange;
    }

    public void Dispose() {
        Service.Framework.Update -= OnFrameworkUpdate;
        Service.ClientState.Login -= OnLogin;
        Service.ClientState.Logout -= OnLogout;
        Service.ClientState.TerritoryChanged -= OnZoneChange;
        
        System.LocalizationController.Dispose();
        System.PayloadController.Dispose();
        System.ModuleController.Dispose();
        System.WindowManager.Dispose();
        System.CommandManager.Dispose();
        System.TodoListController.Dispose();
        System.TimersController.Dispose();
        
        System.NativeController.Dispose();
    }
    
    private void OnFrameworkUpdate(IFramework framework) {
        if (!Service.ClientState.IsLoggedIn) return;

        // Check for reset, and reset modules that need it 
        System.ModuleController.ResetModules();
        
        // Update All Modules
        System.ModuleController.UpdateModules();
        
        System.TodoListController.Update();
        System.TimersController.Update();
    }
    
    private void OnLogin() {
        System.SystemConfig = SystemConfig.Load();
        
        System.TodoListController.Enable();
        System.TimersController.Enable();
        
        System.ModuleController.LoadModules();
    }
    
    private void OnLogout(int type, int code) {
        System.ModuleController.UnloadModules();
        
        System.TodoListController.Disable();
        System.TimersController.Disable();
    }
    
    private void OnZoneChange(ushort territoryTypeId) {
        if (Service.ClientState.IsPvP) return;
        if (!Service.ClientState.IsLoggedIn) return;
        
        System.ModuleController.ZoneChange(territoryTypeId);
    }
}