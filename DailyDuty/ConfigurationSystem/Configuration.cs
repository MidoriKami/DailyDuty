using System;
using System.Collections.Generic;
using Dalamud.Configuration;
using Dalamud.Plugin;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.ConfigurationSystem
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 1;
        
        public Daily.TreasureMapSettings TreasureMapSettings = new();
        public Weekly.WondrousTailsSettings WondrousTailsSettings = new();
        public Weekly.CustomDeliveriesSettings CustomDeliveriesSettings = new();

        [NonSerialized]
        private DalamudPluginInterface? pluginInterface;

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;

            TreasureMapSettings ??= new();
            WondrousTailsSettings ??= new();
            WondrousTailsSettings.Data ??= new (ButtonState, List<uint>)[16];
            CustomDeliveriesSettings ??= new();

            Save();
        }

        public void Save()
        {
            pluginInterface!.SavePluginConfig(this);
        }
    }
}