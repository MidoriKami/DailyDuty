using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;

namespace DailyDuty.Features.ChallengeLog;

public sealed unsafe class StatusNode : SimpleComponentNode {
    private readonly TextNode label;
    private readonly TextNode status;

    public required ContentsNote Data {
        get;
        init {
            field = value;
            label.String = value.Name.ExtractText();
        }
    }

    public StatusNode() {
        label = new TextNode {
            TextFlags = TextFlags.Ellipsis,
        };
        label.AttachNode(this);
        
        status = new TextNode();
        status.AttachNode(this);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        label.Size = new Vector2(Width / 2.0f, Height);
        label.Position = new Vector2(0.0f, 0.0f);
        
        status.Size = new Vector2(Width / 2.0f, Height);
        status.Position = new Vector2(Width / 2.0f, 0.0f);
    }

    public void Update() {
        var isComplete = FFXIVClientStructs.FFXIV.Client.Game.UI.ContentsNote.Instance()->IsContentNoteComplete((int)Data.RowId);
        
        status.String = isComplete ? "Complete" : "Not Complete";
    }
}
