using DailyDuty.CustomNodes;
using KamiToolKit.BaseTypes;
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
                        String = Strings.MiniCactpot_AttemptsAvailable,
                    },
                    attemptsNode = new TextNode {
                        String = Strings.MiniCactpot_AttemptsNotUpdated,
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
