using System;
using DailyDuty.Enums;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;

namespace DailyDuty.Classes.Nodes;

public class GenericDataNode : SimpleComponentNode {
    private readonly ScrollingAreaNode<TabbedVerticalListNode> statusNode;
    
    private readonly TextNode statusTextNode;
    private readonly TextNode resetTimeTextNode;
    private readonly TextNode timeRemainingTextNode;

    public GenericDataNode() {
        statusNode = new ScrollingAreaNode<TabbedVerticalListNode> {
            ContentHeight = 1000.0f,
            AutoHideScrollBar = true,
        };
        statusNode.ContentNode.FitWidth = true;
        statusNode.AttachNode(this);
        
        statusNode.ContentNode.AddNode(new CategoryHeaderNode {
            Label = "Module Status",
            Alignment = AlignmentType.Bottom,
        });
        
        statusNode.ContentNode.AddNode(statusTextNode = new TextNode {
            String = CompletionStatus.Unknown.Description,
            AlignmentType = AlignmentType.Bottom,
            Height = 32.0f,
        });
        
        statusNode.ContentNode.AddNode(new ResNode { Height = 64.0f });

        statusNode.ContentNode.AddNode(new CategoryHeaderNode {
            Label = "Module Reset",
            Alignment = AlignmentType.Bottom,
        });
        
        statusNode.ContentNode.AddNode(1, resetTimeTextNode = new TextNode {
            String = DateTime.UnixEpoch.ToLocalTime().GetDisplayString(),
            AlignmentType = AlignmentType.BottomLeft,
            Height = 32.0f,
        });
        
        statusNode.ContentNode.AddNode(1, timeRemainingTextNode = new TextNode {
            String = TimeSpan.Zero.FormatTimespan(),
            AlignmentType = AlignmentType.BottomLeft,
            Height = 32.0f,
        });
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        statusNode.Size = Size;
        statusNode.ContentNode.RecalculateLayout();
    }

    public void Update(ModuleBase module) {
        var resetTime = module.DataBase.NextReset;

        statusTextNode.String = module.ModuleStatus.Description;
        resetTimeTextNode.String = resetTime.ToLocalTime().GetDisplayString();
        timeRemainingTextNode.String = $"Time Remaining: {(resetTime - DateTime.UtcNow).FormatTimespan()}";
    }
}
