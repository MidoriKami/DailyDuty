using System.Drawing;
using System.Numerics;
using DailyDuty.Modules;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.Configuration;
using KamiToolKit.Nodes;
using KamiToolKit.Nodes.NodeStyles;

namespace DailyDuty.Models;

public class CategoryConfig {
    public ListNodeStyle ListNodeStyle = new() {
        LayoutAnchor = LayoutAnchor.TopLeft,
        LayoutOrientation = LayoutOrientation.Vertical,
        BaseDisable = BaseStyleDisable.Visible | BaseStyleDisable.Position | BaseStyleDisable.Size | BaseStyleDisable.NodeFlags | BaseStyleDisable.Color | BaseStyleDisable.Margin,
        ListStyleDisable = ListStyleDisable.ClipContents,
    };

    public TextNodeStyle HeaderStyle = new() {
        TextFlags = TextFlags.Edge | TextFlags.AutoAdjustNodeSize,
        FontSize = 24,
        Margin = new Vector4(5.0f),
        TextOutlineColor = new Vector4(142, 106, 12, 255) / 255,
        BaseDisable = BaseStyleDisable.Position | BaseStyleDisable.Size | BaseStyleDisable.NodeFlags | BaseStyleDisable.Color,
        TextStyleDisable = TextStyleDisable.TextFlags2 | TextStyleDisable.LineSpacing | TextStyleDisable.AlignmentType | TextStyleDisable.BackgroundColor,
    };

    public TextNodeStyle ModuleStyle = new() {
        FontSize = 12,
        Margin = new Vector4(1.0f),  
        TextOutlineColor = new Vector4(10, 105, 146, 255) / 255,
        FontType = FontType.Axis,
        TextFlags = TextFlags.AutoAdjustNodeSize | TextFlags.Edge,
        BaseDisable = BaseStyleDisable.Visible | BaseStyleDisable.Position | BaseStyleDisable.Size | BaseStyleDisable.NodeFlags | BaseStyleDisable.Color,
        TextStyleDisable = TextStyleDisable.TextFlags2 | TextStyleDisable.LineSpacing | TextStyleDisable.AlignmentType | TextStyleDisable.BackgroundColor,
    };

    public ModuleType ModuleType;
    public bool Enabled = true;
    
    public string HeaderLabel = "ERROR Initializing Data";
    
    public bool UseCustomLabel = false;
    public string CustomLabel = string.Empty;
}

public class TodoConfig {
    public bool Enabled = false;

    public ListNodeStyle ListStyle = new() {
        LayoutAnchor = LayoutAnchor.TopLeft,
        Position = new Vector2(1024.0f, 720.0f ) / 2.0f,
        Size = new Vector2(600.0f, 200.0f),
        LayoutOrientation = LayoutOrientation.Horizontal,
        BackgroundVisible = true,
        BackgroundColor = KnownColor.Aqua.Vector() with { W = 0.40f },
        ClipContents = true,
        BaseDisable = BaseStyleDisable.Visible | BaseStyleDisable.NodeFlags | BaseStyleDisable.Color | BaseStyleDisable.Margin,
    };

    public CategoryConfig[] CategoryConfigs = [
        new CategoryConfig {
            ModuleType = ModuleType.Daily,
            HeaderLabel = "Daily Tasks",
        },
        
        new CategoryConfig {
            ModuleType = ModuleType.Weekly,
            HeaderLabel = "Weekly Tasks",
        },
        
        new CategoryConfig {
            ModuleType = ModuleType.Special,
            HeaderLabel = "Special Tasks",
        },
    ];
    
    public bool HideDuringQuests = true;
    public bool HideInDuties = true;

    public static TodoConfig Load() 
        => Service.PluginInterface.LoadCharacterFile(Service.ClientState.LocalContentId, "TodoList.config.json", () => new TodoConfig());

    public void Save()
        => Service.PluginInterface.SaveCharacterFile(Service.ClientState.LocalContentId, "TodoList.config.json", this);
}