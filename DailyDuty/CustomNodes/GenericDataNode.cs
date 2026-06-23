using Resources;
using System;
using DailyDuty.Classes;
using DailyDuty.Enums;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using KamiToolKit.Premade.Node.Simple;

namespace DailyDuty.CustomNodes;

public class GenericDataNode : SimpleComponentNode {
    private readonly VerticalListNode layoutContainerNode;

    private readonly TextNode statusTextNode;
    private readonly TextNode resetTimeTextNode;
    private readonly TextNode timeRemainingTextNode;

    public GenericDataNode() {
        layoutContainerNode = new VerticalListNode {
            FitWidth = true,
            InitialNodes = [
                new CategoryHeaderNode {
                    String = Strings.ResourceManager.GetString("Module Status", Strings.Culture) ?? "Module Status",
                    Alignment = AlignmentType.Bottom,
                },
                statusTextNode = new TextNode {
                    String = CompletionStatus.Unknown.Description,
                    AlignmentType = AlignmentType.Bottom,
                    Height = 32.0f,
                },
                new ResNode { Height = 50.0f },
                new CategoryHeaderNode {
                    String = Strings.ResourceManager.GetString("Next Reset", Strings.Culture) ?? "Next Reset",
                    Alignment = AlignmentType.Bottom,
                },
                resetTimeTextNode = new TextNode {
                    String = DateTime.UnixEpoch.ToLocalTime().GetDisplayString(),
                    AlignmentType = AlignmentType.Bottom,
                    Height = 32.0f,
                },
                new ResNode { Height = 50.0f },
                new CategoryHeaderNode {
                    String = Strings.ResourceManager.GetString("Time Remaining", Strings.Culture) ?? "Time Remaining",
                    Alignment = AlignmentType.Bottom,
                },
                timeRemainingTextNode = new TextNode {
                    String = TimeSpan.Zero.FormatTimespan(),
                    AlignmentType = AlignmentType.Bottom,
                    Height = 32.0f,
                },
            ],
        };
        layoutContainerNode.AttachNode(this);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        layoutContainerNode.Size = Size;
        layoutContainerNode.RecalculateLayout();
    }

    public void Update(ModuleBase module) {
        var resetTime = module.DataBase.NextReset;

        statusTextNode.String = module.ModuleStatus.Description;

        if (resetTime == DateTime.MaxValue) {
            resetTimeTextNode.String = Strings.ResourceManager.GetString("Available Now", Strings.Culture) ?? "Available Now";
            timeRemainingTextNode.String = "0.00:00:00";
        }
        else if (resetTime <= DateTime.UtcNow) {
            resetTimeTextNode.String = Strings.ResourceManager.GetString("Enable module to initialize", Strings.Culture) ?? "Enable module to initialize";
            timeRemainingTextNode.String = Strings.ResourceManager.GetString("Enable module to initialize", Strings.Culture) ?? "Enable module to initialize";
        }
        else {
            resetTimeTextNode.String = resetTime.ToLocalTime().GetDisplayString();
            timeRemainingTextNode.String = $"{(resetTime - DateTime.UtcNow).FormatTimespan()}";
        }
    }
}
