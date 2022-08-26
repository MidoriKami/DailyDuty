using DailyDuty.System;
using DailyDuty.Utilities;
using Dalamud.Plugin;

namespace DailyDuty;

public sealed class DailyDutyPlugin : IDalamudPlugin
{
    public string Name => "DailyDuty";

    public DailyDutyPlugin(DalamudPluginInterface pluginInterface)
    {
        Log.Verbose("Inflating Service Class");
        pluginInterface.Create<Service>();

        Service.EventManager = new DutyEventManager();
        Service.TeleportManager = new TeleportManager();
        Service.ConfigurationManager = new ConfigurationManager();
        Service.LocalizationManager = new LocalizationManager();
        Service.PayloadManager = new ChatPayloadManager();
        Service.ModuleManager = new ModuleManager();
        Service.ChatManager = new ChatManager();
        Service.WindowManager = new WindowManager();
        Service.CommandSystem = new CommandManager();
        Service.ResetManager = new ResetManager();
    }

    public void Dispose()
    {
        Service.CommandSystem.Dispose();
        Service.WindowManager.Dispose();
        Service.ModuleManager.Dispose();
        Service.LocalizationManager.Dispose();
        Service.ConfigurationManager.Dispose();
        Service.ChatManager.Dispose();
        Service.ResetManager.Dispose();
        Service.TeleportManager.Dispose();
        Service.PayloadManager.Dispose();
        Service.EventManager.Dispose();
    }
}