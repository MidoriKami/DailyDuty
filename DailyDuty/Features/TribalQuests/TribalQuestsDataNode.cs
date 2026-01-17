using DailyDuty.CustomNodes;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;

namespace DailyDuty.Features.TribalQuests;

public unsafe class TribalQuestsDataNode(TribalQuests module) : DataNodeBase<TribalQuests>(module) {

    private TextNode? allowancesNode;

    protected override NodeBase BuildDataNode() => new VerticalListNode {
        FitWidth = true,
        InitialNodes = [
            new HorizontalFlexNode {
                Height = 32.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        String = "Tribal Quests Available",
                        AlignmentType = AlignmentType.Left,
                    },
                    allowancesNode = new TextNode {
                        String = "Available Not Updated",
                        AlignmentType = AlignmentType.Left,
                    },
                ],
            }
        ],
    };

    public override void Update() {
        base.Update();

        allowancesNode?.String = QuestManager.Instance()->GetBeastTribeAllowance().ToString();
    }
}
