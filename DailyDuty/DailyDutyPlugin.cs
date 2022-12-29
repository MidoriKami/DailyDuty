using DailyDuty.System;
using Dalamud.Plugin;

namespace DailyDuty;

public sealed class DailyDutyPlugin : IDalamudPlugin
{
    public string Name => "DailyDuty";

    public DailyDutyPlugin(DalamudPluginInterface pluginInterface)
    {
        pluginInterface.Create<Service>();

        Service.TeleportManager = new TeleportManager();
        Service.ConfigurationManager = new ConfigurationManager();
        Service.PayloadManager = new ChatPayloadManager();
        Service.AddonManager = new AddonManager();
        Service.ModuleManager = new ModuleManager();
        Service.ResetManager = new ResetManager();
        Service.ChatManager = new ChatManager();
        Service.WindowManager = new WindowManager();
    }

    public void Dispose()
    {
        Service.WindowManager.Dispose();
        Service.ModuleManager.Dispose();
        Service.ConfigurationManager.Dispose();
        Service.ChatManager.Dispose();
        Service.ResetManager.Dispose();
        Service.TeleportManager.Dispose();
        Service.PayloadManager.Dispose();
        Service.AddonManager.Dispose();
    }
}