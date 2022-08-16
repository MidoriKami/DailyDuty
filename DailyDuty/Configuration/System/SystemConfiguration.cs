using System;
using DailyDuty.Configuration.System.Components;
using Dalamud.Configuration;

namespace DailyDuty.Configuration.System;

[Serializable]
public class SystemConfiguration : IPluginConfiguration
{
    public int Version { get; set; } = 2;

    public bool DeveloperMode = false;

    public SystemSettings System = new();
    public WindowsSettings Windows = new();

    public void Save() => Service.PluginInterface.SavePluginConfig(this);
}