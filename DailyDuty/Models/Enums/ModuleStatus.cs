using System.Drawing;
using KamiLib.AutomaticUserInterface;

namespace DailyDuty.Models.Enums;

public enum ModuleStatus
{
    [EnumLabel("Unknown")]
    [DisplayColor(KnownColor.Gray)] 
    Unknown,
    
    [EnumLabel("Incomplete")]
    [DisplayColor(KnownColor.Red)]  
    Incomplete,
    
    [EnumLabel("Unavailable")]
    [DisplayColor(KnownColor.Orange)]
    Unavailable,
    
    [EnumLabel("InProgress")]
    [DisplayColor(KnownColor.Aqua)]
    InProgress,
    
    [EnumLabel("Complete")]
    [DisplayColor(KnownColor.Green)]
    Complete,
    
    [EnumLabel("Suppressed")]
    [DisplayColor(KnownColor.MediumPurple)]
    Suppressed
}
