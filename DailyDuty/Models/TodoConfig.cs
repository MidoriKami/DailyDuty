using System.Numerics;
using DailyDuty.Models.Enums;

namespace DailyDuty.Models;

public class TodoConfig : 
    ITodoDisplayEnable, ITodoPositioning, ITodoCategories, ITodoDisplayOptions, ITodoTextStyle, ITodoLabelText, ITodoDisplayStyle, ITodoColorOptions
{
    // ITodoDisplayEnable
    public bool Enable { get; set; } = true;
    public bool PreviewMode { get; set; } = true;

    // ITodoPositioning
    public bool RightAlign { get; set; } = false;
    public bool CanDrag { get; set; } = false;
    public WindowAnchor Anchor { get; set; } = WindowAnchor.TopRight;
    public Vector2 Position { get; set; } = new Vector2(1024, 720) / 2.0f;
    
    // ITodoCategories
    public bool DailyTasks { get; set; } = true;
    public bool WeeklyTasks { get; set; } = true;
    public bool SpecialTasks { get; set; } = true;
    
    // ITodoDisplayOptions
    public bool BackgroundImage { get; set; } = true;
    public bool ShowHeaders { get; set; } = true;
    public bool HideDuringQuests { get; set; } = true;
    public bool HideInDuties { get; set; } = true;
    
    // ITodoTextStyle
    public bool HeaderItalic { get; set; } = false;
    public bool ModuleItalic { get; set; } = false;
    public bool Edge { get; set; } = true;
    public bool Glare { get; set; } = false;
    
    // ITodoLabelText
    public string DailyLabel { get; set; } = "Daily Tasks";
    public string WeeklyLabel { get; set; } = "Weekly Tasks";
    public string SpecialLabel { get; set; } = "Special Tasks";
    
    // ITodoDisplayStyle
    public int FontSize { get; set; } = 20;
    public int HeaderFontSize { get; set; } = 24;
    public int CategorySpacing { get; set; } = 12;
    public int HeaderSpacing { get; set; } = 0;
    public int ModuleSpacing { get; set; } = 0;

    // ITodoColorOptions
    public Vector4 CategoryBackgroundColor { get; set; } = new(0.0f, 0.0f, 0.0f, 0.4f);
    public Vector4 HeaderTextColor { get; set; } = new(1.0f, 1.0f, 1.0f, 1.0f);
    public Vector4 HeaderTextOutline { get; set; } = new(0.5568f, 0.4117f, 0.0470f, 1.0f);
    public Vector4 ModuleTextColor { get; set; } = new(1.0f, 1.0f, 1.0f, 1.0f);
    public Vector4 ModuleOutlineColor { get; set; } = new(0.0392f, 0.4117f, 0.5725f, 1.0f);
}