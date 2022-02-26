using System;

namespace DailyDuty.Data.SettingsObjects.Daily
{
    public class TreasureMapSettings : GenericSettings
    {            
        public DateTime LastMapGathered = new();
        public int MinimumMapLevel = 0;
        public bool NotifyOnAcquisition = false;
        public bool HarvestableMapNotification = false;
    }
}