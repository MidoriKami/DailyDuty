using System.Collections.Generic;
using DailyDuty.DataModels;
using DailyDuty.System;
using DailyDuty.UserInterface.Windows;
using Dalamud.Plugin;
using KamiLib;
using KamiLib.Teleporter;

namespace DailyDuty;

public sealed class DailyDutyPlugin : IDalamudPlugin
{
    public string Name => "DailyDuty";
    private const string ShorthandCommand = "/dd";

    public DailyDutyPlugin(DalamudPluginInterface pluginInterface)
    {
        pluginInterface.Create<Service>();

        KamiCommon.Initialize(pluginInterface, Name, () => Service.ConfigurationManager.Save());

        TeleportManager.Instance.AddTeleports(new List<TeleportInfo>
        {
            new(1, TeleportLocation.GoldSaucer, 62),
            new(2, TeleportLocation.Idyllshire, 75),
            new(3, TeleportLocation.DomanEnclave, 127),
            new(4, TeleportLocation.UlDah, 9),
        });
        
        LocalizationManager.Instance.Initialize();
        
        Service.ConfigurationManager = new ConfigurationManager();
        Service.ModuleManager = new ModuleManager();
        Service.ResetManager = new ResetManager();
        Service.ChatManager = new ChatManager();
        
        Service.ConfigurationManager.TryLogin();
        
        KamiCommon.CommandManager.AddHandler(ShorthandCommand, "shorthand command to open configuration window");
        
        KamiCommon.WindowManager.AddConfigurationWindow(new ConfigurationWindow());
        KamiCommon.WindowManager.AddWindow(new StatusWindow());
        KamiCommon.WindowManager.AddWindow(new OverlayConfigurationWindow());
        KamiCommon.WindowManager.AddWindow(new TimerStyleConfigurationWindow());
        KamiCommon.WindowManager.AddWindow(new HuntMarkDebugWindow());
    }

    public void Dispose()
    {
        KamiCommon.Dispose();
        
        LocalizationManager.Cleanup();
        
        Service.ModuleManager.Dispose();
        Service.ConfigurationManager.Dispose();
        Service.ChatManager.Dispose();
        Service.ResetManager.Dispose();
        
        AddonManager.Cleanup();
    }
}