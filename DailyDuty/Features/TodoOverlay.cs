using System.Collections.Generic;
using System.Numerics;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DailyDuty.Classes;
using DailyDuty.Interfaces;
using DailyDuty.Nodes.Overlay;
using Dalamud.Plugin.Services;
using KamiToolKit.Classes;
using KamiToolKit.Components.Builders;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using KamiToolKit.UiOverlay;

namespace DailyDuty.Features;

public class TodoOverlay : Feature<TodoOverlayConfig> {

    public override string DisplayName => "Todo Panel Overlay";

    public override TodoOverlayConfig Config { get; protected set; } = null!;

    private OverlayController? overlayController;

    private readonly Dictionary<TodoPanelConfig, TodoPanelNode> panelNodes = [];
    private TodoPanelEditNode? panelEditNode;
    private TodoPanelConfig? selectedPanel;
    private DropDownNode<TodoPanelConfig?> dropDownNode = null!;

    public override async Task EnableAsync() {
        Config = await TodoOverlayConfig.Load();

        await IFramework.Get().Run(() => {
            overlayController = new OverlayController();
            BuildPanelNodes();
        });
    }

    public override async Task DisableAsync() {
        await IFramework.Get().Run(() => {
            overlayController?.Dispose();
            overlayController = null;
        });

        panelEditNode = null;

        selectedPanel = null;
        panelNodes.Clear();
    }

    public override void BuildConfigUi(NodeBuilder builder) => builder
        .TabbedContent(tabBuilder => tabBuilder
            .AddTab(Strings.ConfigNodeBase_FeatureSettings, BuildSettingsTab)
            .AddTab("Panels", BuildEditTab));

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

    private void BuildEditTab(NodeBuilder builder) => builder
        .Spacer(10.0f)
        .VerticalList(verticalListBuilder => verticalListBuilder
            .RatioSplit([0.9f, 0.05f, 0.05f],
                configRatio => {
                    configRatio.LayoutFill = LayoutFill.Horizontal;
                    configRatio.ShowSeparators = false;
                },
                comboBuilder => comboBuilder.Combo(
                    Config.Panels!,
                    selectedPanel,
                    OnEditPanelSelected,
                    panel => panel?.Label ?? Strings.TodoOverlay_NewPanel,
                    out dropDownNode,
                    configure => configure.PlaceholderString = Strings.TodoOverlay_PanelsPlaceholder
                ),
                addBuilder => addBuilder
                    .CircleButton(CircleButtonIcon.Add, OnAddPanelClicked, button => {
                        button.Size = new Vector2(24.0f, 24.0f);
                    }),
                removeBuilder => removeBuilder
                    .CircleButton(CircleButtonIcon.Cross, OnRemovePanelClicked, button => {
                        button.Size = new Vector2(24.0f, 24.0f);
                    }))
            .Dummy(8.0f)
            .AddNode(panelEditNode = new TodoPanelEditNode())
        );

    private void OnAddPanelClicked() {
        var newPanel = new TodoPanelConfig();

        Config.Panels.Add(newPanel);
        Config.Save();

        AddPanelNode(newPanel);

        dropDownNode.Options = Config.Panels!;
        dropDownNode.SelectedOption = newPanel;

        selectedPanel = newPanel;
        panelEditNode?.SetPanel(newPanel, Config, panelNodes[newPanel]);
    }

    private void OnRemovePanelClicked() {
        if (selectedPanel is null) return;

        Config.Panels.Remove(selectedPanel);
        Config.Save();

        RemovePanelNode(selectedPanel);

        dropDownNode.Options = Config.Panels!;
        dropDownNode.SelectedOption = null;

        selectedPanel = null;
        panelEditNode?.SetPanel(null, Config);
    }

    private void OnEditPanelSelected(TodoPanelConfig? selectedPanelConfig) {
        selectedPanel = selectedPanelConfig;
        panelEditNode?.SetPanel(selectedPanelConfig, Config, selectedPanelConfig is not null ? panelNodes[selectedPanelConfig] : null);
    }

    private void BuildPanelNodes() {
        if (overlayController is null) return;

        foreach (var panel in Config.Panels) {
            AddPanelNode(panel);
        }
    }

    private void AddPanelNode(TodoPanelConfig panel) {
        if (overlayController is null) return;
        if (panelNodes.ContainsKey(panel)) return;

        var node = new TodoPanelNode(panel, Config) {
            Size = new Vector2(panel.Width, 50.0f),
        };

        overlayController.AddNode(node);
        panelNodes[panel] = node;
    }

    private void RemovePanelNode(TodoPanelConfig panel) {
        if (overlayController is null) return;
        if (!panelNodes.Remove(panel, out var node)) return;

        overlayController.RemoveNode(node);
    }
}

public class TodoOverlayConfig : FeatureConfigFile<TodoOverlayConfig> {
    protected override string FileName => "TodoOverlay.config.json";

    public bool HideInDuties { get; set; }
    public bool HideInQuests { get; set; }
    public List<TodoPanelConfig> Panels { get; set; } = [];
}

public class TodoPanelConfig {
    public Vector2? Position { get; set; }
    public float Scale { get; set; } = 1.0f;
    public string Label { get; set; } = Strings.TodoOverlay_NewPanel;
    public bool IsCollapsed { get; set; }
    public bool ShowFrame { get; set; } = true;
    public VerticalListAlignment Alignment { get; set; } = VerticalListAlignment.Left;
    public HashSet<string> Modules { get; set; } = [];
    public Vector4 TextColor { get; set; } = ColorHelper.GetColor(1);
    public Vector4 OutlineColor { get; set; } = ColorHelper.GetColor(53);
    public int ItemSpacing { get; set; } = 0;
    public float Alpha { get; set; } = 1.0f;
    public float ButtonAlpha { get; set; } = 1.0f;
    public int Width { get; set; } = 225;
    public bool AttachToQuestList { get; set; }
    public bool AutoHideWhenEmpty { get; set; }

    [JsonIgnore]
    public bool EnableMoving { get; set; }

    [JsonIgnore]
    public bool IsDirty { get; set; }
}

