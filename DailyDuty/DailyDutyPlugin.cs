using DailyDuty.System;
using DailyDuty.System.Localization;
using DailyDuty.Views;
using Dalamud.Plugin;
using KamiLib;
using KamiLib.Commands;

namespace DailyDuty;

public sealed class DailyDutyPlugin : IDalamudPlugin
{
    public static DailyDutySystem System = null!;
    
    public DailyDutyPlugin(DalamudPluginInterface pluginInterface)
    {
        pluginInterface.Create<Service>();

        KamiCommon.Initialize(pluginInterface, "DailyDuty");
        KamiCommon.RegisterLocalizationHandler(key => Strings.ResourceManager.GetString(key, Strings.Culture));
                
        System = new DailyDutySystem();
        
        CommandController.RegisterMainCommand("/dd", "/dailyduty");
        
        KamiCommon.WindowManager.AddConfigurationWindow(new ConfigurationWindow());
    }

    public void Dispose()
    {
        KamiCommon.Dispose();
        
        System.Dispose();
    }
}