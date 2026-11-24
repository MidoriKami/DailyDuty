using DailyDuty.Utilities;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Core.Interfaces;
using KamiToolKit.UI.Nodes;
using Lumina.Excel;

namespace DailyDuty.Nodes.Display;

public class LuminaStatusNode<T> : ListItemNode<T>, IListItemNode where T : struct, IExcelRow<T> {

    public static float ItemHeight => 24.0f;

    private readonly RatioSplitNode layoutNode;
    private readonly TextNode labelNode;
    private readonly TextNode statusNode;

    public LuminaStatusNode() {
        layoutNode = new RatioSplitNode {
            Ratios = [0.7f, 0.3f],
            ShowSeparators = false,
        };

        layoutNode.AddNode(labelNode = new TextNode {
            TextFlags = TextFlags.Ellipsis,
        });

        layoutNode.AddNode(statusNode = new TextNode {
            TextFlags = TextFlags.Ellipsis,
        });

        layoutNode.AttachNode(this);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        layoutNode.Size = Size;
    }

    protected override void SetNodeData(T itemData) {
        labelNode.String = itemData.DisplayString;
        labelNode.ItemTooltip = itemData.ItemId;

        statusNode.String = itemData.CompletionStatus.ToString();
    }

    public override void Update() {
        base.Update();

        statusNode.String = ItemData.CompletionStatus.ToString();
    }
}
