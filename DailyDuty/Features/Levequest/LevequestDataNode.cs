using DailyDuty.Classes;
using DailyDuty.CustomNodes;
using FFXIVClientStructs.FFXIV.Client.Game;
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
                        String = Strings.Levequests_Available,
                    },
                    allowancesNode = new TextNode {
                        Width = 100.0f,
                        String = Strings.Available_Not_Updated,
                    },
                ],
            },
            new HorizontalFlexNode {
                Height = 32.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        String = Strings.Levequests_Accepted,
                    },
                    acceptedNode = new TextNode {
                        String = Strings.Accepted_Not_Updated,
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
