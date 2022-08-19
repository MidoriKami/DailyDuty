using System;
using DailyDuty.Configuration.System.Components;
using Dalamud.Configuration;

namespace DailyDuty.Configuration.System;

[Serializable]
public class SystemConfiguration : IPluginConfiguration
{
    public int Version { get; set; } = 3;

    public SystemSettings System = new();

    public void Save() => Service.PluginInterface.SavePluginConfig(this);

    public static SystemConfiguration Load()
    {
        if (Service.PluginInterface.GetPluginConfig() is SystemConfiguration systemConfiguration)
        {
            return systemConfiguration;
        }
        else
        {
            var newConfiguration = new SystemConfiguration();
            newConfiguration.Save();
            return newConfiguration;
        }
    }
}