using System.Collections.Generic;
using DailyDuty.Classes.Nodes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;

namespace DailyDuty.Features.RaidsNormal;

public class DataNode(RaidsNormal module) : DataNodeBase<RaidsNormal>(module) {

    private readonly Dictionary<uint, TextNode> statusNodes = [];
    private readonly RaidsNormal module = module;

    protected override void BuildNode(VerticalListNode container) {
        foreach (var (cfc, _) in module.ModuleData.TaskStatus) {
            var text = Services.DataManager.GetExcelSheet<ContentFinderCondition>().GetRow(cfc).Name.ToString();

            TextNode statusNode;

            container.AddNode([
                new HorizontalFlexNode {
                    Height = 28.0f,
                    AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                    InitialNodes = [
                        new TextNode {
                            TextFlags = TextFlags.Ellipsis,
                            String = text,
                            AlignmentType = AlignmentType.Left,
                        },
                        statusNode = new TextNode {
                            AlignmentType = AlignmentType.Left,
                        },
                    ],
                },
            ]);

            statusNodes.Add(cfc, statusNode);
        }
    }

    public override void Update() {
        base.Update();

        foreach (var (index, statusNode) in statusNodes) {
            if (!module.ModuleData.TaskStatus.TryGetValue(index, out var taskComplete)) continue;

            statusNode.String = taskComplete ? "Complete" : "Incomplete";
        }
    }
}
