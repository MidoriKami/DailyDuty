using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;
using ContentsNoteModule = FFXIVClientStructs.FFXIV.Client.Game.UI.ContentsNote;

namespace DailyDuty.ListItemNodes;

public unsafe class ContentsNoteListItemNode : ListItemNode<ContentsNote> {
    public override float ItemHeight => 32.0f;

    private readonly IconImageNode iconNode;
    private readonly TextNode labelNode;
    private readonly TextNode statusNode;

    public ContentsNoteListItemNode() {
        DisableInteractions();
        
        iconNode = new IconImageNode {
            FitTexture = true,
        };
        iconNode.AttachNode(this);
        
        labelNode = new TextNode {
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

    protected override void SetNodeData(ContentsNote itemData) {
        iconNode.IconId = (uint) itemData.Icon;
        labelNode.String = itemData.Name;
        statusNode.String = "Unknown Status";
    }

    public override void Update() {
        base.Update();

        statusNode.String = ContentsNoteModule.Instance()->IsContentNoteComplete((int)ItemData.RowId) ? "Complete" : "Incomplete";
    }
}
