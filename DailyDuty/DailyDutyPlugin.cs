using System.Numerics;
using DailyDuty.Classes;
using DailyDuty.Windows;
using Dalamud.Game.Command;
using Dalamud.Plugin;
using KamiToolKit;

namespace DailyDuty;

public sealed class DailyDutyPlugin : IDalamudPlugin {
    public DailyDutyPlugin(IDalamudPluginInterface pluginInterface) {
        pluginInterface.Create<Services>();

        KamiToolKitLibrary.Initialize(pluginInterface);

        System.ConfigurationWindow = new ModuleBrowserWindow {
            InternalName = "DailyDutyConfig",
            Title = "Daily Duty Configuration",
            Size = new Vector2(700.0f, 600.0f),
        };

        Services.CommandManager.AddHandler("/dd", new CommandInfo(OnCommandReceived) {
            HelpMessage = "Open DailyDuty Config Window",
            ShowInHelp = true,
        });
        
        Services.CommandManager.AddHandler("/dailyduty", new CommandInfo(OnCommandReceived) {
            HelpMessage = "Open DailyDuty Config Window",
            ShowInHelp = true,
        });
        
        System.PayloadController = new PayloadController();
        System.ModuleManager = new ModuleManager();

        if (Services.ClientState.IsLoggedIn) {
            Services.Framework.RunOnFrameworkThread(OnLogin);
        }
        
        Services.ClientState.Login += OnLogin;
        Services.ClientState.Logout += OnLogout;
        
        if (Services.ClientState.IsLoggedIn) {
            System.ConfigurationWindow.DebugOpen();
        }
    }

    private void OnCommandReceived(string command, string arguments) {
        if (command is not ("/dailyduty" or "/dd")) return;

        switch (arguments) {
            case null or "":
                System.ConfigurationWindow.Toggle();
                break;
        }
    }

    public void Dispose() {
        Services.ClientState.Login -= OnLogin;
        Services.ClientState.Logout -= OnLogout;
        
        Services.CommandManager.RemoveHandler("/dd");
        Services.CommandManager.RemoveHandler("/dailyduty");
        
        System.ConfigurationWindow.Dispose();
        
        System.PayloadController.Dispose();
        System.ModuleManager.Dispose();
        
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
