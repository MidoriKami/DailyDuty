using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;

namespace DailyDuty.ListItemNodes;

public unsafe class MobHuntOrderTypeListItemNode : ListItemNode<MobHuntOrderType> {
    public override float ItemHeight => 32.0f;

    private readonly IconImageNode iconNode;
    private readonly TextNode labelNode;
    private readonly TextNode statusNode;

    public MobHuntOrderTypeListItemNode() {
        DisableInteractions();
        
        iconNode = new IconImageNode {
            FitTexture = true,
            ShowClickableCursor = true,
        };
        iconNode.AttachNode(this);
        
        labelNode = new TextNode {
            AlignmentType = AlignmentType.Left,
            TextFlags = TextFlags.Ellipsis,
        };
        labelNode.AttachNode(this);
        
        statusNode = new TextNode {
            AlignmentType = AlignmentType.Right,
        };
        statusNode.AttachNode(this);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        iconNode.Size = new Vector2(Height - 4.0f, Height - 4.0f);
        iconNode.Position = new Vector2(2.0f, 2.0f);

        statusNode.Size = new Vector2(75.0f, Height);
        statusNode.Position = new Vector2(Width - statusNode.Width, 0.0f);

        labelNode.Size = new Vector2(Width - iconNode.Width - statusNode.Width - 8.0f, Height);
        labelNode.Position = new Vector2(iconNode.Bounds.Right + 4.0f, 0.0f);
    }

    protected override void SetNodeData(MobHuntOrderType itemData) {
        iconNode.IconId = itemData.EventItem.Value.Icon;
        iconNode.ItemTooltip = itemData.EventItem.RowId;
        labelNode.String = itemData.EventItem.Value.Name;
        statusNode.String = MobHunt.Instance()->IsBillComplete((byte) itemData.RowId) ? "Complete" : "Incomplete";
    }

    public override void Update() {
        base.Update();

        statusNode.String = MobHunt.Instance()->IsBillComplete((byte) ItemData.RowId) ? "Complete" : "Incomplete";
    }
}
