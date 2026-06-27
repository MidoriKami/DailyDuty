using DailyDuty.CustomNodes;
using FFXIVClientStructs.FFXIV.Client.Game;
using KamiToolKit.BaseTypes;
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
                        String = Strings.Levequest_Available,
                    },
                    allowancesNode = new TextNode {
                        Width = 100.0f,
                        String = Strings.FashionReport_AvailableNotUpdated,
                    },
                ],
            },
            new HorizontalFlexNode {
                Height = 32.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        String = Strings.Levequest_Accepted,
                    },
                    acceptedNode = new TextNode {
                        String = Strings.Levequest_AcceptedNotUpdated,
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
