using System;
using System.ComponentModel;
using System.Drawing;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Utility;

namespace DailyDuty.Classes;

public class ColorAttribute(KnownColor internalColor) : Attribute {
    private KnownColor InternalColor { get; } = internalColor;
    public Vector4 Color => InternalColor.Vector();
}

public enum ModuleStatus {
    [Description("Unknown")]
    [Color(KnownColor.Gray)] 
    Unknown,
    
    [Description("Incomplete")]
    [Color(KnownColor.Red)]  
    Incomplete,
    
    [Description("Unavailable")]
    [Color(KnownColor.Orange)]
    Unavailable,
    
    [Description("InProgress")]
    [Color(KnownColor.Aqua)]
    InProgress,
    
    [Description("Complete")]
    [Color(KnownColor.Green)]
    Complete,
    
    [Description("Suppressed")]
    [Color(KnownColor.MediumPurple)]
    Suppressed,
}

public static class ModuleStatusExtensions {
    public static Vector4 GetColor(this ModuleStatus status)
        => status.GetAttribute<ColorAttribute>()!.Color;
}