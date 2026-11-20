using System.Drawing;
using System.Numerics;
using DailyDuty.Classes;
using DailyDuty.Enums;
using DailyDuty.Features;
using DailyDuty.Interfaces;
using Dalamud.Interface;
using KamiToolKit.Components.Builders;
using KamiToolKit.Enums;
using KamiToolKit.Interfaces;
using KamiToolKit.Nodes;

namespace DailyDuty.Nodes.Overlay;

public class TimerEditNode : ResNode, ILayoutFiller {

    public LayoutFill LayoutFill { get; set; } = LayoutFill.Both;

    private readonly VerticalListNode layoutContainer;
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

    private void BuildScrollableContent(NodeBuilder builder) {
        builder.ItemSpacing = 6.0f;
        builder
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
    }

    private void BuildTimerInfoSection(NodeBuilder builder) => builder
        .Spacer(6.0f)
        .CategoryLabel("Timer Info")
        .Spacer(2.0f)
        .Row("Display Label", right => right
            .InputText(_ => { }, out labelInput));

    private void BuildAppearanceSection(NodeBuilder builder) => builder
        .CategoryLabel("Appearance")
        .Spacer(2.0f)
        .Row("Background Color", right => right
            .ColorEdit4(Vector4.Zero, KnownColor.CornflowerBlue.Vector(), _ => { }, out colorEdit))
        .Row("Text Color", right => right
            .ColorEdit4(Vector4.Zero, KnownColor.White.Vector(), _ => { }, out textColorEdit))
        .Row("Font Size", right => right
            .IntSlider(16, 8, 48, _ => { }, out fontSizeSlider))
        .Row("Timer Mode", right => right
            .Combo([TimerCountDirection.CountDown, TimerCountDirection.CountUp], TimerCountDirection.CountDown, _ => { }, d => d == TimerCountDirection.CountDown ? "Remaining Time" : "Elapsed Time", out directionCombo));

    private void BuildSizeScaleSection(NodeBuilder builder) => builder
        .CategoryLabel("Size & Scale")
        .Spacer(2.0f)
        .Row("Width", right => right
            .InputInt(300, 100, 800, _ => { }, out widthInput))
        .Row("Height", right => right
            .InputInt(64, 24, 256, _ => { }, out heightInput))
        .Row("Scale", right => right
            .FloatSlider(1.0f, 0.5f, 2.0f, _ => { }, out scaleSlider));

    private void BuildVisibilitySection(NodeBuilder builder) => builder
        .CategoryLabel("Visibility")
        .Spacer(2.0f)
        .Checkbox("Show Text Label", true, _ => { }, out showLabelCheck)
        .Checkbox("Show Countdown Timer", true, _ => { }, out showCountdownCheck)
        .Checkbox("Hide Seconds", false, _ => { }, out hideSecondsCheck)
        .Checkbox("Hide when Completed", false, _ => { }, out hideExpiredCheck);

    private void BuildInteractionSection(NodeBuilder builder) => builder
        .CategoryLabel("Interaction")
        .Spacer(2.0f)
        .Checkbox("Enable Dragging", true, _ => { }, out enableMovingCheck)
        .Spacer(6.0f);

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        layoutContainer.Size = Size;
        layoutContainer.UpdateLayout();
    }

    public void SetModule(IModule? module, TimerOverlayConfig cfg) {
        if (module is null) {
            optionNode!.IsVisible = false;
            warningNode!.IsVisible = true;
            return;
        }

        optionNode!.IsVisible = true;
        warningNode!.IsVisible = false;

        // Get or create timer data for this module
        var displayName = module.DisplayName;
        if (!cfg.TimerData.TryGetValue(displayName, out var timerData)) {
            timerData = new TimerOverlayTimerData {
                Label = displayName,
            };
            cfg.TimerData[displayName] = timerData;
        }

        // Update all edit controls with current values
        labelInput.OnInputReceived = null;
        labelInput.String = timerData.Label ?? string.Empty;
        labelInput.OnInputReceived = newValue => {
            timerData.Label = newValue.IsEmpty ? module.DisplayName : newValue.ToString();
            timerData.IsDirty = true;
            cfg.Save();
        };

        colorEdit.OnColorConfirmed = null;
        colorEdit.CurrentColor = timerData.Color;
        colorEdit.OnColorConfirmed = newColor => {
            timerData.Color = newColor;
            timerData.IsDirty = true;
            cfg.Save();
        };

        directionCombo.OnOptionSelected = null;
        directionCombo.SelectedOption = timerData.CountDirection;
        directionCombo.OnOptionSelected = newDirection => {
            timerData.CountDirection = newDirection;
            timerData.IsDirty = true;
            cfg.Save();
        };

        textColorEdit.OnColorConfirmed = null;
        textColorEdit.CurrentColor = timerData.TextColor;
        textColorEdit.OnColorConfirmed = newTextColor => {
            timerData.TextColor = newTextColor;
            timerData.IsDirty = true;
            cfg.Save();
        };

        fontSizeSlider.OnValueUpdate = null;
        fontSizeSlider.Value = timerData.FontSize;
        fontSizeSlider.OnValueUpdate = newFontSize => {
            timerData.FontSize = newFontSize;
            timerData.IsDirty = true;
            cfg.Save();
        };

        widthInput.OnValueUpdate = null;
        widthInput.Value = timerData.Width ?? 300;
        widthInput.OnValueUpdate = newWidth => {
            timerData.Width = newWidth;
            timerData.IsDirty = true;
            cfg.Save();
        };

        heightInput.OnValueUpdate = null;
        heightInput.Value = timerData.Height ?? 64;
        heightInput.OnValueUpdate = newHeight => {
            timerData.Height = newHeight;
            timerData.IsDirty = true;
            cfg.Save();
        };

        scaleSlider.OnValueUpdate = null;
        scaleSlider.Value = timerData.Scale;
        scaleSlider.OnValueUpdate = newScale => {
            timerData.Scale = newScale;
            timerData.IsDirty = true;
            cfg.Save();
        };

        showLabelCheck.OnChecked = null;
        showLabelCheck.IsChecked = timerData.ShowLabel;
        showLabelCheck.OnChecked = newShowLabel => {
            timerData.ShowLabel = newShowLabel;
            cfg.Save();
        };

        showCountdownCheck.OnChecked = null;
        showCountdownCheck.IsChecked = timerData.ShowCountdown;
        showCountdownCheck.OnChecked = newShowCountdown => {
            timerData.ShowCountdown = newShowCountdown;
            cfg.Save();
        };

        hideSecondsCheck.OnChecked = null;
        hideSecondsCheck.IsChecked = timerData.HideSeconds;
        hideSecondsCheck.OnChecked = newHideSeconds => {
            timerData.HideSeconds = newHideSeconds;
            cfg.Save();
        };

        hideExpiredCheck.OnChecked = null;
        hideExpiredCheck.IsChecked = timerData.HideWhenCompleted;
        hideExpiredCheck.OnChecked = newHideWhenCompleted => {
            timerData.HideWhenCompleted = newHideWhenCompleted;
            cfg.Save();
        };

        enableMovingCheck.OnChecked = null;
        enableMovingCheck.IsChecked = timerData.EnableMoving;
        enableMovingCheck.OnChecked = newEnableMoving => {
            timerData.EnableMoving = newEnableMoving;
            timerData.IsDirty = true;
            cfg.Save();
        };
    }
}
