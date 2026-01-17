using System.Linq;
using DailyDuty.CustomNodes;
using DailyDuty.ListItemNodes;
using KamiToolKit;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;

namespace DailyDuty.Features.ChallengeLog;

public class ChallengeLogDataNode(ChallengeLog module) : DataNodeBase<ChallengeLog>(module) {

    private ListNode<ContentsNote, ContentsNoteListItemNode>? listNode;
    
    protected override NodeBase BuildDataNode()
        => listNode = new ListNode<ContentsNote, ContentsNoteListItemNode> {
            OptionsList = Services.DataManager.GetExcelSheet<ContentsNote>()
                .Where(row => row is { RowId: not 0, Name.IsEmpty: false })
                .ToList(),
            ItemSpacing = 1.0f,
        };

    public override void Update() {
        base.Update();
        
        listNode?.Update();
    }
}
