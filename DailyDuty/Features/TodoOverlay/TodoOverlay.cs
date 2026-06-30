using DailyDuty.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using DailyDuty.Classes;
using DailyDuty.CustomNodes;
using DailyDuty.Enums;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.UiOverlay;

namespace DailyDuty.Features.TodoOverlay;

public class TodoOverlay : FeatureBase {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = Strings.TodoListOverlay_DisplayName,
        FileName = "TodoList",
        Type = ModuleType.GeneralFeatures,
        Tags = ["Tasks", "List"],
    };

    private OverlayController? overlayController;

    public TodoOverlayConfig ModuleTodoOverlayConfig = null!;
    public override NodeBase DisplayNode => new TodoOverlayConfigNode(this);

    private Dictionary<TodoPanelConfig, TodoPanelNode>? panelNodes;

    protected override async Task OnFeatureLoad() {
        ModuleTodoOverlayConfig = await Config.LoadCharacterConfig<TodoOverlayConfig>($"{ModuleInfo.FileName}.config.json");
        if (ModuleTodoOverlayConfig is null) throw new Exception("Failed to load config file");

        ModuleTodoOverlayConfig.FileName = ModuleInfo.FileName;
    }

    protected override Task OnFeatureUnload() {
        ModuleTodoOverlayConfig = null!;

        return Task.CompletedTask;
    }

    protected override async Task OnFeatureEnable() {
        panelNodes = [];

        await Services.Framework.RunSafely(() => {
            overlayController = new OverlayController();
            RebuildPanels();
        });
    }

    protected override async Task OnFeatureDisable() {
        await Services.Framework.RunSafely(() => overlayController?.Dispose());
        overlayController = null;

        panelNodes?.Clear();
        panelNodes = null;
    }

    protected override void OnFeatureUpdate() {
        if (ModuleTodoOverlayConfig.SavePending) {
            Services.PluginLog.Debug($"Saving {ModuleInfo.DisplayName} config");
            ModuleTodoOverlayConfig.Save();
        }
    }

    public unsafe void RebuildPanels() {
        if (!IsEnabled) {
            overlayController?.RemoveAllNodes();
            panelNodes?.Clear();
            return;
        }

        if (panelNodes is null) return;
        if (overlayController is null) return;

        // Remove no longer used panels.
        foreach (var (panelConfig, panelNode) in panelNodes) {
            if (!ModuleTodoOverlayConfig.Panels.Contains(panelConfig)) {
                overlayController.RemoveNode(panelNode);
            }
        }

        // Add new panels.
        foreach (var (index, option) in ModuleTodoOverlayConfig.Panels.Index()) {
            if (!panelNodes.ContainsKey(option)) {
                if (option.Position is null) {
                    var position = new Vector2(AtkStage.Instance()->ScreenSize.Width / 4.0f, AtkStage.Instance()->ScreenSize.Height / 3.0f);
                    var offset = new Vector2(300.0f, 0.0f) * index;

                    option.Position = position + offset;
                    ModuleTodoOverlayConfig.MarkDirty();
                }

                var newPanelNode = new TodoPanelNode {
                    Position = option.Position.Value,
                    Size = new Vector2(200.0f, 200.0f),
                    Config = option,
                    ModuleTodoOverlayConfig = ModuleTodoOverlayConfig,
                };

                overlayController.AddNode(newPanelNode);
                panelNodes.Add(option, newPanelNode);
            }
        }
    }
}
