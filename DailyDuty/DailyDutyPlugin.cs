using DailyDuty.Classes;
using Dalamud.Plugin;
using DailyDuty.Models;
using DailyDuty.Windows;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI;
using KamiLib.Classes;
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

        System.Teleporter = new Teleporter(Service.PluginInterface);
        
        System.CommandManager = new CommandManager(Service.PluginInterface, "dd", "dailyduty");
        System.LocalizationController = new LocalizationController();
        System.PayloadController = new PayloadController();
        System.ContentsFinderController = new AddonController<AddonContentsFinder>("ContentsFinder");
        
        System.ModuleController = new ModuleController();
        System.TodoListController = new TodoListController();
        System.TimersController = new TimersController();
        System.OverlayController = new OverlayController();

        System.ConfigurationWindow = new ConfigurationWindow();
        System.WindowManager = new WindowManager(Service.PluginInterface);
        System.WindowManager.AddWindow(System.ConfigurationWindow, WindowFlags.IsConfigWindow | WindowFlags.RequireLoggedIn);

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
        
        System.WindowManager.Dispose();
        System.LocalizationController.Dispose();
        System.PayloadController.Dispose();
        System.OverlayController.Dispose();
        System.ContentsFinderController.Dispose();

        System.ModuleController.Dispose();

        System.CommandManager.Dispose();

        System.TodoListController.Dispose();
        System.TimersController.Dispose();
        
        System.NativeController.Dispose();
    }
    
    private static void OnFrameworkUpdate(IFramework framework) {
        if (!Service.ClientState.IsLoggedIn) return;

        // Check for reset, and reset modules that need it 
        System.ModuleController.ResetModules();
        
        // Update All Modules
        System.ModuleController.UpdateModules();
        
        System.TodoListController.Update();
        System.TimersController.Update();
    }
    
    private static void OnLogin() {
        System.SystemConfig = SystemConfig.Load();
        System.ModuleController.LoadModules();
        System.ContentsFinderController.Enable();
        System.OverlayController.Enable();
    }
    
    private static void OnLogout(int type, int code) {
        System.OverlayController.Disable();
        System.ContentsFinderController.Disable();
        System.ModuleController.UnloadModules();
    }
    
    private static void OnZoneChange(ushort territoryTypeId) {
        if (Service.ClientState.IsPvP) return;
        if (!Service.ClientState.IsLoggedIn) return;
        
        System.ModuleController.ZoneChange(territoryTypeId);
    }
}