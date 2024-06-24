using System.ComponentModel;

namespace DailyDuty.Classes;

public enum FashionReportMode {
    [Description("All")]
    All,
    
    [Description("Single")]
    Single,
    
    [Description("Plus80")]
    Plus80,
}