using DailyDuty.CustomNodes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;

namespace DailyDuty.Features.MiniCactpot;

public class MiniCactpotDataNode(MiniCactpot module) : DataNodeBase<MiniCactpot>(module) {

    private TextNode? attemptsNode;
    private readonly MiniCactpot module = module;

    protected override NodeBase BuildDataNode() => new VerticalListNode {
        FitWidth = true,
        InitialNodes = [
            new HorizontalFlexNode {
                Height = 32.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        String = "Attempts Available",
                        AlignmentType = AlignmentType.Left,
                    },
                    attemptsNode = new TextNode {
                        String = "Attempts Not Updated",
                        AlignmentType = AlignmentType.Left,
                    },
                ],
            },
        ],
    };

    public override void Update() {
        base.Update();

        attemptsNode?.String = module.ModuleData.AllowancesRemaining.ToString();
    }
}
