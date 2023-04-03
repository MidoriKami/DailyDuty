using DailyDuty.Models.Attributes;

namespace DailyDuty.Models.Enums;

public enum FashionReportMode
{
    [Label("All")]
    All,
    
    [Label("Single")]
    Single,
    
    [Label("Plus80")]
    Plus80,
}