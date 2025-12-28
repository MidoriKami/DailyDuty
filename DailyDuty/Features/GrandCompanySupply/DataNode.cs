using System.Collections.Generic;
using DailyDuty.Classes.Nodes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;

namespace DailyDuty.Features.GrandCompanySupply;

public class DataNode(GrandCompanySupply module) : DataNodeBase<GrandCompanySupply>(module) {
    private readonly GrandCompanySupply module = module;

    private readonly Dictionary<uint, TextNode> statusNodes = [];
    
    protected override void BuildNode(VerticalListNode container) {
        foreach (var (job, _) in module.ModuleData.ClassJobStatus) {
            var classJob = Services.DataManager.GetExcelSheet<ClassJob>().GetRow(job);

            TextNode statusNode;
            
            container.AddNode(new HorizontalListNode {
                Height = 24.0f,
                FitHeight = true,
                InitialNodes = [
                    new TextNode {
                        Width = 200.0f,
                        String = $"{classJob.NameEnglish}",
                        AlignmentType = AlignmentType.Left,
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
    }

    public override void Update() {
        base.Update();

        foreach (var (job, node) in statusNodes) {
            node.String = module.ModuleData.ClassJobStatus[job] ? "Completed" : "Not Completed";
        }
    }
}
