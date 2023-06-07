using System.Numerics;
using DailyDuty.Models.Enums;
using KamiLib.AutomaticUserInterface;

namespace DailyDuty.Models;

public class TodoConfig
{
    [BoolConfigOption("Enable", "TodoDisplayEnable", -1)]
    public bool Enable = true;
    
    [BoolConfigOption("PreviewMode", "TodoDisplayEnable", -1)]
    public bool PreviewMode = true;

    [BoolConfigOption("RightAlign", "Positioning", 0)]
    public bool RightAlign = false;
    
    [BoolConfigOption("Dragable", "Positioning", 0)]
    public bool CanDrag = false;

    [EnumConfigOption("AnchorLocation", "Positioning", 0)]
    public WindowAnchor Anchor = WindowAnchor.TopRight;
    
    [PositionConfigOption("Position", "Positioning", 0)]
    public Vector2 Position = new Vector2(1024, 720) / 2.0f;

    [BoolConfigOption("Background", "DisplayOptions", 2)] 
    public bool BackgroundImage = true;

    [BoolConfigOption("EnableDailyTasks", "Categories", 1)]
    public bool DailyTasks = true;
    
    [BoolConfigOption("EnableWeeklyTasks", "Categories", 1)]
    public bool WeeklyTasks = true;
    
    [BoolConfigOption("EnableSpecialTasks", "Categories", 1)]
    public bool SpecialTasks = true;

    [BoolConfigOption("ShowHeaders", "DisplayOptions", 2)]
    public bool ShowHeaders = true;

    [BoolConfigOption("HideInQuestEvent", "DisplayOptions", 2)]
    public bool HideDuringQuests = true;
    
    [BoolConfigOption("HideInDuties", "DisplayOptions", 2)]
    public bool HideInDuties = true;

    [BoolConfigOption("HeaderItalic", "TextStyle", 3)]
    public bool HeaderItalic = false;
    
    [BoolConfigOption("ModuleItalic", "TextStyle", 3)]
    public bool ModuleItalic = false;

    [BoolConfigOption("EnableOutline", "TextStyle", 3)]
    public bool Edge = true;

    [BoolConfigOption("EnableGlowingOutline", "TextStyle", 3)]
    public bool Glare = false;
    
    [ShortStringConfigOption("DailyTasksLabel", "LabelText", 4, true)]
    public string DailyLabel = "Daily Tasks";
    
    [ShortStringConfigOption("WeeklyTasksLabel", "LabelText", 4, true)]
    public string WeeklyLabel = "Weekly Tasks";
    
    [ShortStringConfigOption("SpecialTasksLabel", "LabelText", 4, true)]
    public string SpecialLabel = "Special Tasks";

    [IntConfigOption("FontSize", "DisplayStyle", 5, 5, 48)]
    public int FontSize = 20;

    [IntConfigOption("HeaderSize", "DisplayStyle", 5, 5, 48)] 
    public int HeaderFontSize = 24;

    [IntConfigOption("CategorySpacing", "DisplayStyle", 5, 0, 100)]
    public int CategorySpacing = 12;

    [IntConfigOption("HeaderSpacing", "DisplayStyle", 5, 0, 100)]
    public int HeaderSpacing = 0;
    
    [IntConfigOption("ModuleSpacing", "DisplayStyle", 5, 0, 100)]
    public int ModuleSpacing = 0;
    
    [ColorConfigOption("CategoryBackgroundColor", "ColorOptions", 6, 0.0f, 0.0f, 0.0f, 0.40f)]
    public Vector4 CategoryBackgroundColor = new(0.0f, 0.0f, 0.0f, 0.4f);
    
    [ColorConfigOption("HeaderColor", "ColorOptions", 6, 1.0f, 1.0f, 1.0f, 1.0f)]
    public Vector4 HeaderTextColor = new(1.0f, 1.0f, 1.0f, 1.0f);

    [ColorConfigOption("HeaderOutlineColor", "ColorOptions", 6, 0.5568f, 0.4117f, 0.0470f, 1.0f)]
    public Vector4 HeaderTextOutline = new(0.5568f, 0.4117f, 0.0470f, 1.0f);
    
    [ColorConfigOption("ModuleTextColor", "ColorOptions", 6, 1.0f, 1.0f, 1.0f, 1.0f)]
    public Vector4 ModuleTextColor = new(1.0f, 1.0f, 1.0f, 1.0f);

    [ColorConfigOption("ModuleOutlineColor", "ColorOptions", 6, 0.0392f, 0.4117f, 0.5725f, 1.0f)]
    public Vector4 ModuleOutlineColor = new(0.0392f, 0.4117f, 0.5725f, 1.0f);
}