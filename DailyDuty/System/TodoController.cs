using System;
using System.Linq;
using System.Numerics;
using System.Reflection;
using DailyDuty.Models.Attributes;
using DailyDuty.Models.Enums;
using DailyDuty.System.Commands;
using DailyDuty.System.Localization;
using DailyDuty.Views.Components;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib;
using KamiLib.Atk;
using KamiLib.GameState;

namespace DailyDuty.System;

public class UiColor
{
    public required ushort ColorKey { get; set; }
}

public class TodoConfig
{
    public bool Enable = true;
    
    [ConfigOption("EnableDailyTasks")]
    public bool DailyTasks = true;
    
    [ConfigOption("EnableWeeklyTasks")]
    public bool WeeklyTasks = true;
    
    [ConfigOption("EnableSpecialTasks")]
    public bool SpecialTasks = true;

    [ConfigOption("HideInQuestEvent")]
    public bool HideDuringQuests = true;
    
    [ConfigOption("HideInDuties")]
    public bool HideInDuties = true;

    [ConfigOption("DailyTasksLabel", true)]
    public string DailyLabel = "Daily Tasks";
    
    [ConfigOption("WeeklyTasksLabel", true)]
    public string WeeklyLabel = "Weekly Tasks";
    
    [ConfigOption("SpecialTasksLabel", true)]
    public string SpecialLabel = "Special Tasks";

    [ConfigOption("Position")]
    public Vector2 TextNodePosition = new(750, 512);
    
    [ConfigOption("TextAlignment")]
    public AlignmentType AlignmentType = AlignmentType.TopLeft;
    
    [ConfigOption("LineSpacing", 5, 30)]
    public int LineSpacing = 20;
    
    [ConfigOption("FontSize", 5, 30)]
    public int FontSize = 14;

    [ConfigOption("TextColor", 1.0f, 1.0f, 1.0f, 1.0f)]
    public Vector4 TextColor = Vector4.One;
    
    [ConfigOption("TextBorderColor", 142 / 255.0f, 106 / 255.0f, 12 / 255.0f, 1.0f)]
    public Vector4 TextBorderColor = new(142 / 255.0f, 106 / 255.0f, 12 / 255.0f, 1.0f);
    
    [ConfigOption("HeaderOutlineColor", 14)]
    public UiColor HeaderGlowKey = new() { ColorKey = 14 };
}

public unsafe class TodoController : IDisposable
{
    public TodoConfig Config = new();
    private bool configChanged;
    private static AtkUnitBase* ParentAddon => (AtkUnitBase*) Service.GameGui.GetAddonByName("NamePlate");
    private readonly TodoCommands todoCommands = new();
    private TextNode? todoListNode;

    public void Load()
    {
        PluginLog.Debug($"[TodoConfig] Loading Todo System");
        
        KamiCommon.CommandManager.AddCommand(todoCommands);
        Config = LoadConfig();
        
        if(ParentAddon is null) PluginLog.Warning("NamePlate is Null on TodoController.Load");
        todoListNode ??= new TextNode(GetTextNodeOptions(), ParentAddon);
    }
    
    public void DrawConfig()
    {
        var fields = Config
            .GetType()
            .GetFields(); 
        
        var configOptions = fields
            .Where(field => field.GetCustomAttribute(typeof(ConfigOption)) is not null)
            .Select(field => (field, (ConfigOption) field.GetCustomAttribute(typeof(ConfigOption))!))
            .ToList();
        
        TodoEnableView.Draw(Config, SaveConfig);
        GenericConfigView.Draw(configOptions, Config, SaveConfig, Strings.TodoDisplayConfiguration);
    }

    public void Show() => todoListNode?.ToggleVisibility(Config.Enable);
    public void Hide() => todoListNode?.ToggleVisibility(false);

    public void Update()
    {
        var seString = new SeStringBuilder();

        if(Config.DailyTasks) UpdateCategory(seString, ModuleType.Daily);
        if(Config.WeeklyTasks) UpdateCategory(seString, ModuleType.Weekly);
        if(Config.SpecialTasks) UpdateCategory(seString, ModuleType.Special);

        var encodedString = seString.Encode();
            
        todoListNode?.SetText(encodedString);
            
        todoListNode?.ToggleVisibility(encodedString.Length != 0 && Config.Enable);
         if(Config.HideInDuties && Condition.IsBoundByDuty()) todoListNode?.ToggleVisibility(false);
         if(Config.HideDuringQuests && Condition.IsInCutsceneOrQuestEvent()) todoListNode?.ToggleVisibility(false);
            
        todoListNode?.UpdateOptions(GetTextNodeOptions());
        
        if(configChanged) SaveConfig();
        configChanged = false;
    }

    private void UpdateCategory(SeStringBuilder builder, ModuleType type)
    {
        var tasks = DailyDutyPlugin.System.ModuleController
            .GetModules(type)
            .Where(module => module.ModuleConfig.ModuleEnabled)
            .Where(module => module.ModuleStatus is ModuleStatus.Incomplete)
            .ToList();

        if (tasks.Count == 0) return;

        var label = type switch
        {
            ModuleType.Daily => Config.DailyLabel,
            ModuleType.Weekly => Config.WeeklyLabel,
            ModuleType.Special => Config.SpecialLabel,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
        
        builder.AddUiGlow(label, Config.HeaderGlowKey.ColorKey);
        builder.Add(new NewLinePayload());
        
        foreach (var module in tasks)
        {
            if (module.ModuleConfig.UseCustomTodoLabel && module.ModuleConfig.CustomTodoLabel != string.Empty)
            {
                builder.AddText(module.ModuleConfig.CustomTodoLabel);
                builder.Add(new NewLinePayload());
            }
            else
            {
                builder.AddText(module.ModuleName.GetLabel());
                builder.Add(new NewLinePayload());
            }
        }
    }
    
    public void Unload()
    {
        KamiCommon.CommandManager.RemoveCommand(todoCommands);
        
        todoListNode?.Dispose();
        todoListNode = null;
    }
    
    public void Dispose() => Unload();

    private TextNodeOptions GetTextNodeOptions() => new()
    {
        Alignment = Config.AlignmentType,
        Id = 1000,
        Position = Config.TextNodePosition,
        Size = new Vector2(200.0f, 200.0f),
        BackgroundColor = Vector4.One,
        EdgeColor = Config.TextBorderColor,
        FontSize = (byte) Config.FontSize,
        LineSpacing = (byte) Config.LineSpacing,
        TextColor = Config.TextColor
    };

    private TodoConfig LoadConfig() => (TodoConfig) FileController.LoadFile("Todo.config.json", Config);
    
    public void SaveConfig()
    {
        todoListNode?.UpdateOptions(GetTextNodeOptions());

        FileController.SaveFile("Todo.config.json", Config.GetType(), Config);
    }
}