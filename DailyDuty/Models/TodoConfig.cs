using System.Numerics;

namespace DailyDuty.Models;

public class TodoConfig {
    // ITodoDisplayEnable
    public bool Enable = true;
    public bool PreviewMode = true;

    // ITodoPositioning
    public bool RightAlign = false;
    public bool CanDrag = false;
    // public WindowAnchor Anchor = WindowAnchor.TopRight;
    public Vector2 Position = new Vector2(1024, 720) / 2.0f;
    
    // ITodoCategories
    public bool DailyTasks = true;
    public bool WeeklyTasks = true;
    public bool SpecialTasks = true;
    
    // ITodoDisplayOptions
    public bool BackgroundImage = true;
    public bool ShowHeaders = true;
    public bool HideDuringQuests = true;
    public bool HideInDuties = true;
    
    // ITodoTextStyle
    public bool HeaderItalic = false;
    public bool ModuleItalic = false;
    public bool Edge = true;
    public bool Glare = false;
    
    // ITodoLabelText
    public string DailyLabel = "Daily Tasks";
    public string WeeklyLabel = "Weekly Tasks";
    public string SpecialLabel = "Special Tasks";
    
    // ITodoDisplayStyle
    public int FontSize = 20;
    public int HeaderFontSize = 24;
    public int CategorySpacing = 12;
    public int HeaderSpacing = 0;
    public int ModuleSpacing = 0;

    // ITodoColorOptions
    public Vector4 CategoryBackgroundColor = new(0.0f, 0.0f, 0.0f, 0.4f);
    public Vector4 HeaderTextColor = new(1.0f, 1.0f, 1.0f, 1.0f);
    public Vector4 HeaderTextOutline = new(0.5568f, 0.4117f, 0.0470f, 1.0f);
    public Vector4 ModuleTextColor = new(1.0f, 1.0f, 1.0f, 1.0f);
    public Vector4 ModuleOutlineColor = new(0.0392f, 0.4117f, 0.5725f, 1.0f);
}