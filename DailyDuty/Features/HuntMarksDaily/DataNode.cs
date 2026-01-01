using System.Collections.Generic;
using System.Numerics;
using DailyDuty.CustomNodes;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;

namespace DailyDuty.Features.HuntMarksDaily;

public class DataNode(HuntMarksDaily module) : DataNodeBase<HuntMarksDaily>(module) {
    
    private readonly Dictionary<uint, TextNode> statusNodes = [];
    
    protected override void BuildNode(VerticalListNode container) {
        foreach (var contentsRow in Services.DataManager.GetExcelSheet<MobHuntOrderType>()) {
            if (contentsRow is not { EventItem.ValueNullable.Name.ByteLength: > 0 } ) continue;
            if (contentsRow.Type is not 1) continue;
            
            container.ItemSpacing = 6.0f;

            TextNode statusNode;
            
            container.AddNode(new HorizontalListNode {
                FitToContentHeight = true,
                ItemSpacing = 4.0f,
                InitialNodes = [
                    new IconImageNode {
                        Size = new Vector2(28.0f, 28.0f),
                        FitTexture = true,
                        IconId = contentsRow.EventItem.Value.Icon,
                        ShowClickableCursor = true,
                        ItemTooltip = contentsRow.EventItem.RowId,
                    },
                    new TextNode {
                        Size = new Vector2(225.0f, 28.0f),
                        TextFlags = TextFlags.Ellipsis,
                        AlignmentType = AlignmentType.Left,
                        String = contentsRow.EventItem.Value.Name.ToString(),
                    },
                    statusNode = new TextNode {
                        Size = new Vector2(100.0f, 28.0f),
                        AlignmentType = AlignmentType.Left,
                        String = "Status not Updated",
                    },
                ],
            });

            statusNodes.Add(contentsRow.RowId, statusNode);
        }
    }

    public override unsafe void Update() {
        base.Update();
        
        foreach (var (rowId, textNode) in statusNodes) {
            var isComplete = MobHunt.Instance()->IsBillComplete((byte) rowId);
    
            textNode.String = isComplete ? "Complete" : "Not Complete";
        }
    }
}
