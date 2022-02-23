using System;
using System.Collections.Generic;
using DailyDuty.Data.SettingsObjects;
using DailyDuty.Data.SettingsObjects.Addons;
using DailyDuty.Data.SettingsObjects.Timers;
using DailyDuty.Data.SettingsObjects.WindowSettings;
using DailyDuty.Utilities;
using DailyDuty.Utilities.Helpers;
using Dalamud.Configuration;
using Dalamud.Plugin;

namespace DailyDuty.Data
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 1;

        public SystemSettings System = new();

        public TimersSettings TimersSettings = new();
        public TodoWindowSettings TodoWindowSettings = new();
        public SettingsWindowSettings SettingsWindowSettings = new();
        public TimersWindowSettings TimersWindowSettings = new();
        public DutyFinderAddonSettings DutyFinderAddonSettings = new();
    
        public Dictionary<ulong, CharacterSettings> CharacterSettingsMap = new();

        public CharacterSettings Current()
        {
            return ConfigurationHelper.GetCurrentCharacterData();
        }

        [NonSerialized]
        private DalamudPluginInterface? pluginInterface;

        private void CheckAndPurgeNullCharacter()
        {
            if (CharacterSettingsMap.ContainsKey(0))
                CharacterSettingsMap.Remove(0);
        }

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;

            CharacterSettingsMap ??= new();

            CheckAndPurgeNullCharacter();
        }

        public void Save()
        {
            if (System.ShowSaveDebugInfo == true)
            {
                Chat.Print($"Save",
                    Service.LoggedIn == true ? 
                        $"Saving {DateTime.Now}" : 
                        "Not logged into a character, skipping save");
            }

            if (Service.LoggedIn == true)
            {
                pluginInterface!.SavePluginConfig(this);
            }
        }
    }
}