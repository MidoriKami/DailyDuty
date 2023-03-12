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
    
    [Label("Complete")]
    [DisplayColor(KnownColor.Green)]
    Complete,
    
    [Label("Suppressed")]
    [DisplayColor(KnownColor.Purple)]
    Suppressed
}
