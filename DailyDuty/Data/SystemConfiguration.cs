using System;
using DailyDuty.Data.Components;
using Dalamud.Configuration;

namespace DailyDuty.Data
{
    [Serializable]
    public class SystemConfiguration : IPluginConfiguration
    {
        public int Version { get; set; } = 2;

        public bool DeveloperMode = false;

        public SystemSettings System = new();
        public TimersSettings Timers = new();
        public WindowsSettings Windows = new();
        public AddonSettings Addons = new();

        public void Save() => Service.PluginInterface.SavePluginConfig(this);
    }
}