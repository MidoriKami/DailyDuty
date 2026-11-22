using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DailyDuty.Classes;
using DailyDuty.Enums;
using DailyDuty.Interfaces;
using DailyDuty.Nodes.Overlay;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Components.Builders;
using KamiToolKit.Enums;
using KamiToolKit.Interfaces;
using KamiToolKit.UiOverlay;

namespace DailyDuty.Features;

public class TimerOverlay : Feature<TimerOverlayConfig> {

    public override string DisplayName => Strings.TimersOverlay_DisplayName;

    public override TimerOverlayConfig Config { get; protected set; } = null!;

    private OverlayController? overlayController;

    private readonly Dictionary<string, TimerOverlayNode> timerNodes = [];
    private IListNode<IModule, TimerOverlayListItemNode>? timerSelectionList;
    private TimerEditNode? timerEditNode;

    public override async Task EnableAsync() {
        Config = await TimerOverlayConfig.Load();

        System.Controller.OnModuleStateChanged += OnModuleStateChanged;

        await IFramework.Get().Run(() => {
            overlayController = new OverlayController();
            BuildTimerNodes();
        });
    }

    public override async Task DisableAsync() {
        System.Controller.OnModuleStateChanged -= OnModuleStateChanged;

        await IFramework.Get().Run(() => {
            overlayController?.Dispose();
            overlayController = null;
        });

        timerNodes.Clear();
    }

    private void OnModuleStateChanged(IModule module, bool enabled) => IFramework.Get().Run(() => {
        if (enabled) {
            if (Config.SelectedModules.Contains(module.DisplayName)) {
                AddTimerNode(module);
            }
        }
        else {
            RemoveTimerNode(module);
        }
    });

    public override void BuildConfigUi(NodeBuilder builder) => builder
        .TabbedContent(tabBuilder => tabBuilder
            .AddTab(Strings.ConfigNodeBase_FeatureSettings, BuildSettingsTab)
            .AddTab(Strings.TimersOverlay_Selection, BuildSelectionTab)
            .AddTab("Edit", BuildEditTab));

    private void BuildSettingsTab(NodeBuilder builder) => builder
        .Spacer(10.0f)
        .VerticalList(generalSettingsBuilder => generalSettingsBuilder
            .Checkbox(Strings.TimersOverlay_HideInDuties, Config.HideInDuties, val => {
                Config.HideInDuties = val;
                Config.Save();
            })
            .Checkbox(Strings.TimersOverlay_HideInQuests, Config.HideInQuests, val => {
                Config.HideInQuests = val;
                Config.Save();
            }), node => {
            node.FirstItemSpacing = 5.0f;
            node.ItemSpacing = 4.0f;
        });

    private void BuildSelectionTab(NodeBuilder builder) => builder
        .Spacer(10.0f)
        .TextCentered(Strings.TimersOverlay_SelectionNote)
        .VerticalList(selectionContent => selectionContent
            .List<IModule, TimerOverlayListItemNode>(
                [.. System.Controller.Modules],
                OnModuleSelected,
                list => {
                    list.NodeReferenceData = Config;
                    list.ItemSpacing = 4.0f;
                    timerSelectionList = list;
                }));

    private void BuildEditTab(NodeBuilder builder) => builder
        .Spacer(20.0f)
        .VerticalList(editContent => editContent
            .Combo(
                [.. System.Controller.Modules],
                null!,
                OnEditModuleSelected,
                module => module.DisplayName,
                node => node.LayoutFill = LayoutFill.Horizontal)
            .Dummy(8.0f)
            .AddNode(timerEditNode = new TimerEditNode()));

    private void OnModuleSelected(TimerOverlayListItemNode timerOptionNode, IModule selectedModule) {
        if (timerOptionNode.IsChecked) {
            if (!Config.SelectedModules.Contains(selectedModule.DisplayName)) {
                Config.SelectedModules.Add(selectedModule.DisplayName);
                Config.Save();
            }
            AddTimerNode(selectedModule);
        }
        else {
            if (Config.SelectedModules.Remove(selectedModule.DisplayName)) {
                Config.Save();
            }
            RemoveTimerNode(selectedModule);
        }
    }

    private void OnEditModuleSelected(IModule? selectedModule)
        => timerEditNode?.SetModule(selectedModule, Config);

    private void BuildTimerNodes() {
        if (overlayController is null) return;

        var enabledModules = System.Controller.Modules
            .Where(m => m is { Enabled: true })
            .ToList();

        foreach (var module in enabledModules) {
            if (Config.SelectedModules.Contains(module.DisplayName)) {
                AddTimerNode(module);
            }
        }
    }

    private void AddTimerNode(IModule module) {
        if (overlayController is null) return;
        if (timerNodes.ContainsKey(module.DisplayName)) return;
        if (!module.Enabled) return;

        var timerData = Config.TimerData.GetValueOrDefault(module.DisplayName);

        var nodeWidth = timerData?.Width ?? 300;
        var nodeHeight = timerData?.Height ?? 64;

        var node = new TimerOverlayNode(module, Config) {
            Size = new Vector2(nodeWidth, nodeHeight),
        };

        Vector2 initialPosition;
        if (timerData is not null && timerData.Position != Vector2.Zero) {
            initialPosition = timerData.Position;
        }
        else {
            initialPosition = ComputeGridPosition(module.DisplayName);
        }

        node.SetInitialPosition(initialPosition);
        overlayController.AddNode(node);
        timerNodes[module.DisplayName] = node;
    }

    private void RemoveTimerNode(IModule module) {
        if (overlayController is null) return;
        if (!timerNodes.Remove(module.DisplayName, out var node)) return;

        overlayController.RemoveNode(node);
    }

    // When creating a new timer, pop it in centered ish, but in a grid pattern to prevent overlap.
    private unsafe Vector2 ComputeGridPosition(string moduleName) {
        var screenSize = new Vector2(
            AtkStage.Instance()->ScreenSize.Width,
            AtkStage.Instance()->ScreenSize.Height
        );

        // Find index of this module among selected modules
        var index = Config.SelectedModules.IndexOf(moduleName);
        var total = Config.SelectedModules.Count;

        var col = index % 3;
        var row = index / 3;

        var cols = Math.Min(total, 3);
        var rows = (total + 3 - 1) / 3;

        var gridWidth = cols * 300 + (cols - 1) * 8;
        var gridHeight = rows * 64 + (rows - 1) * 8;

        var startX = (screenSize.X - gridWidth) / 2.0f;
        var startY = (screenSize.Y - gridHeight) / 2.0f;

        return new Vector2(startX + col * (300 + 8), startY + row * (64 + 8));
    }

    public override void UpdateConfigUi() {
        timerSelectionList?.Update();
    }
}

public class TimerOverlayConfig : FeatureConfigFile<TimerOverlayConfig> {
    protected override string FileName => "TimerOverlay.config.json";

    public bool HideInDuties { get; set; }
    public bool HideInQuests { get; set; }
    public List<string> SelectedModules { get; set; } = [];
    public Dictionary<string, TimerOverlayTimerData> TimerData { get; set; } = [];
}

public class TimerOverlayTimerData {
    public Vector2 Position { get; set; }
    public Vector4 Color { get; set; } = new(0.388f, 0.573f, 0.929f, 1.0f);
    public Vector4 TextColor { get; set; } = new(1.0f, 1.0f, 1.0f, 1.0f);
    public string? Label { get; set; }
    public TimerCountDirection CountDirection { get; set; }
    public int FontSize { get; set; } = 16;
    public int? Width { get; set; }
    public int? Height { get; set; }
    public bool ShowLabel { get; set; } = true;
    public bool ShowCountdown { get; set; } = true;
    public bool HideSeconds { get; set; }
    public bool HideWhenCompleted { get; set; }
    public float Scale { get; set; } = 1.0f;

    [JsonIgnore]
    public bool EnableMoving { get; set; }

    [JsonIgnore]
    public bool IsDirty { get; set; }
}
