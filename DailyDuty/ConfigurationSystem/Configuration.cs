using System;
using System.Collections.Generic;
using DailyDuty.Data;
using Dalamud.Configuration;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Logging;
using Dalamud.Plugin;
using Newtonsoft.Json;

namespace DailyDuty.ConfigurationSystem
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 1;

        public DateTime NextDailyReset = new();
        public DateTime NextWeeklyReset = new();
        public int TerritoryUpdateStaggerRate = 1;

        public Dictionary<ulong, CharacterSettings> CharacterSettingsMap = new();
        public ulong CurrentCharacter = new();

        [Serializable]
        public class CharacterSettings
        {
            public Daily.TreasureMapSettings TreasureMapSettings = new();
            public Daily.Cactpot MiniCactpotSettings = new();

            public Weekly.WondrousTailsSettings WondrousTailsSettings = new();
            public Weekly.CustomDeliveriesSettings CustomDeliveriesSettings = new();
            public Weekly.JumboCactpotSettings JumboCactpotSettings = new();
            public Weekly.FashionReportSettings FashionReportSettings = new();
            public Weekly.EliteHuntSettings EliteHuntSettings = new();
        }

        public void UpdateCharacter()
        {
            var newCharacterID = Service.ClientState.LocalContentId;

            if (CharacterSettingsMap.ContainsKey(newCharacterID))
            {
                CurrentCharacter = newCharacterID;
            }
            else
            {
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
                settings.EliteHuntSettings ??= new();
                settings.EliteHuntSettings.EliteHunts ??= new (EliteHuntEnum, bool, bool)[5]
                {
                    new(EliteHuntEnum.RealmReborn, false, false),
                    new(EliteHuntEnum.Heavensward, false, false),
                    new(EliteHuntEnum.Stormblood, false, false),
                    new(EliteHuntEnum.Shadowbringers, false, false),
                    new(EliteHuntEnum.Endwalker, false, false)
                };
            }

            if (!CharacterSettingsMap.ContainsKey(0))
            {
                CharacterSettingsMap.Add(0, new());
            }

            Save();
        }

        public void Save()
        {

            pluginInterface!.SavePluginConfig(this);
        }
    }
}