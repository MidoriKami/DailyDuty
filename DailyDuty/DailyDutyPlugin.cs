using DailyDuty.Models;
using DailyDuty.Models.Enums;
using DailyDuty.System;
using Dalamud.Plugin;
using KamiLib;

namespace DailyDuty;

public sealed class DailyDutyPlugin : IDalamudPlugin
{
    public string Name => "DailyDuty";

    public static DailyDutySystem System = null!;
    
    public DailyDutyPlugin(DalamudPluginInterface pluginInterface)
    {
        pluginInterface.Create<Service>();
        
        KamiCommon.Initialize(pluginInterface, Name, () => {});

        System = new DailyDutySystem();
    }

    public void Dispose()
    {
        KamiCommon.Dispose();
        
        System.Dispose();
    }
}