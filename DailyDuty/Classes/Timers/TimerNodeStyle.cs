using System.Drawing;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes.NodeStyles;

namespace DailyDuty.Classes.Timers;

public class TimerNodeStyle : NodeBaseStyle {
    public ProgressBarNodeStyle ProgressBarNodeStyle = new() {
        Size = new Vector2(400.0f, 48.0f),
        NodeFlags = NodeFlags.Visible,
        BackgroundColor = KnownColor.Black.Vector(),
        BarColor = KnownColor.Aqua.Vector(),
        BaseDisable = BaseStyleDisable.Size | BaseStyleDisable.NodeFlags | BaseStyleDisable.Color | BaseStyleDisable.Margin,
    };
    
    public TextNodeStyle ModuleNameStyle = new() {
        Position = new Vector2(12.0f, -24.0f),
        NodeFlags = NodeFlags.Visible,
        FontType = FontType.Jupiter,
        FontSize = 24,
        TextFlags = TextFlags.AutoAdjustNodeSize | TextFlags.Bold | TextFlags.Edge,
        BaseDisable = BaseStyleDisable.Size | BaseStyleDisable.NodeFlags | BaseStyleDisable.Color | BaseStyleDisable.Margin,
        TextStyleDisable = TextStyleDisable.LineSpacing | TextStyleDisable.TextFlags2 | TextStyleDisable.BackgroundColor,
    };
    
    public TextNodeStyle TimerTextStyle = new() {
        NodeFlags = NodeFlags.Visible | NodeFlags.AnchorRight,
        TextFlags = TextFlags.AutoAdjustNodeSize | TextFlags.Bold | TextFlags.Edge,
        FontType = FontType.Axis,
        FontSize = 24,
        CharacterSpacing = 0,
        Position = new Vector2(250.0f, -22.0f),
        BaseDisable = BaseStyleDisable.Size | BaseStyleDisable.NodeFlags | BaseStyleDisable.Color | BaseStyleDisable.Margin,
        TextStyleDisable = TextStyleDisable.LineSpacing | TextStyleDisable.TextFlags2 | TextStyleDisable.BackgroundColor,
    };
    
    public override bool DrawSettings() {
        var configChanged = false;

        using (ImRaii.TabBar("option_tabs")) {
            using (var baseTab = ImRaii.TabItem("Base")) {
                if (baseTab) {
                    configChanged |= base.DrawSettings();
                }
            }

            using (var progressBarTab = ImRaii.TabItem("Progress Bar")) {
                if (progressBarTab) {
                    configChanged |= ProgressBarNodeStyle.DrawSettings();
                }
            }

            using (var moduleNameTab = ImRaii.TabItem("Module Name")) {
                if (moduleNameTab) {
                    configChanged |= ModuleNameStyle.DrawSettings();
                }
            }

            using (var timerTextTab = ImRaii.TabItem("Timer Text")) {
                if (timerTextTab) {
                    configChanged |= TimerTextStyle.DrawSettings();
                }
            }
        }

        return configChanged;
    }
}