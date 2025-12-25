using System;
using System.Numerics;
using DailyDuty.Extensions;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Nodes;

namespace DailyDuty.ConfigurationWindow.AutoConfig.ConfigEntries;

public class IntSliderConfig : BaseConfigEntry {
    public required Range Range { get; init; }
    public required int InitialValue { get; set; }

    public override NodeBase BuildNode() {
        var layoutNode = new HorizontalListNode {
            Height = 24.0f,
            ItemSpacing = 20.0f,
        };

        var sliderNode = new SliderNode {
            Size = new Vector2(175.0f, 24.0f),
            Range = Range,
            OnValueChanged = OnOptionChanged,
            Value = InitialValue,
        };

        var labelNode = new CategoryTextNode {
            Size = new Vector2(0.0f, 24.0f),
            String = Label,
            AlignmentType = AlignmentType.TopLeft,
        };
            
        layoutNode.AddNode(sliderNode);
        layoutNode.AddNode(labelNode);

        return layoutNode;
    }

    private void OnOptionChanged(int newValue) {
        InitialValue = newValue;
        MemberInfo.SetValue(Config, newValue);
        Config.Save();
    }
}
