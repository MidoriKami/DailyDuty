// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedParameter.Local
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using DailyDuty.Abstracts;
using DailyDuty.Models;
using DailyDuty.Models.Enums;
using DailyDuty.System.Localization;
using Dalamud.Game.Addon;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using KamiLib;
using KamiLib.Atk;
using KamiLib.AutomaticUserInterface;
using KamiLib.ChatCommands;
using KamiLib.Commands;
using KamiLib.GameState;
using KamiLib.Utilities;

namespace DailyDuty.System;

public class TodoController : IDisposable
{
    public TodoConfig Config = new();
    private bool configChanged;
    private TodoUiController? uiController;
    private Vector2? holdOffset;
    private readonly Dictionary<ModuleName, DisplayData> displayDataCache = new();
    private record DisplayData(bool ShowModule, bool ShowHeader, bool ShowCategory);

    public TodoController()
    {
        Service.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, "NamePlate", (_, _) => Load());
        Service.AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, "NamePlate", (_, _) => Unload());
    }
    
    public void Dispose()
    { 
        Service.AddonLifecycle.UnregisterListener(AddonEvent.PostSetup, "NamePlate");
        Service.AddonLifecycle.UnregisterListener(AddonEvent.PreFinalize, "NamePlate");
        
        Unload();
    }

    public void Load()
    {
        Service.Log.Debug($"[TodoConfig] Loading Todo System");
        
        CommandController.RegisterCommands(this);
        Config = LoadConfig();
        
        uiController ??= new TodoUiController();

        foreach (var module in DailyDutySystem.ModuleController.GetModules())
        {
            module.ModuleConfig.StyleChanged = true;
        }
    }
    
    public void Unload()
    {
        Service.Log.Debug("[TodoConfig] Unloading Todo System");
        
        CommandController.UnregisterCommands(this);
        
        uiController?.Dispose();
        uiController = null;
    }
    
    public void DrawConfig()
    {
        DrawableAttribute.DrawAttributes(Config, () =>
        {
            SaveConfig();
            foreach (var module in DailyDutySystem.ModuleController.GetModules())
            {
                module.ModuleConfig.StyleChanged = true;
            }
        });
    }

    public void DrawExtras()
    {
        if (Config.CanDrag && uiController != null)
        {
            var size = uiController.GetSize() + Vector2.One * TodoUiController.EdgeSize * 2.0f;

            var positionOffsetX = Config.Anchor.HasFlag(WindowAnchor.TopRight) ? size.X - TodoUiController.EdgeSize : TodoUiController.EdgeSize;
            var positionOffsetY = Config.Anchor.HasFlag(WindowAnchor.BottomLeft) ? size.Y - TodoUiController.EdgeSize : TodoUiController.EdgeSize;
            
            var position = Config.Position - new Vector2(positionOffsetX, positionOffsetY);

            ImGui.SetNextWindowPos(position);
            ImGui.SetNextWindowSize(size);
            if (ImGui.Begin("##DailyDutyTodoDrag", ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoBackground))
            {
                ImGui.GetBackgroundDrawList().AddRect(position, position + size, ImGui.GetColorU32(new Vector4(1.0f, 0.0f, 0.0f, 1.0f)), 0.0f, ImDrawFlags.RoundCornersNone, 2.0f);
                
                var pos = ImGui.GetMousePos();
                if (ImGui.IsMouseDown(ImGuiMouseButton.Left) && ImGui.IsWindowFocused()) 
                {
                    holdOffset ??= Config.Position - pos;
                
                    var old = Config.Position;
                    Config.Position = (Vector2)(pos + holdOffset)!;
                
                    if (old != Config.Position)
                        configChanged = true;
                } 
                else 
                {
                    holdOffset = null;
                }
                
                var textPosition = position with { Y = position.Y + size.Y };
                
                ImGui.GetBackgroundDrawList().AddText(KamiCommon.FontManager.Axis12.ImFont, 21, textPosition - new Vector2(1, 0), ImGui.GetColorU32(KnownColor.Black.AsVector4()), Strings.WindowDraggingEnabled);
                ImGui.GetBackgroundDrawList().AddText(KamiCommon.FontManager.Axis12.ImFont, 21, textPosition - new Vector2(0, 1), ImGui.GetColorU32(KnownColor.Black.AsVector4()), Strings.WindowDraggingEnabled);
                ImGui.GetBackgroundDrawList().AddText(KamiCommon.FontManager.Axis12.ImFont, 21, textPosition + new Vector2(0, 1), ImGui.GetColorU32(KnownColor.Black.AsVector4()), Strings.WindowDraggingEnabled);
                ImGui.GetBackgroundDrawList().AddText(KamiCommon.FontManager.Axis12.ImFont, 21, textPosition + new Vector2(1, 0), ImGui.GetColorU32(KnownColor.Black.AsVector4()), Strings.WindowDraggingEnabled);
                ImGui.GetBackgroundDrawList().AddText(KamiCommon.FontManager.Axis12.ImFont, 21, textPosition, ImGui.GetColorU32(KnownColor.OrangeRed.AsVector4()), Strings.WindowDraggingEnabled);
            }
            
            ImGui.End();
        }
    }

    public void Update()
    {
        uiController?.Show(Config.Enable);
        
        if (Config.Enable)
        {
            UpdateCategory(ModuleType.Daily, Config.DailyTasks);
            UpdateCategory(ModuleType.Weekly, Config.WeeklyTasks);
            UpdateCategory(ModuleType.Special, Config.SpecialTasks);
        
            if(Config.HideDuringQuests && Condition.IsInQuestEvent()) uiController?.Show(false);
            if(Config.HideInDuties && Condition.IsBoundByDuty()) uiController?.Show(false);
            if(Condition.IsDutyRecorderPlayback()) uiController?.Show(false);
        
            uiController?.Update(Config);
        }
        
        if(configChanged) SaveConfig();
        configChanged = false;
    }

    private void UpdateCategory(ModuleType type, bool enabled)
    {
        foreach (var module in DailyDutySystem.ModuleController.GetModules(type))
        {
            var wasStyleChanged = module.ModuleConfig.StyleChanged;

            if (enabled && wasStyleChanged)
            {
                uiController?.UpdateModuleStyle(type, module.ModuleName, GetModuleTextStyleOptions(module));
                uiController?.UpdateHeaderStyle(type, HeaderOptions);
                uiController?.UpdateCategoryStyle(type, BackgroundImageOptions);
                    
                module.ModuleConfig.StyleChanged = false;
            }

            var name = module.ModuleName;
            var display = new DisplayData(
                GetModuleActiveState(module) && enabled || Config.PreviewMode,
                Config.ShowHeaders,
                enabled);

            if (!displayDataCache.TryGetValue(name, out var cached) || !cached.Equals(display) || wasStyleChanged)
            {
                uiController?.UpdateModule(type, name, GetModuleTodoLabel(module), display.ShowModule);
                uiController?.UpdateCategoryHeader(type, GetCategoryLabel(type), display.ShowHeader);
                uiController?.UpdateCategory(type, display.ShowCategory);
                displayDataCache[name] = display;
            }
        }
    }
    
    private string GetModuleTodoLabel(BaseModule module)
    {
        var todoOptions = module.ModuleConfig;

        if (todoOptions.UseCustomTodoLabel && todoOptions.CustomTodoLabel != string.Empty)
        {
            return todoOptions.CustomTodoLabel;
        }

        return module.ModuleName.GetLabel();
    }

    private bool GetModuleActiveState(BaseModule module)
    {
        if (!module.ModuleConfig.ModuleEnabled) return false;
        if (!module.ModuleConfig.TodoEnabled) return false;
        if (module.ModuleStatus is not ModuleStatus.Incomplete) return false;

        return true;
    }

    [DoubleTierCommandHandler("TodoEnable", "todo", "enable")]
    private void ShowTodoCommand(params string[]? _)
    {
        if (!Service.ClientState.IsLoggedIn) return;
        if (Service.ClientState.IsPvP)
        {
            Chat.PrintError("This command cannot be used while in a PvP area");
            return;
        }

        Config.Enable = true;
        SaveConfig();
    }
    
    [DoubleTierCommandHandler("TodoDisable", "todo", "disable")]
    private void HideTodoCommand(params string[]? _)
    {
        if (!Service.ClientState.IsLoggedIn) return;
        if (Service.ClientState.IsPvP)
        {
            Chat.PrintError("This command cannot be used while in a PvP area");
            return;
        }

        Config.Enable = false;
        SaveConfig();
    }
    
    [DoubleTierCommandHandler("TodoToggle", "todo", "toggle")]
    private void ToggleTodoCommand(params string[]? _)
    {
        if (!Service.ClientState.IsLoggedIn) return;
        if (Service.ClientState.IsPvP)
        {
            Chat.PrintError("This command cannot be used while in a PvP area");
            return;
        }

        Config.Enable = !Config.Enable;
        SaveConfig();
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
        Flags = GetHeaderFlags(),
        Type = NodeType.Text,
    };

    private TextNodeOptions GetModuleTextStyleOptions(BaseModule module) => new()
    {
        Alignment = AlignmentType.Left,
        TextColor = module.ModuleConfig.OverrideTextColor ? module.ModuleConfig.TodoTextColor : Config.ModuleTextColor,
        EdgeColor = module.ModuleConfig.OverrideTextColor ? module.ModuleConfig.TodoTextOutline : Config.ModuleOutlineColor,
        BackgroundColor = KnownColor.White.AsVector4(),
        FontSize = (byte) Config.FontSize,
        Flags = GetModuleFlags(),
        Type = NodeType.Text,
    };

    private ImageNodeOptions BackgroundImageOptions => new()
    {
        Color = Config.CategoryBackgroundColor
    };

    private TextFlags GetHeaderFlags()
    {
        var flags = TextFlags.AutoAdjustNodeSize;

        if (Config.HeaderItalic) flags |= TextFlags.Italic;
        if (Config.Edge) flags |= TextFlags.Edge;
        if (Config.Glare) flags |= TextFlags.Glare;

        return flags;
    }
    
    private TextFlags GetModuleFlags()
    {
        var flags = TextFlags.AutoAdjustNodeSize;

        if (Config.ModuleItalic) flags |= TextFlags.Italic;
        if (Config.Edge) flags |= TextFlags.Edge;
        if (Config.Glare) flags |= TextFlags.Glare;

        return flags;
    }
    
    public void Show() => uiController?.Show(Config.Enable);
    public void Hide() => uiController?.Hide();
    private TodoConfig LoadConfig() => CharacterFileController.LoadFile<TodoConfig>("Todo.config.json", Config);
    public void SaveConfig() => CharacterFileController.SaveFile("Todo.config.json", Config.GetType(), Config);
}