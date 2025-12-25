using System.Numerics;
using DailyDuty.Extensions;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using KamiToolKit.Premade.Addons;
using KamiToolKit.Premade.Nodes;

namespace DailyDuty.ConfigurationWindow.AutoConfig.ConfigEntries;

public class ColorConfig : BaseConfigEntry {
    public required Vector4 Color { get; set; }
    public Vector4? DefaultColor { get; init; }

    private readonly ColorPickerAddon colorPickerInstance = new() {
        InternalName = "ColorPicker",
        Title = "Color Picker",
    };

    public override NodeBase BuildNode() {
        var layoutNode = new HorizontalListNode {
            Size = new Vector2(100.0f, 24.0f),
            ItemSpacing = 10.0f,
        };

        var colorSquareNode = new ColorPreviewNode {
            Size = new Vector2(24.0f, 24.0f),
            Color = Color,
        };
        layoutNode.AddNode(colorSquareNode);

        layoutNode.CollisionNode.DrawFlags |= DrawFlags.ClickableCursor;
        layoutNode.CollisionNode.AddEvent(AtkEventType.MouseClick, () => {
            colorPickerInstance.InitialColor = Color;
            colorPickerInstance.DefaultColor = DefaultColor;
            colorPickerInstance.Toggle();
        });
        
        colorPickerInstance.OnColorConfirmed = newValue => {
            colorSquareNode.Color = newValue;
            Color = newValue;
            MemberInfo.SetValue(Config, newValue);
            Config.Save();
        };

        layoutNode.AddNode(GetLabelNode());
        
        return layoutNode;
    }

    public override void Dispose()
        => colorPickerInstance.Dispose();
}
