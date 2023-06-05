using System.Numerics;
using DailyDuty.Models.Enums;
using KamiLib.AutomaticUserInterface;

namespace DailyDuty.Models;

public class TodoConfig
{
    [DrawCategory("TodoDisplayEnable", -1)]
    [BoolConfigOption("Enable")]
    public bool Enable = true;
    
    [DrawCategory("TodoDisplayEnable", -1)]
    [BoolConfigOption("PreviewMode")]
    public bool PreviewMode = true;

    [DrawCategory("Positioning", 0)]
    [BoolConfigOption("RightAlign")]
    public bool RightAlign = false;
    
    [DrawCategory("Positioning", 0)]
    [BoolConfigOption("Dragable")]
    public bool CanDrag = false;

    [DrawCategory("Positioning", 0)]
    [EnumConfigOption("AnchorLocation")]
    public WindowAnchor Anchor = WindowAnchor.TopRight;
    
    [DrawCategory("Positioning", 0)]
    [PositionConfigOption("Position")]
    public Vector2 Position = new Vector2(1024, 720) / 2.0f;

    [DrawCategory("DisplayOptions", 2)]
    [BoolConfigOption("Background")] 
    public bool BackgroundImage = true;

    [DrawCategory("Categories", 1)]
    [BoolConfigOption("EnableDailyTasks")]
    public bool DailyTasks = true;
    
    [DrawCategory("Categories", 1)]
    [BoolConfigOption("EnableWeeklyTasks")]
    public bool WeeklyTasks = true;
    
    [DrawCategory("Categories", 1)]
    [BoolConfigOption("EnableSpecialTasks")]
    public bool SpecialTasks = true;

    [DrawCategory("DisplayOptions", 2)]
    [BoolConfigOption("ShowHeaders")]
    public bool ShowHeaders = true;

    [DrawCategory("DisplayOptions", 2)]
    [BoolConfigOption("HideInQuestEvent")]
    public bool HideDuringQuests = true;
    
    [DrawCategory("DisplayOptions", 2)]
    [BoolConfigOption("HideInDuties")]
    public bool HideInDuties = true;

    [DrawCategory("TextStyle", 3)]
    [BoolConfigOption("HeaderItalic")]
    public bool HeaderItalic = false;
    
    [DrawCategory("TextStyle", 3)]
    [BoolConfigOption("ModuleItalic")]
    public bool ModuleItalic = false;

    [DrawCategory("TextStyle", 3)]
    [BoolConfigOption("EnableOutline")]
    public bool Edge = true;

    [DrawCategory("TextStyle", 3)]
    [BoolConfigOption("EnableGlowingOutline")]
    public bool Glare = false;
    
    [DrawCategory("LabelText", 4)]
    [ShortStringConfigOption("DailyTasksLabel", true)]
    public string DailyLabel = "Daily Tasks";
    
    [DrawCategory("LabelText", 4)]
    [ShortStringConfigOption("WeeklyTasksLabel", true)]
    public string WeeklyLabel = "Weekly Tasks";
    
    [DrawCategory("LabelText", 4)]
    [ShortStringConfigOption("SpecialTasksLabel", true)]
    public string SpecialLabel = "Special Tasks";

    [DrawCategory("DisplayStyle", 5)]
    [IntConfigOption("FontSize", 5, 48)]
    public int FontSize = 20;

    [DrawCategory("DisplayStyle", 5)]
    [IntConfigOption("HeaderSize", 5, 48)] 
    public int HeaderFontSize = 24;

    [DrawCategory("DisplayStyle", 5)]
    [IntConfigOption("CategorySpacing", 0, 100)]
    public int CategorySpacing = 12;

    [DrawCategory("DisplayStyle", 5)]
    [IntConfigOption("HeaderSpacing", 0, 100)]
    public int HeaderSpacing = 0;
    
    [DrawCategory("DisplayStyle", 5)]
    [IntConfigOption("ModuleSpacing", 0, 100)]
    public int ModuleSpacing = 0;
    
    [DrawCategory("ColorOptions", 6)]
    [ColorConfigOption("CategoryBackgroundColor", 0.0f, 0.0f, 0.0f, 0.40f)]
    public Vector4 CategoryBackgroundColor = new(0.0f, 0.0f, 0.0f, 0.4f);
    
    [DrawCategory("ColorOptions", 6)]
    [ColorConfigOption("HeaderColor", 1.0f, 1.0f, 1.0f, 1.0f)]
    public Vector4 HeaderTextColor = new(1.0f, 1.0f, 1.0f, 1.0f);

    [DrawCategory("ColorOptions", 6)]
    [ColorConfigOption("HeaderOutlineColor", 0.5568f, 0.4117f, 0.0470f, 1.0f)]
    public Vector4 HeaderTextOutline = new(0.5568f, 0.4117f, 0.0470f, 1.0f);
    
    [DrawCategory("ColorOptions", 6)]
    [ColorConfigOption("ModuleTextColor", 1.0f, 1.0f, 1.0f, 1.0f)]
    public Vector4 ModuleTextColor = new(1.0f, 1.0f, 1.0f, 1.0f);

    [DrawCategory("ColorOptions", 6)]
    [ColorConfigOption("ModuleOutlineColor", 0.0392f, 0.4117f, 0.5725f, 1.0f)]
    public Vector4 ModuleOutlineColor = new(0.0392f, 0.4117f, 0.5725f, 1.0f);
}