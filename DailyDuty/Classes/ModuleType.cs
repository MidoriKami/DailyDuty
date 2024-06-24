using System.ComponentModel;

namespace DailyDuty.Classes;

public enum ModuleType {
    [Description("Daily")]
    Daily,
    
    [Description("Weekly")]
    Weekly,
    
    [Description("Special")]
    Special,
}