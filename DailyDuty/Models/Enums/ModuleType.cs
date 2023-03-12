using DailyDuty.Models.Attributes;

namespace DailyDuty.Models.Enums;

public enum ModuleType
{
    [Label("Daily")]
    Daily,
    
    [Label("Weekly")]
    Weekly,
    
    [Label("Special")]
    Special
}