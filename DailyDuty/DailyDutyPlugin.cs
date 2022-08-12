using DailyDuty.System;
using DailyDuty.Utilities;
using Dalamud.Plugin;
// Daily Duty 3.0

namespace DailyDuty;

public sealed class DailyDutyPlugin : IDalamudPlugin
{
    public string Name => "DailyDuty";

    public DailyDutyPlugin(DalamudPluginInterface pluginInterface)
    {
        Log.Verbose("Inflating Service Class");
        pluginInterface.Create<Service>();

        Log.Verbose("Creating DailyDutyCore");
        Service.System = new DailyDutyCore();
    }

    public void Dispose()
    {
        Service.System?.Dispose();
    }
}