using System.Collections.Generic;
using DailyDuty.CustomNodes;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using Dalamud.Excel.Sheets;

namespace DailyDuty.Features.GrandCompanySupply;

public class GrandCompanySupplyDataNode(GrandCompanySupply module) : DataNodeBase<GrandCompanySupply>(module) {
    private readonly GrandCompanySupply module = module;

    private readonly Dictionary<uint, TextNode> statusNodes = [];

    protected override NodeBase BuildDataNode() {
        var container = new VerticalListNode {
            FitWidth = true,
        };

        foreach (var (job, _) in module.ModuleData.ClassJobStatus) {
            var classJob = IDataManager.Get().GetExcelSheet<ClassJob>().GetRow(job);

            TextNode statusNode;

            container.AddNode(new HorizontalFlexNode {
                Height = 32.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        Width = 200.0f,
                        String = $"{classJob.NameEnglish}",
                        TextFlags = TextFlags.Ellipsis,
                    },
                    statusNode = new TextNode {
                        Width = 100.0f,
                        String = $"{classJob.NameEnglish} {Strings.Additional_DataNotSet}",
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
            node.String = module.ModuleData.ClassJobStatus[job] ? Strings.CompletionStatus_Complete : Strings.CompletionStatus_Incomplete;
        }
    }
}
