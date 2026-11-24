using System.Drawing;
using System.Numerics;
using DailyDuty.Enums;
using DailyDuty.Features;
using DailyDuty.Interfaces;
using Dalamud.Interface;
using KamiToolKit.Controls.Builders;
using KamiToolKit.Core.Enums;
using KamiToolKit.Core.Layout;
using KamiToolKit.UI.Nodes;
using KamiToolKit.UI.Nodes.ComponentNode;
using KamiToolKit.UI.Nodes.Layout;

namespace DailyDuty.Nodes.Overlay;

public class TimerEditNode : ResNode, ILayoutFiller {

    public LayoutFill LayoutFill { get; set; } = LayoutFill.Both;

    private readonly ILayoutNode layoutContainer;
    private VerticalListNode? optionNode;

    private TextInputNode labelInput = null!;
    private ColorEditNode colorEdit = null!;
    private DropDownNode<TimerCountDirection> directionCombo = null!;
    private ColorEditNode textColorEdit = null!;
    private SliderNode fontSizeSlider = null!;
    private NumericInputNode widthInput = null!;
    private NumericInputNode heightInput = null!;
    private FloatSliderNode scaleSlider = null!;
    private CheckboxNode showLabelCheck = null!;
    private CheckboxNode showCountdownCheck = null!;
    private CheckboxNode hideSecondsCheck = null!;
    private CheckboxNode hideExpiredCheck = null!;
    private CheckboxNode enableMovingCheck = null!;

    private TextNode? warningNode;

    private IModule? module;
    private TimerOverlayConfig? config;

    private TimerOverlayTimerData? TimerData {
        get {
            if (module is null || config is null) return null;

            var displayName = module.DisplayName;
            if (!config.TimerData.TryGetValue(displayName, out var data)) {
                data = new TimerOverlayTimerData { Label = displayName };
                config.TimerData[displayName] = data;
            }

            return data;
        }
    }

    public TimerEditNode() {
        layoutContainer = NativeUiBuilder.Create(builder => builder
            .FittedContent(fittedBuilder => fittedBuilder
                .VerticalList(options => options
                        .Scrolling(BuildScrollableContent, scrollingNode => {
                            scrollingNode.LayoutFill = LayoutFill.Both;
                            scrollingNode.AutoHideScrollBar = true;
                        }),
                    out optionNode,
                    content => {
                        content.LayoutFill = LayoutFill.Both;
                        content.ItemSpacing = 8.0f;
                        content.IsVisible = false;
                    })
                .TextWarningFilled("Select a timer to configure", out warningNode)
            ));

        layoutContainer.AttachNode(this);
    }

    private void BuildScrollableContent(NodeBuilder builder) => builder
        .BuildAction(BuildTimerInfoSection)
        .Spacer(6.0f)
        .Separator()
        .Spacer(6.0f)
        .BuildAction(BuildAppearanceSection)
        .Spacer(6.0f)
        .Separator()
        .Spacer(6.0f)
        .BuildAction(BuildSizeScaleSection)
        .Spacer(6.0f)
        .Separator()
        .Spacer(6.0f)
        .BuildAction(BuildVisibilitySection)
        .Spacer(6.0f)
        .Separator()
        .Spacer(6.0f)
        .BuildAction(BuildInteractionSection);

    private void BuildTimerInfoSection(NodeBuilder builder) => builder
        .Spacer(6.0f)
        .CategoryLabel("Timer Info")
        .Spacer(2.0f)
        .Row("Display Label", right => right
            .InputText(newValue => {
                if (TimerData is null) return;

                TimerData.Label = newValue.IsEmpty ? module?.DisplayName ?? string.Empty : newValue.ToString();
                TimerData.IsDirty = true;
                config?.Save();
            },
            out labelInput));

    private void BuildAppearanceSection(NodeBuilder builder) => builder
        .CategoryLabel("Appearance")
        .Spacer(2.0f)
        .Row("Background Color", right => right
            .ColorEdit4(Vector4.Zero, KnownColor.CornflowerBlue.Vector(), newColor => {
                if (TimerData is null) return;

                TimerData.Color = newColor;
                TimerData.IsDirty = true;
                config?.Save();
            },
            out colorEdit))
        .Row("Text Color", right => right
            .ColorEdit4(Vector4.Zero, KnownColor.White.Vector(), newTextColor => {
                if (TimerData is null) return;

                TimerData.TextColor = newTextColor;
                TimerData.IsDirty = true;
                config?.Save();
            },
            out textColorEdit))
        .Row("Font Size", right => right
            .IntSlider(16, 8, 48, newFontSize => {
                if (TimerData is null) return;

                TimerData.FontSize = newFontSize;
                TimerData.IsDirty = true;
                config?.Save();
            },
            out fontSizeSlider))
        .Row("Timer Mode", right => right
            .Combo([TimerCountDirection.CountDown, TimerCountDirection.CountUp], TimerCountDirection.CountDown, newDirection => {
                if (TimerData is null) return;

                TimerData.CountDirection = newDirection;
                TimerData.IsDirty = true;
                config?.Save();
            },
            d => d == TimerCountDirection.CountDown ? "Remaining Time" : "Elapsed Time",
            out directionCombo));

    private void BuildSizeScaleSection(NodeBuilder builder) => builder
        .CategoryLabel("Size & Scale")
        .Spacer(2.0f)
        .Row("Width", right => right
            .InputInt(300, 100, 800, newWidth => {
                if (TimerData is null) return;

                TimerData.Width = newWidth;
                TimerData.IsDirty = true;
                config?.Save();
            },
            out widthInput))
        .Row("Height", right => right
            .InputInt(64, 24, 256, newHeight => {
                if (TimerData is null) return;

                TimerData.Height = newHeight;
                TimerData.IsDirty = true;
                config?.Save();
            },
            out heightInput))
        .Row("Scale", right => right
            .FloatSlider(1.0f, 0.5f, 2.0f, newScale => {
                if (TimerData is null) return;

                TimerData.Scale = newScale;
                TimerData.IsDirty = true;
                config?.Save();
            },
            out scaleSlider));

    private void BuildVisibilitySection(NodeBuilder builder) => builder
        .CategoryLabel("Visibility")
        .Spacer(2.0f)
        .Checkbox("Show Text Label", true, newValue => {
            if (TimerData is null) return;

            TimerData.ShowLabel = newValue;
            TimerData.IsDirty = true;
            config?.Save();
        }, out showLabelCheck)
        .Checkbox("Show Countdown Timer", true, newValue => {
            if (TimerData is null) return;

            TimerData.ShowCountdown = newValue;
            TimerData.IsDirty = true;
            config?.Save();
        }, out showCountdownCheck)
        .Checkbox("Hide Seconds", false, newValue => {
            if (TimerData is null) return;

            TimerData.HideSeconds = newValue;
            TimerData.IsDirty = true;
            config?.Save();
        }, out hideSecondsCheck)
        .Checkbox("Hide when Completed", false, newValue => {
            if (TimerData is null) return;

            TimerData.HideWhenCompleted = newValue;
            TimerData.IsDirty = true;
            config?.Save();
        }, out hideExpiredCheck);

    private void BuildInteractionSection(NodeBuilder builder) => builder
        .CategoryLabel("Interaction")
        .Spacer(2.0f)
        .Checkbox("Enable Dragging", true, newValue => {
            if (TimerData is null) return;

            TimerData.EnableMoving = newValue;
            TimerData.IsDirty = true;
        }, out enableMovingCheck)
        .Spacer(6.0f);

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        layoutContainer.Size = Size;
    }

    public void SetModule(IModule? newModule, TimerOverlayConfig newConfig) {
        if (newModule is null) {
            module = null;
            config = null;

            optionNode!.IsVisible = false;
            warningNode!.IsVisible = true;
            return;
        }

        module = newModule;
        config = newConfig;

        optionNode!.IsVisible = true;
        warningNode!.IsVisible = false;

        if (TimerData is null) return;

        labelInput.SetValueSilent(TimerData.Label ?? string.Empty);
        colorEdit.CurrentColor = TimerData.Color;
        directionCombo.SelectedOption = TimerData.CountDirection;
        textColorEdit.CurrentColor = TimerData.TextColor;
        fontSizeSlider.SetValueSilent(TimerData.FontSize);
        widthInput.SetValueSilent(TimerData.Width ?? 300);
        heightInput.SetValueSilent(TimerData.Height ?? 64);
        scaleSlider.SetValueSilent(TimerData.Scale);
        showLabelCheck.SetValueSilent(TimerData.ShowLabel);
        showCountdownCheck.SetValueSilent(TimerData.ShowCountdown);
        hideSecondsCheck.SetValueSilent(TimerData.HideSeconds);
        hideExpiredCheck.SetValueSilent(TimerData.HideWhenCompleted);
        enableMovingCheck.SetValueSilent(TimerData.EnableMoving);
    }
}
