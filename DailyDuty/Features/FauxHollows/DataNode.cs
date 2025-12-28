using System.Numerics;
using DailyDuty.Classes.Nodes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;

namespace DailyDuty.Features.FauxHollows;

public class DataNode(FauxHollows module) : DataNodeBase<FauxHollows>(module) {
    private readonly FauxHollows module = module;
    private TextNode? statusNode;

    protected override void BuildNode(VerticalListNode container) {
        container.AddNode([
            new HorizontalListNode {
                FitToContentHeight = true,
                ItemSpacing = 4.0f,
                InitialNodes = [
                    new TextNode {
                        Size = new Vector2(250.0f, 28.0f),
                        TextFlags = TextFlags.Ellipsis,
                        AlignmentType = AlignmentType.Left,
                        String = "Current Completion Count",
                    },
                    statusNode = new TextNode {
                        Size = new Vector2(100.0f, 28.0f),
                        AlignmentType = AlignmentType.Left,
                        String = "Status not Updated",
                    },
                ],
            }
        ]);
    }

    public override void Update() {
        base.Update();

        statusNode?.String = module.ModuleData.FauxHollowsCompletions.ToString();
    }
}
