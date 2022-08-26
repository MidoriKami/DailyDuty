using System;
using DailyDuty.Configuration.Components;

namespace DailyDuty.Configuration.ModuleSettings;

public class TreasureMapSettings : GenericSettings
{            
    public DateTime LastMapGathered = new();
}