using System.Collections.Generic;
using System.Numerics;
using KamiToolKit;
using KamiToolKit.Nodes;

namespace DailyDuty.ConfigurationWindow.AutoConfig.ConfigEntries;

public class MultiSelectIconConfig : SelectIconConfig {
    public required List<uint> Options { get; init; }
    public required bool AllowManualInput { get; init; }

    public override NodeBase BuildNode() {
        var iconSelectNode = base.BuildNode();
        
        var verticalLayoutNode = new VerticalListNode {
            Height = AllowManualInput ? iconSelectNode.Height + 48.0f : 48.0f,
        };

        var horizontalLayoutNode = new HorizontalListNode {
            Height = 48.0f,
        };
        verticalLayoutNode.AddNode(horizontalLayoutNode);

        foreach (var option in Options) {
            var buttonNode = new IconButtonNode {
                Size = new Vector2(48.0f, 48.0f),
                IconId = option,
                OnClick = () => SetIconId((int)option),
            };
            horizontalLayoutNode.AddNode(buttonNode);
        }

        if (AllowManualInput) {
            verticalLayoutNode.AddNode(iconSelectNode);
        }
        
        return verticalLayoutNode;
    }
}
