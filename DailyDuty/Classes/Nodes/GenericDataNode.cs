using System;
using DailyDuty.Enums;
using DailyDuty.Extensions;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;

namespace DailyDuty.Classes.Nodes;

public class GenericDataNode : SimpleComponentNode {
    private readonly TabbedVerticalListNode listNode;
    
    private readonly TextNode statusTextNode;
    private readonly TextNode resetTimeTextNode;
    private readonly TextNode timeRemainingTextNode;

    private readonly bool isReady;
    
    public GenericDataNode() {
        listNode = new TabbedVerticalListNode {
            FitWidth = true,
        };
        listNode.AttachNode(this);
        
        listNode.AddNode(new ResNode { Height = 32.0f });
        
        listNode.AddNode(new CategoryTextNode {
            String = "Module Status",
            AlignmentType = AlignmentType.Bottom,
            Height = 24.0f,
        });
        
        listNode.AddNode(new HorizontalLineNode {
            Height = 4.0f,
        });
        
        listNode.AddNode(statusTextNode = new TextNode {
            String = CompletionStatus.Unknown.Description,
            AlignmentType = AlignmentType.Bottom,
            Height = 32.0f,
        });
        
        listNode.AddNode(new ResNode { Height = 64.0f });

        listNode.AddNode(new CategoryTextNode {
            String = "Module Reset",
            AlignmentType = AlignmentType.Bottom,
            Height = 24.0f,
        });

        listNode.AddNode(new HorizontalLineNode {
            Height = 4.0f,
        });
        
        listNode.AddNode(1, resetTimeTextNode = new TextNode {
            String = DateTime.UnixEpoch.ToLocalTime().GetDisplayString(),
            AlignmentType = AlignmentType.BottomLeft,
            Height = 32.0f,
        });
        
        listNode.AddNode(1, timeRemainingTextNode = new TextNode {
            String = TimeSpan.Zero.FormatTimespan(),
            AlignmentType = AlignmentType.BottomLeft,
            Height = 32.0f,
        });
        
        isReady = true;
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        listNode.Size = Size;
        listNode.RecalculateLayout();
    }

    public void Update(ModuleBase module) {
        if (!isReady) return;
        
        var resetTime = module.DataBase.NextReset;

        statusTextNode.String = module.ModuleStatus.Description;
        resetTimeTextNode.String = resetTime.ToLocalTime().GetDisplayString();
        timeRemainingTextNode.String = $"Time Remaining: {(resetTime - DateTime.UtcNow).FormatTimespan()}";
    }
}
