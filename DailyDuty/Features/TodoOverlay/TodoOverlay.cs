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
    
    public Config ModuleConfig = null!;
    public override NodeBase DisplayNode => new ConfigNode(this);

    protected override void OnFeatureLoad() {
        ModuleConfig = Utilities.Config.LoadCharacterConfig<Config>($"{ModuleInfo.FileName}.config.json");
        if (ModuleConfig is null) throw new Exception("Failed to load config file");
        
        ModuleConfig.FileName = ModuleInfo.FileName;
        System.ModuleManager.OnLoadComplete += RebuildPanels;
    }

    protected override void OnFeatureUnload() {
        ModuleConfig = null!;
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
        if (ModuleConfig.SavePending) {
            Services.PluginLog.Debug($"Saving {ModuleInfo.DisplayName} config");
            ModuleConfig.Save();
        }
    }
    
    public void RebuildPanels() {
        if (!System.ModuleManager.IsLoadComplete) return;
        System.ModuleManager.OnLoadComplete -= RebuildPanels;

        overlayController?.RemoveAllNodes();
        if (!IsEnabled) return;

        foreach (var (index, option) in ModuleConfig.Panels.Index()) {

            if (option.Position is null) {
                var position = new Vector2(AtkStage.Instance()->ScreenSize.Width / 4.0f, AtkStage.Instance()->ScreenSize.Height / 3.0f);
                var offset = new Vector2(300.0f, 0.0f) * index;
                
                option.Position = position + offset;
                ModuleConfig.MarkDirty();
            }
            
            overlayController?.CreateNode(() => new TodoPanelNode {
                Position = option.Position.Value,
                Size = new Vector2(200.0f, 200.0f),
                Config = option,
                ModuleConfig = ModuleConfig,
            });
        }
    }
}
