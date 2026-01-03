using DailyDuty.CustomNodes;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace DailyDuty.Features.Levequest;

public unsafe class DataNode(Levequest module) : DataNodeBase<Levequest>(module) {

    private TextNode? allowancesNode;
    private TextNode? acceptedNode;
    
    protected override void BuildNode(ScrollingListNode container) {
        container.AddNode([
            new HorizontalFlexNode {
                Height = 28.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        String = "Levequests Available",
                        AlignmentType = AlignmentType.Left,
                    },
                    allowancesNode = new TextNode {
                        String = "Available Not Updated",
                        AlignmentType = AlignmentType.Left,
                    },
                ],
            },
            new HorizontalFlexNode {
                Height = 28.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        String = "Levequests Accepted",
                        AlignmentType = AlignmentType.Left,
                    },
                    acceptedNode = new TextNode {
                        String = "Accepted Not Updated",
                        AlignmentType = AlignmentType.Left,
                    },
                ],
            },
        ]);
    }

    public override void Update() {
        base.Update();

        allowancesNode?.String = QuestManager.Instance()->NumLeveAllowances.ToString();
        acceptedNode?.String = QuestManager.Instance()->NumAcceptedLeveQuests.ToString();
    }
}
