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
            public Weekly.FashionReportSettings FashionReportSettings = new();
        }

        public ulong CurrentCharacter = new();

        public void UpdateCharacter()
        {
            var newCharacterID = Service.ClientState.LocalContentId;

            if (CharacterSettingsMap.ContainsKey(newCharacterID))
            {
#if DEBUG
                PluginLog.Information($"[System] [onLogin] Character Found in Map {newCharacterID}.");
#endif
                CurrentCharacter = newCharacterID;
            }
            else if(newCharacterID != 0)
            {
#if DEBUG
                PluginLog.Information($"[System] [onLogin] Character Not Found in Map {newCharacterID}, Creating new Entry.");
#endif
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
                settings.JumboCactpotSettings ??= new();
                settings.FashionReportSettings ??= new();
            }

            Save();
        }

        public void Save()
        {
            if (CurrentCharacter == 0)
            {
                // Don't Save Null Character
                return;
            }

            pluginInterface!.SavePluginConfig(this);
        }
    }
}