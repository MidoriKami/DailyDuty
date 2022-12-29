using DailyDuty.System;
using DailyDuty.UserInterface.Windows;
using Dalamud.Plugin;
using KamiLib;

namespace DailyDuty;

public sealed class DailyDutyPlugin : IDalamudPlugin
{
    public string Name => "DailyDuty";
    private const string ShorthandCommand = "/dd";

    public DailyDutyPlugin(DalamudPluginInterface pluginInterface)
    {
        pluginInterface.Create<Service>();

        KamiCommon.Initialize(pluginInterface, Name, () => Service.ConfigurationManager.Save());
        
        Service.TeleportManager = new TeleportManager();
        Service.ConfigurationManager = new ConfigurationManager();
        Service.PayloadManager = new ChatPayloadManager();
        Service.AddonManager = new AddonManager();
        Service.ModuleManager = new ModuleManager();
        Service.ResetManager = new ResetManager();
        Service.ChatManager = new ChatManager();
        
        KamiCommon.CommandManager.AddHandler(ShorthandCommand, "shorthand command to open configuration window");
        
        KamiCommon.WindowManager.AddWindow(new ConfigurationWindow());
        KamiCommon.WindowManager.AddWindow(new StatusWindow());
        KamiCommon.WindowManager.AddWindow(new TodoConfigurationWindow());
        KamiCommon.WindowManager.AddWindow(new TimersConfigurationWindow());
        KamiCommon.WindowManager.AddWindow(new AboutWindow());
    }

    public void Dispose()
    {
        Service.ModuleManager.Dispose();
        Service.ConfigurationManager.Dispose();
        Service.ChatManager.Dispose();
        Service.ResetManager.Dispose();
        Service.TeleportManager.Dispose();
        Service.PayloadManager.Dispose();
        Service.AddonManager.Dispose();
    }
}