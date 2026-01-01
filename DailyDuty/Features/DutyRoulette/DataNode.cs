using System.Collections.Generic;
using System.Numerics;
using DailyDuty.CustomNodes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;
using InstanceContent = FFXIVClientStructs.FFXIV.Client.Game.UI.InstanceContent;

namespace DailyDuty.Features.DutyRoulette;

public unsafe class DataNode(DutyRoulette module) : DataNodeBase<DutyRoulette>(module) {
    
    private readonly Dictionary<uint, TextNode> statusNodes = [];
    
    protected override void BuildNode(VerticalListNode container) {
        foreach (var roulette in Services.DataManager.GetExcelSheet<ContentRoulette>()) {
            if (roulette is not { Name.ByteLength: > 0, ContentRouletteRoleBonus.RowId: not 0 }) continue;
            
            container.ItemSpacing = 6.0f;

            TextNode statusNode;
            
            container.AddNode(new HorizontalListNode {
                FitToContentHeight = true,
                ItemSpacing = 4.0f,
                InitialNodes = [
                    new TextNode {
                        Size = new Vector2(250.0f, 28.0f),
                        TextFlags = TextFlags.Ellipsis,
                        AlignmentType = AlignmentType.Left,
                        String = roulette.Name.ToString(),
                    },
                    statusNode = new TextNode {
                        Size = new Vector2(100.0f, 28.0f),
                        AlignmentType = AlignmentType.Left,
                        String = "Status not Updated",
                    },
                ],
            });

            statusNodes.Add(roulette.RowId, statusNode);
        }
    }

    public override void Update() {
        base.Update();
        
        foreach (var (rowId, textNode) in statusNodes) {
            var isComplete = InstanceContent.Instance()->IsRouletteComplete((byte) rowId);
    
            textNode.String = isComplete ? "Complete" : "Not Complete";
        }
    }
}
