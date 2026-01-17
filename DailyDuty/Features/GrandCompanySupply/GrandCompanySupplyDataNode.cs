using System.Collections.Generic;
using DailyDuty.CustomNodes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;

namespace DailyDuty.Features.GrandCompanySupply;

public class GrandCompanySupplyDataNode(GrandCompanySupply module) : DataNodeBase<GrandCompanySupply>(module) {
    private readonly GrandCompanySupply module = module;

    private readonly Dictionary<uint, TextNode> statusNodes = [];

    protected override NodeBase BuildDataNode() {
        var container = new VerticalListNode {
            FitWidth = true,
        };

        foreach (var (job, _) in module.ModuleData.ClassJobStatus) {
            var classJob = Services.DataManager.GetExcelSheet<ClassJob>().GetRow(job);

            TextNode statusNode;
            
            container.AddNode(new HorizontalFlexNode {
                Height = 32.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        Width = 200.0f,
                        String = $"{classJob.NameEnglish}",
                        AlignmentType = AlignmentType.Left,
                        TextFlags = TextFlags.Ellipsis,
                    },
                    statusNode = new TextNode {
                        Width = 100.0f,
                        String = $"{classJob.NameEnglish} Data Not Set",
                        AlignmentType = AlignmentType.Left,
                    },
                ],
            });
            
            statusNodes.TryAdd(job, statusNode);
        }

        return container;
    }

    public override void Update() {
        base.Update();

        foreach (var (job, node) in statusNodes) {
            node.String = module.ModuleData.ClassJobStatus[job] ? "Complete" : "Incomplete";
        }
    }
}
