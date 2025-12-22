using System.Numerics;
using DailyDuty.Classes;
using DailyDuty.ConfigurationWindow;
using Dalamud.Plugin;
using KamiToolKit;

namespace DailyDuty;

public sealed class DailyDutyPlugin : IDalamudPlugin {
    public DailyDutyPlugin(IDalamudPluginInterface pluginInterface) {
        pluginInterface.Create<Services>();

        KamiToolKitLibrary.Initialize(pluginInterface);

        System.ConfigurationWindow = new ConfigWindow {
            InternalName = "DailyDutyConfig",
            Title = "Daily Duty Configuration",
            Size = new Vector2(700.0f, 600.0f),
        };
        
        System.ConfigurationWindow.DebugOpen();
        
        System.PayloadController = new PayloadController();
        System.ModuleManager = new ModuleManager();

        if (Services.ClientState.IsLoggedIn) {
            Services.Framework.RunOnFrameworkThread(OnLogin);
        }
        
        Services.ClientState.Login += OnLogin;
        Services.ClientState.Logout += OnLogout;
    }

    public void Dispose() {
        Services.ClientState.Login -= OnLogin;
        Services.ClientState.Logout -= OnLogout;
        
        System.PayloadController.Dispose();
        System.ModuleManager.Dispose();
        
        System.ConfigurationWindow.Dispose();
        
        KamiToolKitLibrary.Dispose();
    }

    private static void OnLogin() {
        System.SystemConfig = SystemConfig.Load();
        System.ModuleManager.LoadModules();
    }
    
    private static void OnLogout(int type, int code) {
        System.ModuleManager.UnloadModules();
        System.SystemConfig = null;
    }
}
