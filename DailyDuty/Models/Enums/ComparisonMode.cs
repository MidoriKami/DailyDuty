using KamiLib.AutomaticUserInterface;

namespace DailyDuty.Models.Enums;

public enum ComparisonMode
{
    [EnumLabel("LessThan")]
    LessThan,
    
    [EnumLabel("EqualTo")]
    EqualTo,
    
    [EnumLabel("LessThanOrEqual")]
    LessThanOrEqual
}