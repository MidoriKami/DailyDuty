using System.Collections.Generic;
using DailyDuty.CustomNodes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;

namespace DailyDuty.Features.RaidsAlliance;

public class RaidsAllianceDataNode(RaidsAlliance module) : DataNodeBase<RaidsAlliance>(module) {

    private readonly Dictionary<uint, TextNode> statusNodes = [];
    private readonly RaidsAlliance module = module;

    protected override NodeBase BuildDataNode() {
        var verticalListNode = new VerticalListNode {
            FitWidth = true,
        };
        
        foreach (var (cfc, _) in module.ModuleData.TaskStatus) {
            var text = Services.DataManager.GetExcelSheet<ContentFinderCondition>().GetRow(cfc).Name.ToString();

            TextNode statusNode;

            verticalListNode.AddNode(new HorizontalListNode {
                Height = 32.0f,
                FitHeight = true,
                InitialNodes = [
                    new TextNode {
                        Width = 325.0f,
                        TextFlags = TextFlags.Ellipsis,
                        AlignmentType = AlignmentType.Left,
                        String = text,
                    },
                    statusNode = new TextNode {
                        Width = 50.0f,
                        AlignmentType = AlignmentType.Right,
                        String = "Status not Updated",
                    },
                ],
            });

            statusNodes.Add(cfc, statusNode);
        }
        
        return verticalListNode;
    }

    public override void Update() {
        base.Update();

        foreach (var (index, statusNode) in statusNodes) {
            if (!module.ModuleData.TaskStatus.TryGetValue(index, out var taskComplete)) continue;

            statusNode.String = taskComplete ? "Complete" : "Incomplete";
        }
    }
}
