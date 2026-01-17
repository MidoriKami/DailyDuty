using System.Collections.Generic;
using System.Linq;
using DailyDuty.CustomNodes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;

namespace DailyDuty.Features.JumboCactpot;

public class JumboCactpotDataNode(JumpCactpot module) : DataNodeBase<JumpCactpot>(module) {

    private readonly Dictionary<int, TextNode> statusNodes = [];
    private readonly JumpCactpot module = module;

    protected override NodeBase BuildDataNode() {
        var verticalListNode = new VerticalListNode {
            FitWidth = true,
        };
        
        foreach (var index in Enumerable.Range(0, 3)) {

            TextNode statusNode;
            
            verticalListNode.AddNode([
                new HorizontalFlexNode {
                    Height = 32.0f,
                    AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                    InitialNodes = [
                        new TextNode {
                            String = $"Ticket #{index + 1}",
                            AlignmentType = AlignmentType.Left,
                        },
                        statusNode = new TextNode {
                            String = "Ticket Not Updated",
                            AlignmentType = AlignmentType.Left,
                        },
                    ],
                },
            ]);
            
            statusNodes.Add(index, statusNode);
        }
        
        return verticalListNode;
    }

    public override void Update() {
        base.Update();

        foreach (var (index, statusNode) in statusNodes) {
            if (module.ModuleData.Tickets.Count > index) {
                statusNode.String = module.ModuleData.Tickets[index].ToString();
            }
            else {
                statusNode.String = "Ticket not Claimed";
            }
        }
    }
}
