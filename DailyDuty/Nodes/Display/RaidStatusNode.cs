using DailyDuty.Classes;
using DailyDuty.Enums;
using Lumina.Excel.Sheets;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Interfaces;
using KamiToolKit.Nodes;

namespace DailyDuty.Nodes.Display;

/// <summary>
/// List item node for displaying a tracked raid with its completion status.
/// </summary>
public class RaidStatusNode : ListItemNode<RaidEntry>, IListItemNode {

    public static float ItemHeight => 24.0f;

    private readonly RatioSplitNode layoutNode;
    private readonly TextNode labelNode;
    private readonly TextNode statusNode;

    public RaidStatusNode() {
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
        layoutNode.UpdateLayout();
    }

    protected override void SetNodeData(RaidEntry itemData) {
        var cfc = IDataManager.Get()
            .GetExcelSheet<ContentFinderCondition>()
            .GetRow(itemData.Id);

        labelNode.String = cfc.Name;
        labelNode.ItemTooltip = 0u;

        statusNode.String = itemData.Complete ? CompletionStatus.Complete.ToString() : CompletionStatus.Incomplete.ToString();
    }

    public override void Update() {
        base.Update();

        statusNode.String = ItemData!.Complete ? CompletionStatus.Complete.ToString() : CompletionStatus.Incomplete.ToString();
    }
}
