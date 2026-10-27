using System.Linq;
using DailyDuty.CustomNodes;
using DailyDuty.ListItemNodes;
using Dalamud.Plugin.Services;
using KamiToolKit.BaseTypes;
using KamiToolKit.Nodes;
using Dalamud.Excel.Sheets;

namespace DailyDuty.Features.HuntMarksWeekly;

public class HuntMarksWeeklyDataNode(HuntMarksWeekly module) : DataNodeBase<HuntMarksWeekly>(module) {

    private ListNode<MobHuntOrderType, MobHuntOrderTypeListItemNode>? listNode;

    protected override NodeBase BuildDataNode()
        => listNode = new ListNode<MobHuntOrderType, MobHuntOrderTypeListItemNode> {
            OptionsList = [
                .. IDataManager.Get().GetExcelSheet<MobHuntOrderType>()
                    .Where(row => row is { RowId: not 0, EventItem.ValueNullable.Name.ByteLength: > 0, Type: 2 }),
            ],
            ItemSpacing = 1.0f,
        };

    public override void Update() {
        base.Update();

        listNode?.Update();
    }
}
