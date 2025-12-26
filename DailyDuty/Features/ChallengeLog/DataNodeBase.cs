using System.Collections.Generic;
using System.Numerics;
using DailyDuty.Classes.Nodes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;
using ContentsNoteModule = FFXIVClientStructs.FFXIV.Client.Game.UI.ContentsNote;

namespace DailyDuty.Features.ChallengeLog;

public class DataNode(ChallengeLog module) : DataNodeBase<ChallengeLog>(module) {

    private readonly Dictionary<uint, TextNode> statusNodes = [];
    
    protected override void BuildNode(VerticalListNode container) {
        foreach (var contentsRow in Services.DataManager.GetExcelSheet<ContentsNote>()) {
            if (contentsRow is not { Name.ByteLength: > 0 } ) continue;

            container.ItemSpacing = 6.0f;

            var listNode = new HorizontalListNode {
                FitToContentHeight = true,
                ItemSpacing = 4.0f,
            };
            
            listNode.AddNode(new IconImageNode {
                Size = new Vector2(28.0f, 28.0f),
                FitTexture = true,
                IconId = (uint)contentsRow.Icon,
            });
            
            listNode.AddNode(new TextNode {
                Size = new Vector2(225.0f, 28.0f),
                TextFlags = TextFlags.Ellipsis,
                AlignmentType = AlignmentType.Left,
                String = contentsRow.Name.ExtractText(),
            });

            var statusNode = new TextNode {
                Size = new Vector2(100.0f, 28.0f),
                AlignmentType = AlignmentType.Left,
                String = "Status not Updated",
            };
            
            listNode.AddNode(statusNode);
            container.AddNode(listNode);
            statusNodes.Add(contentsRow.RowId, statusNode);
        }
    }

    public override unsafe void Update() {
        base.Update();
        
        foreach (var (rowId, textNode) in statusNodes) {
            var isComplete = ContentsNoteModule.Instance()->IsContentNoteComplete((int)rowId);
    
            textNode.String = isComplete ? "Complete" : "Not Complete";
        }
    }
}
