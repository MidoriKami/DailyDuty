using System;
using System.Numerics;
using DailyDuty.Extensions;
using KamiToolKit;
using KamiToolKit.Nodes;

namespace DailyDuty.ConfigurationWindow.AutoConfig.ConfigEntries;

public class FloatSliderConfig : BaseConfigEntry {
    public required float MinValue { get; init; }
    public required float MaxValue { get; init; }
    public required float InitialValue { get; set; }
    public required int DecimalPlaces { get; init; }
    public required float StepSpeed { get; init; }
    
    public override NodeBase BuildNode() {
        var layoutNode = new HorizontalListNode {
            Height = 24.0f,
            ItemSpacing = 40.0f,
        };

        var sliderNode = new SliderNode {
            Size = new Vector2(175.0f, 24.0f),
            Position = new Vector2(0.0f, 4.0f),
            DecimalPlaces = DecimalPlaces,
            Range = (int)(MinValue * Math.Pow(10, DecimalPlaces))..(int)(MaxValue * Math.Pow(10, DecimalPlaces)),
            OnValueChanged = newValue => OnOptionChanged(newValue / MathF.Pow(10, DecimalPlaces)),
            Value = (int)(InitialValue * Math.Pow(10, DecimalPlaces)),
            Step = (int)(StepSpeed * MathF.Pow(10, DecimalPlaces)),
        };

        layoutNode.AddNode(sliderNode);
        layoutNode.AddNode(GetLabelNode());

        return layoutNode;
    }

    private void OnOptionChanged(float newValue) {
        InitialValue = newValue;
        MemberInfo.SetValue(Config, newValue);
        Config.Save();
    }
}
