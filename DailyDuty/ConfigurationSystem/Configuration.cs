using System;
using System.Collections.Generic;
using DailyDuty.Data;
using Dalamud.Configuration;
using Dalamud.Plugin;

namespace DailyDuty.ConfigurationSystem
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 1;

        public DateTime NextDailyReset = new();
        public DateTime NextWeeklyReset = new();
        public int TerritoryUpdateStaggerRate = 1;
        public ToDoWindowSettings ToDoWindowSettings = new();

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

            ToDoWindowSettings ??= new();

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
                settings.EliteHuntSettings.TrackedHunts ??= new TrackedHunt[]
                {
                    new (EliteHuntExpansionEnum.RealmReborn, false),
                    new (EliteHuntExpansionEnum.Heavensward, false),
                    new (EliteHuntExpansionEnum.Stormblood, false),
                    new (EliteHuntExpansionEnum.Shadowbringers, false),
                    new (EliteHuntExpansionEnum.Endwalker, false)
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