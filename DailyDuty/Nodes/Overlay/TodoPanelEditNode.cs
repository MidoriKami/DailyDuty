using DailyDuty.Features;
using DailyDuty.Interfaces;
using KamiToolKit.Core.Classes;
using KamiToolKit.Controls.Builders;
using KamiToolKit.Core.Enums;
using KamiToolKit.Core.Interfaces;
using KamiToolKit.UI.Nodes;

namespace DailyDuty.Nodes.Overlay;

public class TodoPanelEditNode : ResNode, ILayoutFiller {

    public LayoutFill LayoutFill { get; set; } = LayoutFill.Both;

    private readonly ILayoutNode layoutContainer;
    private TabbedContentNode? optionNode;

    private TextInputNode labelInput = null!;
    private ColorEditNode textColorEdit = null!;
    private ColorEditNode outlineColorEdit = null!;
    private SliderNode spacingSlider = null!;
    private SliderNode widthSlider = null!;
    private SliderNode listXOffsetSlider = null!;
    private FloatSliderNode alphaSlider = null!;
    private FloatSliderNode buttonAlphaSlider = null!;
    private FloatSliderNode scaleSlider = null!;
    private CheckboxNode showFrameCheck = null!;
    private CheckboxNode enableMovingCheck = null!;
    private CheckboxNode attachToQuestListCheck = null!;
    private CheckboxNode autoHideCheck = null!;
    private DropDownNode<VerticalListAlignment> alignmentCombo = null!;
    private IListNode<IModule, TodoListItemNode>? moduleList;

    private TextNode? warningNode;

    private TodoPanelConfig? panel;
    private TodoOverlayConfig? config;
    private TodoPanelNode? panelNode;
    private IDropDownNode<TodoPanelConfig?>? dropDownNode;

    public TodoPanelEditNode() {
        layoutContainer = NativeUiBuilder.Create(builder => builder
            .FittedContent(fittedBuilder => fittedBuilder
                .TabbedContent(tabBuilder => tabBuilder
                        .AddTab("Modules", BuildModulesTab)
                        .AddTab("Style", BuildFeaturesTab),
                    out optionNode,
                    tabbed => tabbed.LayoutFill = LayoutFill.Both)
                .TextWarningFilled("Select a panel to configure", out warningNode)
            ));

        optionNode!.IsVisible = false;
        layoutContainer.AttachNode(this);
    }

    private void BuildModulesTab(NodeBuilder builder) => builder
        .Spacer(5.0f)
        .List<IModule, TodoListItemNode>(
            [.. System.Controller.Modules],
            OnModuleSelected,
            list => {
                list.NodeReferenceData = panel;
                list.ItemSpacing = 4.0f;
                moduleList = list;
            });

    private void OnModuleSelected(TodoListItemNode todoOptionNode, IModule selectedModule) {
        if (panel is null) return;

        if (todoOptionNode.IsChecked) {
            panel.Modules.Add(selectedModule.DisplayName);
            panelNode?.Show();
        }
        else {
            panel.Modules.Remove(selectedModule.DisplayName);
        }

        config?.Save();
    }

    private void BuildFeaturesTab(NodeBuilder builder) => builder
        .Spacer(10.0f)
        .VerticalList(options => options
            .Scrolling(BuildScrollableFeatures, scrollingNode => {
                scrollingNode.LayoutFill = LayoutFill.Both;
                scrollingNode.AutoHideScrollBar = true;
            }),
            content => {
                content.LayoutFill = LayoutFill.Both;
                content.ItemSpacing = 8.0f;
            });

    private void BuildScrollableFeatures(NodeBuilder builder) => builder
        .Spacer(5.0f)
        .BuildAction(BuildPanelInfoSection)
        .Spacer(6.0f)
        .Separator()
        .Spacer(6.0f)
        .BuildAction(BuildAppearanceSection)
        .Spacer(6.0f)
        .Separator()
        .Spacer(6.0f)
        .BuildAction(BuildLayoutSection)
        .Spacer(6.0f)
        .Separator()
        .Spacer(6.0f)
        .BuildAction(BuildInteractionSection);

    private void BuildPanelInfoSection(NodeBuilder builder) => builder
        .CategoryLabel("Panel Info")
        .Spacer(2.0f)
        .Row("Display Label", right => right
            .InputText(newValue => {
                if (panel is null) return;

                panel.Label = newValue.IsEmpty ? Strings.TodoOverlay_NewPanel : newValue.ToString();
                panel.IsDirty = true;
                config?.Save();
                dropDownNode?.RefreshLabel();
            },
            out labelInput));

    private void BuildAppearanceSection(NodeBuilder builder) => builder
        .CategoryLabel("Appearance")
        .Spacer(2.0f)
        .Row("Text Color", right => right
            .ColorEdit4(ColorHelper.GetColor(1), ColorHelper.GetColor(1), newTextColor => {
                if (panel is null) return;

                panel.TextColor = newTextColor;
                panel.IsDirty = true;
                config?.Save();
            },
            out textColorEdit))
        .Row("Text Outline Color", right => right
            .ColorEdit4(ColorHelper.GetColor(53), ColorHelper.GetColor(53), newOutlineColor => {
                if (panel is null) return;

                panel.OutlineColor = newOutlineColor;
                panel.IsDirty = true;
                config?.Save();
            },
            out outlineColorEdit))
        .Row("Background Alpha", right => right
            .FloatSlider(1.0f, 0.0f, 1.0f, newAlpha => {
                if (panel is null) return;

                panel.Alpha = newAlpha;
                panel.ShowFrame = newAlpha > 0.0f;
                showFrameCheck.SetValueSilent(newAlpha > 0.0f);
                panel.IsDirty = true;
                config?.Save();
            },
            out alphaSlider))
        .Row(Strings.TodoOverlay_ButtonAlpha, right => right
            .FloatSlider(1.0f, 0.1f, 1.0f, newButtonAlpha => {
                if (panel is null) return;

                panel.ButtonAlpha = newButtonAlpha;
                panel.IsDirty = true;
                config?.Save();
            },
            out buttonAlphaSlider));

    private void BuildLayoutSection(NodeBuilder builder) => builder
        .CategoryLabel("Layout")
        .Spacer(2.0f)
        .Row("Scale", right => right
            .FloatSlider(1.0f, 0.5f, 2.0f, newScale => {
                if (panel is null) return;

                panel.Scale = newScale;
                panel.IsDirty = true;
                config?.Save();
            },
            out scaleSlider))
        .Row("Width", right => right
            .IntSlider(225, 150, 400, newWidth => {
                if (panel is null) return;

                panel.Width = newWidth;
                panel.IsDirty = true;
                config?.Save();
            },
            out widthSlider))
        .Row("List X Offset", right => right
            .IntSlider(18, 0, 100, newListXOffset => {
                if (panel is null) return;

                panel.ListXOffset = newListXOffset;
                panel.IsDirty = true;
                config?.Save();
            },
            out listXOffsetSlider))
        .Row("Item Spacing", right => right
            .IntSlider(0, -6, 20, newSpacing => {
                if (panel is null) return;

                panel.ItemSpacing = newSpacing;
                panel.IsDirty = true;
                config?.Save();
            },
            out spacingSlider))
        .Row(Strings.TodoOverlay_Alignment, right => right
            .Combo([VerticalListAlignment.Left, VerticalListAlignment.Right],
                VerticalListAlignment.Left, newAlignment => {
                    if (panel is null) return;

                    panel.Alignment = newAlignment;
                    panel.IsDirty = true;
                    config?.Save();
                },
                a => a == VerticalListAlignment.Left ? "Left" : "Right",
                out alignmentCombo))
        .Checkbox("Show Frame", true, newValue => {
            if (panel is null) return;

            panel.ShowFrame = newValue;
            panel.IsDirty = true;
            config?.Save();
        }, out showFrameCheck);

    private void BuildInteractionSection(NodeBuilder builder) => builder
        .CategoryLabel("Interaction")
        .Spacer(2.0f)
        .Checkbox(Strings.TodoOverlay_EnableMoving, false, newValue => {
            if (panel is null) return;

            panel.EnableMoving = newValue;
        }, out enableMovingCheck)
        .Checkbox("Auto-hide when all complete", false, newValue => {
            if (panel is null) return;

            panel.AutoHideWhenEmpty = newValue;
            if (!newValue) panelNode?.Show();
            else if (!panelNode?.HasVisibleItems() ?? false) panelNode?.Hide();
            config?.Save();
        }, out autoHideCheck)
        .Checkbox(Strings.TodoOverlay_PinToQuestList, false, newValue => {
            if (panel is null || config is null) return;

            // Only one panel can be pinned at a time
            if (newValue) {
                foreach (var panelConfig in config.Panels) {
                    if (panelConfig != panel) {
                        panelConfig.AttachToQuestList = false;
                    }
                }
            }
            panel.AttachToQuestList = newValue;
            config.Save();
        }, out attachToQuestListCheck)
        .Spacer(6.0f);

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        layoutContainer.Size = Size;
    }

    public void SetPanel(TodoPanelConfig? newPanel, TodoOverlayConfig? newConfig, TodoPanelNode? newPanelNode = null, IDropDownNode<TodoPanelConfig?>? newDropDownNode = null) {
        if (newPanel is null) {
            panel = null;
            config = null;
            panelNode = null;
            dropDownNode = null;

            optionNode!.IsVisible = false;
            warningNode!.IsVisible = true;
            return;
        }

        panel = newPanel;
        config = newConfig;
        panelNode = newPanelNode;
        dropDownNode = newDropDownNode;

        optionNode!.IsVisible = true;
        warningNode!.IsVisible = false;

        // Set panel info
        labelInput.SetValueSilent(panel.Label);

        // Update module list reference data
        if (moduleList is not null) {
            moduleList.NodeReferenceData = panel;
            moduleList.Update();
        }

        // Set appearance
        textColorEdit.CurrentColor = panel.TextColor;
        outlineColorEdit.CurrentColor = panel.OutlineColor;
        alphaSlider.SetValueSilent(panel.Alpha);
        buttonAlphaSlider.SetValueSilent(panel.ButtonAlpha);

        // Set layout
        scaleSlider.SetValueSilent(panel.Scale);
        widthSlider.SetValueSilent(panel.Width);
        listXOffsetSlider.SetValueSilent(panel.ListXOffset);
        spacingSlider.SetValueSilent(panel.ItemSpacing);
        alignmentCombo.SelectedOption = panel.Alignment;
        showFrameCheck.SetValueSilent(panel.ShowFrame);

        // Set interaction
        enableMovingCheck.SetValueSilent(panel.EnableMoving);
        autoHideCheck.SetValueSilent(panel.AutoHideWhenEmpty);
        attachToQuestListCheck.SetValueSilent(panel.AttachToQuestList);
    }
}
