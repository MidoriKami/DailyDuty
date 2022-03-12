using System;
using System.Collections.Generic;
using DailyDuty.Data.SettingsObjects;
using DailyDuty.Data.SettingsObjects.Timers;
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
        public TimersSettings Timers = new();
        public WindowsSettings Windows = new();
        public AddonSettings Addons = new();
    
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