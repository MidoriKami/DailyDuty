using DailyDuty.CustomNodes;
using FFXIVClientStructs.FFXIV.Client.Game;
using KamiToolKit;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;

namespace DailyDuty.Features.CustomDelivery;

public unsafe class CustomDeliveryDataNode(CustomDelivery module) : DataNodeBase<CustomDelivery>(module) {

    private TextNode? allowancesTextNode;
    
    protected override NodeBase BuildDataNode() => new VerticalListNode {
        FitWidth = true,
        InitialNodes = [
            new HorizontalFlexNode {
                Height = 32.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        String = "Allowances Remaining",
                    },
                    allowancesTextNode = new TextNode {
                        String = "Allowances Not Updated",
                    },
                ],
            },
        ],
    };

    public override void Update() {
        base.Update();

        allowancesTextNode?.String = SatisfactionSupplyManager.Instance()->GetRemainingAllowances().ToString();
    }
}
