using System;
using System.Collections.Generic;
using Dalamud.Configuration;
using Dalamud.Logging;
using Dalamud.Plugin;

namespace DailyDuty.ConfigurationSystem
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 1;

        public DateTime NextDailyReset = new();
        public DateTime NextWeeklyReset = new();
        public Dictionary<ulong, CharacterSettings> CharacterSettingsMap = new();

        [Serializable]
        public class CharacterSettings
        {
            public Daily.TreasureMapSettings TreasureMapSettings = new();
            public Daily.Cactpot MiniCactpotSettings = new();

            public Weekly.WondrousTailsSettings WondrousTailsSettings = new();
            public Weekly.CustomDeliveriesSettings CustomDeliveriesSettings = new();
            public Weekly.JumboCactpotSettings JumboCactpotSettings = new();
        }

        public ulong CurrentCharacter = new();

        public void UpdateCharacter()
        {
            var newCharacterID = Service.ClientState.LocalContentId;

            if (CharacterSettingsMap.ContainsKey(newCharacterID))
            {
                PluginLog.Information($"[System] [onLogin] Character Found in Map {newCharacterID}.");
                CurrentCharacter = newCharacterID;
            }
            else
            {
                PluginLog.Information($"[System] [onLogin] Character Not Found in Map {newCharacterID}, Creating new Entry.");
                CharacterSettingsMap.Add(newCharacterID, new CharacterSettings());
                CurrentCharacter = newCharacterID;
            }

            Save();
        }

        [NonSerialized]
        private DalamudPluginInterface? pluginInterface;

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;

            CharacterSettingsMap ??= new();
            foreach (var (_, settings) in CharacterSettingsMap)
            {
                // Daily
                settings.TreasureMapSettings ??= new();
                settings.MiniCactpotSettings ??= new();

                // Weekly
                settings.WondrousTailsSettings ??= new();
                settings.CustomDeliveriesSettings ??= new();
                settings.CustomDeliveriesSettings.DeliveryNPC ??= new();
                settings.JumboCactpotSettings ??= new();
            }

            Save();
        }

        public void Save()
        {
            pluginInterface!.SavePluginConfig(this);
        }
    }
}