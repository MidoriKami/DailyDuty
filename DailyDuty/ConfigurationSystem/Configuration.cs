using System;
using System.Collections.Generic;
using DailyDuty.System.Utilities;
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
            public Weekly.WondrousTailsSettings WondrousTailsSettings = new();
            public Weekly.CustomDeliveriesSettings CustomDeliveriesSettings = new();
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
            foreach (var (charID, settings) in CharacterSettingsMap)
            {
                settings.TreasureMapSettings ??= new();

                settings.WondrousTailsSettings ??= new();

                settings.CustomDeliveriesSettings ??= new();
                settings.CustomDeliveriesSettings.DeliveryNPC ??= new();
            }

            Save();
        }

        public void Save()
        {
            pluginInterface!.SavePluginConfig(this);
        }
    }
}