using System;
using System.Numerics;
using DailyDuty.Models.Attributes;

namespace DailyDuty.Models;

public class ModuleTodoOptions
{
    [ConfigOption("Enable")]
    public bool Enabled = true;
    
    [ConfigOption("Use Custom Label")]
    public bool UseCustomTodoLabel = false;
    
    [ConfigOption("CustomLabel", true, false)]
    public string CustomTodoLabel = string.Empty;

    [ConfigOption("OverrideTodoListColor")]
    public bool OverrideTextColor = false;
    
    [ConfigOption("Text Color", 1.0f, 1.0f, 1.0f, 1.0f)]
    public Vector4 TextColor = new(1.0f, 1.0f, 1.0f, 1.0f);
    
    [ConfigOption("Text Outline Color", 0.0f, 0.0f, 0.0f, 1.0f)]
    public Vector4 TextOutline = new(0.0f, 0.0f, 0.0f, 1.0f);

    [NonSerialized]
    public bool StyleChanged = true;
}