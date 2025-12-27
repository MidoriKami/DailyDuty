using System.Numerics;
using DailyDuty.Classes.Nodes;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;

namespace DailyDuty.Features.CustomDelivery;

public unsafe class DataNode(CustomDelivery module) : DataNodeBase<CustomDelivery>(module) {

    private TextNode? allowancesTextNode;
    
    protected override void BuildNode(VerticalListNode container) {
        container.AddNode(new HorizontalListNode {
            FitToContentHeight = true,
            ItemSpacing = 4.0f,
            InitialNodes = [
                new TextNode {
                    Size = new Vector2(225.0f, 28.0f),
                    String = "Allowances Remaining",
                    AlignmentType = AlignmentType.Left,
                    Height = 32.0f,
                },
                allowancesTextNode = new TextNode {
                    Size = new Vector2(225.0f, 32.0f),
                    AlignmentType = AlignmentType.Left,
                    String = "Allowances Not Updated",
                },
            ],
        });
    }

    public override void Update() {
        base.Update();

        allowancesTextNode?.String = SatisfactionSupplyManager.Instance()->GetRemainingAllowances().ToString();
    }
}
