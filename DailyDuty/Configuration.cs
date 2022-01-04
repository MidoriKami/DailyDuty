using Dalamud.Configuration;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;

namespace PartnerUp
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 1;

        public bool DiableInAllianceRaid = true;
        public int TerritoryChangeDelayTime = 8000;

        public bool AllEnabled = false;
        public bool EnableDancePartnerBanner = false;
        public bool EnableFaerieBanner = false;
        public bool EnableKardionBanner = false;

        public bool ForceShowDancePartnerBanner = false;
        public bool ForceShowFaerieBanner = false;
        public bool ForceShowKardionBanner = false;

        public bool RepositionModeDancePartnerBanner = false;
        public bool RepositionModeFaerieBanner = false;
        public bool RepositionModeKardionBanner = false;

        public List<int> TerritoryBlacklist = new();

        public bool ForceWindowUpdate = false;


        [NonSerialized]
        private DalamudPluginInterface? pluginInterface;

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;
        }

        public void Save()
        {
            pluginInterface!.SavePluginConfig(this);
        }
    }
}