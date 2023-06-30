using System;
using DailyDuty.Abstracts;
using KamiLib.AutomaticUserInterface;

namespace DailyDuty.Models.ModuleData;

[Category("ModuleData", 1)]
public interface IWondrousTailsModuleData
{
    [IntDisplay("PlacedStickers")]
    public int PlacedStickers { get; set; }

    [UintDisplay("SecondChancePoints")] 
    public uint SecondChance { get; set; }

    [BoolDisplay("NewBookAvailable")]
    public bool NewBookAvailable { get; set; }

    [BoolDisplay("PlayerHasBook")]
    public bool PlayerHasBook { get; set; }
    
    [LocalDateTimeDisplay("Deadline")]
    public DateTime Deadline { get; set; }

    [TimeSpanDisplay("TimeRemaining")]
    public TimeSpan TimeRemaining { get; set; }

    [BoolDisplay("BookExpired")] 
    public bool BookExpired { get; set; }
    
    [BoolDisplay("NearKhloe")]
    public bool CloseToKhloe { get; set; }

    [FloatDisplay("DistanceToKhloe")] 
    public float DistanceToKhloe { get; set; }

    [BoolDisplay("CastingTeleport")] 
    public bool CastingTeleport { get; set; }
}

public class WondrousTailsData : IModuleDataBase, IWondrousTailsModuleData
{
    public DateTime NextReset { get; set; } = DateTime.MinValue;
    
    public int PlacedStickers { get; set; }
    public uint SecondChance { get; set; }
    public bool NewBookAvailable { get; set; }
    public bool PlayerHasBook { get; set; }
    public DateTime Deadline { get; set; }
    public TimeSpan TimeRemaining { get; set; }
    public bool BookExpired { get; set; }
    public bool CloseToKhloe { get; set; }
    public float DistanceToKhloe { get; set; }
    public bool CastingTeleport { get; set; }
}