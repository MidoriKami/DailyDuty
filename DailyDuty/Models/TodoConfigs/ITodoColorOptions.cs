using System.Numerics;
using KamiLib.AutomaticUserInterface;

namespace DailyDuty.Models;

[Category("ColorOptions", 6)]
public interface ITodoColorOptions
{
    [ColorConfig("CategoryBackgroundColor", 0.0f, 0.0f, 0.0f, 0.40f)]
    public Vector4 CategoryBackgroundColor { get; set; }
    
    [ColorConfig("HeaderColor", 1.0f, 1.0f, 1.0f, 1.0f)]
    public Vector4 HeaderTextColor { get; set; }

    [ColorConfig("HeaderOutlineColor", 0.5568f, 0.4117f, 0.0470f, 1.0f)]
    public Vector4 HeaderTextOutline { get; set; }
    
    [ColorConfig("ModuleTextColor", 1.0f, 1.0f, 1.0f, 1.0f)]
    public Vector4 ModuleTextColor { get; set; }

    [ColorConfig("ModuleOutlineColor", 0.0392f, 0.4117f, 0.5725f, 1.0f)]
    public Vector4 ModuleOutlineColor { get; set; }
}