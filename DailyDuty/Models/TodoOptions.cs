using System;
using System.Numerics;
using KamiLib.AutomaticUserInterface;

namespace DailyDuty.Models;

public class ModuleTodoOptions
{
    [BoolConfigOption("Enable", "TodoConfiguration", 1)]
    public bool Enabled = true;
    
    [BoolConfigOption("UseCustomLabel", "TodoConfiguration", 1)]
    public bool UseCustomTodoLabel = false;
    
    [StringConfigOption("UseCustomLabel", "TodoConfiguration", 1, true)]
    public string CustomTodoLabel = string.Empty;

    [BoolConfigOption("OverrideTodoListColor", "TodoConfiguration", 1)]
    public bool OverrideTextColor = false;
    
    [ColorConfigOption("TextColor", "TodoConfiguration", 1, 1.0f, 1.0f, 1.0f, 1.0f)]
    public Vector4 TextColor = new(1.0f, 1.0f, 1.0f, 1.0f);
    
    [ColorConfigOption("TextOutlineColor", "TodoConfiguration", 1, 0.0f, 0.0f, 0.0f, 1.0f)]
    public Vector4 TextOutline = new(0.0f, 0.0f, 0.0f, 1.0f);

    [NonSerialized]
    public bool StyleChanged = true;
}