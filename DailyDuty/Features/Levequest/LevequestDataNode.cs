using DailyDuty.CustomNodes;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;

namespace DailyDuty.Features.Levequest;

public unsafe class LevequestDataNode(Levequest module) : DataNodeBase<Levequest>(module) {

    private TextNode? allowancesNode;
    private TextNode? acceptedNode;

    protected override NodeBase BuildDataNode() => new VerticalListNode {
        FitWidth = true,
        InitialNodes = [
            new HorizontalFlexNode {
                Height = 32.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        Width = 200.0f,
                        String = "Levequests Available",
                        AlignmentType = AlignmentType.Left,
                    },
                    allowancesNode = new TextNode {
                        Width = 100.0f,
                        String = "Available Not Updated",
                        AlignmentType = AlignmentType.Left,
                    },
                ],
            },
            new HorizontalFlexNode {
                Height = 32.0f,
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
        ],
    };

    public override void Update() {
        base.Update();

        allowancesNode?.String = QuestManager.Instance()->NumLeveAllowances.ToString();
        acceptedNode?.String = QuestManager.Instance()->NumAcceptedLeveQuests.ToString();
    }
}
