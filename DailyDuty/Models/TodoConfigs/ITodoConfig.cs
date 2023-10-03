using System.Numerics;
using KamiLib.AutomaticUserInterface;

namespace DailyDuty.Models;

[Category("TodoConfiguration", 11)]
public interface ITodoConfig
{
    [BoolConfig("Enable")]
    public bool TodoEnabled { get; set; }
    
    [BoolConfig("UseCustomLabel")]
    public bool UseCustomTodoLabel { get; set; }
    
    [StringConfig("UseCustomLabel")]
    public string CustomTodoLabel { get; set; }

    [BoolConfig("OverrideTodoListColor")]
    public bool OverrideTextColor { get; set; }
    
    [ColorConfig("TextColor", 1.0f, 1.0f, 1.0f, 1.0f)]
    public Vector4 TodoTextColor { get; set; }
    
    [ColorConfig("TextOutlineColor", 0.0f, 0.0f, 0.0f, 1.0f)]
    public Vector4 TodoTextOutline { get; set; }

    public bool StyleChanged { get; set; }
}