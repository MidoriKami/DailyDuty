using System.Drawing;
using DailyDuty.Models.Attributes;

namespace DailyDuty.Models.Enums;

public enum ModuleStatus
{
    [Label("Unknown")]
    [DisplayColor(KnownColor.Gray)] 
    Unknown,
    
    [Label("Incomplete")]
    [DisplayColor(KnownColor.Red)]  
    Incomplete,
    
    [Label("Unavailable")]
    [DisplayColor(KnownColor.Orange)]
    Unavailable,
    
    [Label("InProgress")]
    [DisplayColor(KnownColor.Aqua)]
    InProgress,
    
    [Label("Complete")]
    [DisplayColor(KnownColor.Green)]
    Complete,
    
    [Label("Suppressed")]
    [DisplayColor(KnownColor.MediumPurple)]
    Suppressed
}
