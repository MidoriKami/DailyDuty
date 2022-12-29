using DailyDuty.System;
using DailyDuty.UserInterface.Windows;
using Dalamud.Plugin;
using KamiLib;
using LocalizationManager = DailyDuty.System.LocalizationManager;

namespace DailyDuty;

public sealed class DailyDutyPlugin : IDalamudPlugin
{
    public string Name => "DailyDuty";
    private const string ShorthandCommand = "/dd";

    public DailyDutyPlugin(DalamudPluginInterface pluginInterface)
    {
        pluginInterface.Create<Service>();

        KamiCommon.Initialize(pluginInterface, Name, () => Service.ConfigurationManager.Save());

        Service.Localization = new LocalizationManager();
        
        Service.TeleportManager = new TeleportManager();
        Service.ConfigurationManager = new ConfigurationManager();
        Service.AddonManager = new AddonManager();
        Service.ModuleManager = new ModuleManager();
        Service.ResetManager = new ResetManager();
        Service.ChatManager = new ChatManager();
        
        // Check if we are already logged in
        Service.ConfigurationManager.LoginLogic(Service.Framework);
        
        KamiCommon.CommandManager.AddHandler(ShorthandCommand, "shorthand command to open configuration window");
        
        KamiCommon.WindowManager.AddWindow(new ConfigurationWindow());
        KamiCommon.WindowManager.AddWindow(new StatusWindow());
        KamiCommon.WindowManager.AddWindow(new TodoConfigurationWindow());
        KamiCommon.WindowManager.AddWindow(new TimersConfigurationWindow());
        KamiCommon.WindowManager.AddWindow(new AboutWindow());
    }

    public void Dispose()
    {
        KamiCommon.Dispose();
        
        Service.Localization.Dispose();
        
        Service.ModuleManager.Dispose();
        Service.ConfigurationManager.Dispose();
        Service.ChatManager.Dispose();
        Service.ResetManager.Dispose();
        Service.TeleportManager.Dispose();
        Service.AddonManager.Dispose();
    }
}