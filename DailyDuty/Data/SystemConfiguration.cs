using System;
using Dalamud.Configuration;

namespace DailyDuty.Data
{
    [Serializable]
    public class SystemConfiguration : IPluginConfiguration
    {
        public int Version { get; set; } = 2;

        public bool DeveloperMode = false;
        public bool EchoLogToChat = false;

        public void Save() => Service.PluginInterface.SavePluginConfig(this);
    }
}