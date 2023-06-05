using System;
using System.Numerics;
using KamiLib.AutomaticUserInterface;

namespace DailyDuty.Models;

public class ModuleTodoOptions
{
    [DrawCategory("TodoConfiguration", 1)]
    [BoolConfigOption("Enable")]
    public bool Enabled = true;
    
    [DrawCategory("TodoConfiguration", 1)]
    [BoolConfigOption("UseCustomLabel")]
    public bool UseCustomTodoLabel = false;
    
    [DrawCategory("TodoConfiguration", 1)]
    [StringConfigOption("UseCustomLabel", true)]
    public string CustomTodoLabel = string.Empty;

    [DrawCategory("TodoConfiguration", 1)]
    [BoolConfigOption("OverrideTodoListColor")]
    public bool OverrideTextColor = false;
    
    [DrawCategory("TodoConfiguration", 1)]
    [ColorConfigOption("TextColor", 1.0f, 1.0f, 1.0f, 1.0f)]
    public Vector4 TextColor = new(1.0f, 1.0f, 1.0f, 1.0f);
    
    [DrawCategory("TodoConfiguration", 1)]
    [ColorConfigOption("TextOutlineColor", 0.0f, 0.0f, 0.0f, 1.0f)]
    public Vector4 TextOutline = new(0.0f, 0.0f, 0.0f, 1.0f);

    [NonSerialized]
    public bool StyleChanged = true;
}