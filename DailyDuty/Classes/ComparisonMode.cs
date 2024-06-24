using System.ComponentModel;

namespace DailyDuty.Classes;

public enum ComparisonMode {
    [Description("LessThan")]
    LessThan,
    
    [Description("EqualTo")]
    EqualTo,
    
    [Description("LessThanOrEqual")]
    LessThanOrEqual,
}