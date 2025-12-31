using System.Collections.Generic;
using System.Linq;
using DailyDuty.Classes.Nodes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;

namespace DailyDuty.Features.MaskedCarnivale;

public class DataNode(MaskedCarnivale module) : DataNodeBase<MaskedCarnivale>(module) {

    private readonly Dictionary<int, TextNode> statusNodes = [];
    private readonly MaskedCarnivale module = module;

    protected override void BuildNode(VerticalListNode container) {
        foreach (var index in Enumerable.Range(12447, 3)) {

            var text = Services.DataManager.GetExcelSheet<Addon>().GetRow((uint)index).Text.ToString();

            TextNode statusNode;

            container.AddNode([
                new HorizontalFlexNode {
                    Height = 28.0f,
                    AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                    InitialNodes = [
                        new TextNode {
                            String = text,
                            AlignmentType = AlignmentType.Left,
                        },
                        statusNode = new TextNode {
                            AlignmentType = AlignmentType.Left,
                        },
                    ],
                },
            ]);

            statusNodes.Add(index, statusNode);
        }
    }

    public override void Update() {
        base.Update();

        foreach (var (index, statusNode) in statusNodes) {
            if (!module.ModuleData.TaskData.TryGetValue(index, out var taskComplete)) continue;

            statusNode.String = taskComplete ? "Complete" : "Incomplete";
        }
    }
}
