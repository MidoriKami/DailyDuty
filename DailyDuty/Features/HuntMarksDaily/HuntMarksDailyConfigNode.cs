using DailyDuty.CustomNodes;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;

namespace DailyDuty.Features.HuntMarksDaily;

public class HuntMarksDailyConfigNode(HuntMarksDaily module) : ConfigNodeBase<HuntMarksDaily>(module) {
    private readonly HuntMarksDaily module = module;

    protected override void BuildNode(ScrollingListNode container) {
        container.AddNode([
            new LuminaMultiSelectNode<MobHuntOrderType> {
                GetLabelFunc = item => item.EventItem.ValueNullable?.Name.ToString(),
                OnEdited = module.ModuleConfig.MarkDirty,
                FilterFunc = orderType => orderType.Type is 1,
                Options = module.ModuleConfig.TrackedHuntMarks,
                Height = 360.0f,
            },
        ]);
    }
}
