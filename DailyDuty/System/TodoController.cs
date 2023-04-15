using System;
using System.Drawing;
using System.Numerics;
using DailyDuty.Abstracts;
using DailyDuty.Models.Attributes;
using DailyDuty.Models.Enums;
using DailyDuty.System.Commands;
using DailyDuty.System.Helpers;
using DailyDuty.System.Localization;
using DailyDuty.Views.Components;
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

    [ConfigOption("RightAlign")]
    public bool RightAlign = false;

    [ConfigOption("EnableDailyTasks")]
    public bool DailyTasks = true;
    
    [ConfigOption("EnableWeeklyTasks")]
    public bool WeeklyTasks = true;
    
    [ConfigOption("EnableSpecialTasks")]
    public bool SpecialTasks = true;

    [ConfigOption("ShowHeaders")]
    public bool ShowHeaders = true;

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
    public Vector2 Position = new Vector2(1024, 720) / 2.0f;

    [ConfigOption("FontSize", 5, 36)]
    public int FontSize = 20;

    [ConfigOption("HeaderSize", 5, 36)] 
    public int HeaderFontSize = 24;

    [ConfigOption("CategorySpacing", 0, 100)]
    public int CategorySpacing = 12;
    
    [ConfigOption("HeaderColor", 1.0f, 1.0f, 1.0f, 1.0f)]
    public Vector4 HeaderTextColor = new(1.0f, 1.0f, 1.0f, 1.0f);

    [ConfigOption("HeaderOutlineColor", 0.0f, 0.0f, 0.0f, 1.0f)]
    public Vector4 HeaderTextOutline = new(0.0f, 0.0f, 0.0f, 1.0f);
    
    [ConfigOption("ModuleTextColor", 1.0f, 1.0f, 1.0f, 1.0f)]
    public Vector4 ModuleTextColor = new(1.0f, 1.0f, 1.0f, 1.0f);

    [ConfigOption("ModuleOutlineColor", 0.0f, 0.0f, 0.0f, 1.0f)]
    public Vector4 ModuleOutlineColor = new(0.0f, 0.0f, 0.0f, 1.0f);
}

public class TodoController : IDisposable
{
    public TodoConfig Config = new();
    private bool configChanged;
    private readonly TodoCommands todoCommands = new();
    private TodoUiController? uiController;

    public void Dispose() => Unload();
    
    public void Load()
    {
        PluginLog.Debug($"[TodoConfig] Loading Todo System");
        
        KamiCommon.CommandManager.AddCommand(todoCommands);
        Config = LoadConfig();
        
        uiController ??= new TodoUiController();
    }
    
    public void Unload()
    {
        KamiCommon.CommandManager.RemoveCommand(todoCommands);

        uiController?.Dispose();
        uiController = null;
    }
    
    public void DrawConfig()
    {
        var configOptions = AttributeHelper.GetFieldAttributes<ConfigOption>(Config);
        
        TodoEnableView.Draw(Config, SaveConfig);
        GenericConfigView.Draw(configOptions, Config, () =>
        {
            SaveConfig();
            foreach (var module in DailyDutySystem.ModuleController.GetModules())
            {
                module.ModuleConfig.TodoOptions.StyleChanged = true;
            }
            
        }, Strings.TodoDisplayConfiguration);
    }

    public void Show() => uiController?.Show(Config.Enable);

    public void Hide() => uiController?.Hide();

    public void Update()
    {
        UpdateCategory(ModuleType.Daily, Config.DailyTasks);
        UpdateCategory(ModuleType.Weekly, Config.WeeklyTasks);
        UpdateCategory(ModuleType.Special, Config.SpecialTasks);
        
        uiController?.Show(Config.Enable);
        if(Config.HideDuringQuests && Condition.IsInQuestEvent()) uiController?.Show(false);
        if(Config.HideInDuties && Condition.IsBoundByDuty()) uiController?.Show(false);
        
        uiController?.Update(Config);
        
        if(configChanged) SaveConfig();
        configChanged = false;
    }

    private void UpdateCategory(ModuleType type, bool enabled)
    {
        foreach (var module in DailyDutySystem.ModuleController.GetModules(type))
        {
            if (enabled)
            {
                if (module.ModuleConfig.TodoOptions.StyleChanged)
                {
                    uiController?.UpdateModuleStyle(type, module.ModuleName, GetModuleTextStyleOptions(module));
                    uiController?.UpdateHeaderStyle(type, HeaderOptions);
                    
                    module.ModuleConfig.TodoOptions.StyleChanged = false;
                }
            }
            
            uiController?.UpdateModule(type, module.ModuleName, GetModuleTodoLabel(module), GetModuleActiveState(module) && enabled);
            uiController?.UpdateCategoryHeader(type, GetCategoryLabel(type), Config.ShowHeaders);
            uiController?.UpdateCategory(type, enabled);
        }
    }
    
    private string GetModuleTodoLabel(BaseModule module)
    {
        var todoOptions = module.ModuleConfig.TodoOptions;

        if (todoOptions.UseCustomTodoLabel && todoOptions.CustomTodoLabel != string.Empty)
        {
            return todoOptions.CustomTodoLabel;
        }

        return module.ModuleName.GetLabel();
    }

    private bool GetModuleActiveState(BaseModule module)
    {
        if (!module.ModuleConfig.ModuleEnabled) return false;
        if (!module.ModuleConfig.TodoOptions.Enabled) return false;
        if (module.ModuleStatus is not ModuleStatus.Incomplete) return false;

        return true;
    }

    private string GetCategoryLabel(ModuleType type) => type switch
    {
        ModuleType.Daily => Config.DailyLabel,
        ModuleType.Weekly => Config.WeeklyLabel,
        ModuleType.Special => Config.SpecialLabel,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };
    
    private TextNodeOptions HeaderOptions => new()
    {
        Alignment = AlignmentType.Left,
        TextColor = Config.HeaderTextColor,
        EdgeColor = Config.HeaderTextOutline,
        BackgroundColor = KnownColor.White.AsVector4(),
        FontSize = (byte) Config.HeaderFontSize,
        Flags = TextFlags.AutoAdjustNodeSize | TextFlags.Edge | TextFlags.Glare,
        Type = NodeType.Text,
    };

    private TextNodeOptions GetModuleTextStyleOptions(BaseModule module) => new()
    {
        Alignment = AlignmentType.Left,
        TextColor = module.ModuleConfig.TodoOptions.OverrideTextColor ? module.ModuleConfig.TodoOptions.TextColor : Config.ModuleTextColor,
        EdgeColor = module.ModuleConfig.TodoOptions.OverrideTextColor ? module.ModuleConfig.TodoOptions.TextOutline : Config.ModuleOutlineColor,
        BackgroundColor = KnownColor.White.AsVector4(),
        FontSize = (byte) Config.FontSize,
        Flags = TextFlags.AutoAdjustNodeSize | TextFlags.Edge | TextFlags.Glare,
        Type = NodeType.Text,
    };


    private TodoConfig LoadConfig() => (TodoConfig) FileController.LoadFile("Todo.config.json", Config);
    
    public void SaveConfig() => FileController.SaveFile("Todo.config.json", Config.GetType(), Config);
}