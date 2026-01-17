using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using Lumina.Text.ReadOnly;

namespace DailyDuty.CustomNodes;

public sealed class CategoryHeaderNode : SimpleComponentNode {
    private readonly CategoryTextNode labelNode;
    private readonly HorizontalLineNode lineNode;
    
    public CategoryHeaderNode() {
        labelNode = new CategoryTextNode {
            AlignmentType = AlignmentType.BottomLeft,
        };
        labelNode.AttachNode(this);

        lineNode = new HorizontalLineNode();
        lineNode.AttachNode(this);

        Height = 40.0f;
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();
        
        labelNode.Size = new Vector2(Width, Height - 16.0f);
        labelNode.Position = new Vector2(0.0f, 4.0f);
        
        lineNode.Size = new Vector2(Width, 4.0f);
        lineNode.Position = new Vector2(0.0f, labelNode.Height + 4.0f);
    }

    public required ReadOnlySeString String {
        get => labelNode.String;
        set => labelNode.String = value;
    }
    
    public AlignmentType Alignment {
        get => labelNode.AlignmentType;
        set => labelNode.AlignmentType = value;
    }
}
