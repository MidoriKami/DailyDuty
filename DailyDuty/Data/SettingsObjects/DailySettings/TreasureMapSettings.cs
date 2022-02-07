using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyDuty.Data.SettingsObjects.DailySettings;

public class TreasureMapSettings : GenericSettings
{            
    public DateTime LastMapGathered = new();
    public int MinimumMapLevel = 0;
    public bool NotifyOnAcquisition = false;
    public bool HarvestableMapNotification = false;
}