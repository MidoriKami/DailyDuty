using System.Collections.Generic;
using DailyDuty.CustomNodes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;
using InstanceContent = FFXIVClientStructs.FFXIV.Client.Game.UI.InstanceContent;

namespace DailyDuty.Features.DutyRoulette;

public unsafe class DutyRouletteDataNode(DutyRoulette module) : DataNodeBase<DutyRoulette>(module) {
    
    private readonly Dictionary<uint, TextNode> statusNodes = [];
    
    protected override NodeBase BuildDataNode() {
        var verticalListNode = new VerticalListNode {
            FitWidth = true,
        };
        
        foreach (var roulette in Services.DataManager.GetExcelSheet<ContentRoulette>()) {
            if (roulette is not { Name.ByteLength: > 0, ContentRouletteRoleBonus.RowId: not 0 }) continue;
            TextNode statusNode;
            
            verticalListNode.AddNode(new HorizontalListNode {
                Height = 32.0f,
                FitHeight = true,
                InitialNodes = [
                    new TextNode {
                        Width = 325.0f,
                        TextFlags = TextFlags.Ellipsis,
                        String = roulette.Name.ToString(),
                    },
                    statusNode = new TextNode {
                        Width = 50.0f,
                        AlignmentType = AlignmentType.Right,
                        String = "Status not Updated",
                    },
                ],
            });

            statusNodes.Add(roulette.RowId, statusNode);
        }
        
        return verticalListNode;
    }

    public override void Update() {
        base.Update();
        
        foreach (var (rowId, textNode) in statusNodes) {
            var isComplete = InstanceContent.Instance()->IsRouletteComplete((byte) rowId);
    
            textNode.String = isComplete ? "Complete" : "Incomplete";
        }
    }
}
