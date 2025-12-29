using DailyDuty.Classes.Nodes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace DailyDuty.Features.MiniCactpot;

public class DataNode(MiniCactpot module) : DataNodeBase<MiniCactpot>(module) {

    private TextNode? attemptsNode;
    private readonly MiniCactpot module = module;

    protected override void BuildNode(VerticalListNode container) {
        container.AddNode([
            new HorizontalFlexNode {
                Height = 28.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        String = "Attemptes Available",
                        AlignmentType = AlignmentType.Left,
                    },
                    attemptsNode = new TextNode {
                        String = "Attempts Not Updated",
                        AlignmentType = AlignmentType.Left,
                    },
                ],
            },
        ]);
    }

    public override void Update() {
        base.Update();

        attemptsNode?.String = module.ModuleData.AllowancesRemaining.ToString();
    }
}
