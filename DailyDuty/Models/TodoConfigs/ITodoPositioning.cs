using System.Numerics;
using DailyDuty.Models.Enums;
using KamiLib.AutomaticUserInterface;

namespace DailyDuty.Models;

[Category("Positioning")]
public interface ITodoPositioning
{
    [BoolConfig("RightAlign")]
    public bool RightAlign { get; set; }
    
    [BoolConfig("Dragable")]
    public bool CanDrag { get; set; }

    [EnumConfig("AnchorLocation")]
    public WindowAnchor Anchor { get; set; }
    
    [Vector2Config("Position")]
    public Vector2 Position { get; set; }
}