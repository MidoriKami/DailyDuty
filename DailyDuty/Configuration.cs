using System;
using Dalamud.Configuration;
using Dalamud.Plugin;

namespace DailyDuty
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 1;

        public class GenericSettings
        {
            public bool Enabled = false;
        }

        public class DailyTreasureMapSettings : GenericSettings
        {
            public DateTime LastMapGathered = new ();
            public int MinimumMapLevel = 0;
            public bool NotificationEnabled = false;
        }

        public DailyTreasureMapSettings TreasureMapSettings = new();
        

        [NonSerialized]
        private DalamudPluginInterface? pluginInterface;

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;

            TreasureMapSettings ??= new();

            Save();
        }

        public void Save()
        {
            pluginInterface!.SavePluginConfig(this);
        }
    }
}