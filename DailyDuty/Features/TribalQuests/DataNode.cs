using DailyDuty.CustomNodes;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace DailyDuty.Features.TribalQuests;

public unsafe class DataNode(TribalQuests module) : DataNodeBase<TribalQuests>(module) {

    private TextNode? allowancesNode;
    
    protected override void BuildNode(VerticalListNode container) {
        container.AddNode([
            new HorizontalFlexNode {
                Height = 28.0f,
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
            },
        ]);
    }

    public override void Update() {
        base.Update();

        allowancesNode?.String = QuestManager.Instance()->GetBeastTribeAllowance().ToString();
    }
}
