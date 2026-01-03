using System;
using DailyDuty.Classes;
using DailyDuty.Enums;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;

namespace DailyDuty.CustomNodes;

public class GenericDataNode : SimpleComponentNode {
    private readonly ScrollingListNode statusNode;
    
    private readonly TextNode statusTextNode;
    private readonly TextNode resetTimeTextNode;
    private readonly TextNode timeRemainingTextNode;

    public GenericDataNode() {
        statusNode = new ScrollingListNode {
            AutoHideScrollBar = true,
        };
        statusNode.FitWidth = true;
        statusNode.AttachNode(this);
        
        statusNode.AddNode([
            new CategoryHeaderNode {
                Label = "Module Status",
                Alignment = AlignmentType.Bottom,
            },
            statusTextNode = new TextNode {
                String = CompletionStatus.Unknown.Description,
                AlignmentType = AlignmentType.Bottom,
                Height = 32.0f,
            },
            new ResNode { Height = 50.0f },
            new CategoryHeaderNode {
                Label = "Next Reset",
                Alignment = AlignmentType.Bottom,
            },
            resetTimeTextNode = new TextNode {
                String = DateTime.UnixEpoch.ToLocalTime().GetDisplayString(),
                AlignmentType = AlignmentType.Bottom,
                Height = 32.0f,
            },
            new ResNode { Height = 50.0f },
            new CategoryHeaderNode {
                Label = "Time Remaining",
                Alignment = AlignmentType.Bottom,
            },
            timeRemainingTextNode = new TextNode {
                String = TimeSpan.Zero.FormatTimespan(),
                AlignmentType = AlignmentType.Bottom,
                Height = 32.0f,
            },
        ]);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        statusNode.Size = Size;
        statusNode.RecalculateLayout();
    }

    public void Update(ModuleBase module) {
        var resetTime = module.DataBase.NextReset;

        statusTextNode.String = module.ModuleStatus.Description;

        if (resetTime == DateTime.MaxValue) {
            resetTimeTextNode.String = "Available Now";
            timeRemainingTextNode.String = "0.00:00:00";
        }
        else {
            resetTimeTextNode.String = resetTime.ToLocalTime().GetDisplayString();
            timeRemainingTextNode.String = $"{(resetTime - DateTime.UtcNow).FormatTimespan()}";
        }
    }
}
