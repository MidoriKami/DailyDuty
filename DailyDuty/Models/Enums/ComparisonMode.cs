using DailyDuty.Models.Attributes;

namespace DailyDuty.Models.Enums;

public enum ComparisonMode
{
    [Label("LessThan")]
    LessThan,
    
    [Label("EqualTo")]
    EqualTo,
    
    [Label("LessThanOrEqual")]
    LessThanOrEqual
}