using System.Numerics;
using DailyDuty.CustomNodes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;

namespace DailyDuty.Features.FauxHollows;

public class FauxHollowsDataNode(FauxHollows module) : DataNodeBase<FauxHollows>(module) {
    private readonly FauxHollows module = module;
    private TextNode? statusNode;

    protected override NodeBase BuildDataNode() => new VerticalListNode {
        FitWidth = true,
        InitialNodes = [
            new HorizontalFlexNode {
                Height = 32.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        Size = new Vector2(250.0f, 28.0f),
                        TextFlags = TextFlags.Ellipsis,
                        String = "Current Completion Count",
                    },
                    statusNode = new TextNode {
                        Size = new Vector2(100.0f, 28.0f),
                        String = "Status not Updated",
                    },
                ],
            },
        ],
    };

    public override void Update() {
        base.Update();

        statusNode?.String = module.ModuleData.FauxHollowsCompletions.ToString();
    }
}
