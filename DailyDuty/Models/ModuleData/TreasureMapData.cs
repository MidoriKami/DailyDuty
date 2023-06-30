using System;
using DailyDuty.Abstracts;
using KamiLib.AutomaticUserInterface;

namespace DailyDuty.Models.ModuleData;

[Category("ModuleData", 1)]
public interface ITreasureMapData
{
    [LocalDateTimeDisplay("LastMapGathered")]
    public DateTime LastMapGatheredTime { get; set; }
    
    [BoolDisplay("MapAvailable")]
    public bool MapAvailable { get; set; }
}

public class TreasureMapData : IModuleDataBase, ITreasureMapData
{
    public DateTime NextReset { get; set; } = DateTime.MinValue;
    
    public DateTime LastMapGatheredTime { get; set; } = DateTime.MinValue;
    
    public bool MapAvailable { get; set; } = true;
}