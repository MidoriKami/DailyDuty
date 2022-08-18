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

        Service.ConfigurationManager = new ConfigurationManager();
        Service.LocalizationManager = new LocalizationManager();
        Service.ModuleManager = new ModuleManager();
        Service.WindowManager = new WindowManager();
        Service.CommandSystem = new CommandManager();
    }

    public void Dispose()
    {
        Service.CommandSystem.Dispose();
        Service.WindowManager.Dispose();
        Service.ModuleManager.Dispose();
        Service.LocalizationManager.Dispose();
        Service.ConfigurationManager.Dispose();
    }
}