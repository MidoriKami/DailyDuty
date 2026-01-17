using System;
using System.Linq;
using System.Numerics;
using DailyDuty.Classes;
using DailyDuty.CustomNodes;
using DailyDuty.Enums;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Overlay;

namespace DailyDuty.Features.TodoOverlay;

public unsafe class TodoOverlay : FeatureBase {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Todo List Overlay",
        FileName = "TodoList",
        Type = ModuleType.GeneralFeatures,
        ChangeLog = [
            new ChangeLogInfo(1, "Initial Re-Implementation"),
        ],
        Tags = [ "Tasks", "List" ],
    };

    private OverlayController? overlayController;
    
    public TodoOverlayConfig ModuleTodoOverlayConfig = null!;
    public override NodeBase DisplayNode => new TodoOverlayConfigNode(this);

    protected override void OnFeatureLoad() {
        ModuleTodoOverlayConfig = Utilities.Config.LoadCharacterConfig<TodoOverlayConfig>($"{ModuleInfo.FileName}.config.json");
        if (ModuleTodoOverlayConfig is null) throw new Exception("Failed to load config file");
        
        ModuleTodoOverlayConfig.FileName = ModuleInfo.FileName;
        System.ModuleManager.OnLoadComplete += RebuildPanels;
    }

    protected override void OnFeatureUnload() {
        ModuleTodoOverlayConfig = null!;
    }

    protected override void OnFeatureEnable() {
        overlayController = new OverlayController();

        RebuildPanels();
    }

    protected override void OnFeatureDisable() {
        overlayController?.Dispose();
        overlayController = null;
    }
    
    protected override void OnFeatureUpdate() {
        if (ModuleTodoOverlayConfig.SavePending) {
            Services.PluginLog.Debug($"Saving {ModuleInfo.DisplayName} config");
            ModuleTodoOverlayConfig.Save();
        }
    }
    
    public void RebuildPanels() {
        if (!System.ModuleManager.IsLoadComplete) return;
        System.ModuleManager.OnLoadComplete -= RebuildPanels;

        overlayController?.RemoveAllNodes();
        if (!IsEnabled) return;

        foreach (var (index, option) in ModuleTodoOverlayConfig.Panels.Index()) {

            if (option.Position is null) {
                var position = new Vector2(AtkStage.Instance()->ScreenSize.Width / 4.0f, AtkStage.Instance()->ScreenSize.Height / 3.0f);
                var offset = new Vector2(300.0f, 0.0f) * index;
                
                option.Position = position + offset;
                ModuleTodoOverlayConfig.MarkDirty();
            }
            
            overlayController?.CreateNode(() => new TodoPanelNode {
                Position = option.Position.Value,
                Size = new Vector2(200.0f, 200.0f),
                Config = option,
                ModuleTodoOverlayConfig = ModuleTodoOverlayConfig,
            });
        }
    }
}
