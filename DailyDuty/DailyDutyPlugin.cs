using DailyDuty.System;
using DailyDuty.System.Localization;
using DailyDuty.Views;
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

        KamiCommon.Initialize(pluginInterface, Name);
        KamiCommon.RegisterLocalizationHandler(key => Strings.ResourceManager.GetString(key, Strings.Culture));
                
        System = new DailyDutySystem();
        
        KamiCommon.CommandManager.AddHandler("/dd", "Shorthand Command Handler");
        KamiCommon.WindowManager.AddConfigurationWindow(new ConfigurationWindow());
    }

    public void Dispose()
    {
        KamiCommon.Dispose();
        
        System.Dispose();
    }
}