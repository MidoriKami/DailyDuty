using System;
using Dalamud.Configuration;
using Dalamud.Plugin;

namespace DailyDuty
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 1;

        public class DailyTreasureMapSettings
        {
            public DateTime LastMapGathered = new DateTime();
            public bool Enabled = false;
            public int MinimumMapLevel = 0;
            public bool NotificationEnabled = false;
        }

        public DailyTreasureMapSettings TreasureMapSettings = new DailyTreasureMapSettings();
        

        [NonSerialized]
        private DalamudPluginInterface? pluginInterface;

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;

            TreasureMapSettings ??= new DailyTreasureMapSettings();

            Save();
        }

        public void Save()
        {
            pluginInterface!.SavePluginConfig(this);
        }
    }
}