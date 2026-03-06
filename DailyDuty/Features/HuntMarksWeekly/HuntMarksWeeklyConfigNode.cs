using DailyDuty.CustomNodes;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;

namespace DailyDuty.Features.HuntMarksWeekly;

public class HuntMarksWeeklyConfigNode(HuntMarksWeekly module) : ConfigNodeBase<HuntMarksWeekly>(module) {
    private readonly HuntMarksWeekly module = module;
    
    protected override void BuildNode(ScrollingListNode container) {
        container.AddNode([
            new LuminaMultiSelectNode<MobHuntOrderType> {
                GetLabelFunc = item => item.EventItem.ValueNullable?.Name.ToString(),
                OnEdited = module.ModuleConfig.MarkDirty,
                FilterFunc = orderType => orderType.Type is 2,
                Options = module.ModuleConfig.TrackedHuntMarks,
                Height = 360.0f,
            },
        ]);
    }
}
