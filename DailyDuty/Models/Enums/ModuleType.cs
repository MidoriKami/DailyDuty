using KamiLib.AutomaticUserInterface;

namespace DailyDuty.Models.Enums;

public enum ModuleType
{
    [EnumLabel("Daily")]
    Daily,
    
    [EnumLabel("Weekly")]
    Weekly,
    
    [EnumLabel("Special")]
    Special
}