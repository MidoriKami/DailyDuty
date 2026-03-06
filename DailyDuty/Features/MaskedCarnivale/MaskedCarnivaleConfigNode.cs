using DailyDuty.CustomNodes;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;

namespace DailyDuty.Features.MaskedCarnivale;

public class MaskedCarnivaleConfigNode(MaskedCarnivale module) : ConfigNodeBase<MaskedCarnivale>(module){
    private readonly MaskedCarnivale module = module;
    
    protected override void BuildNode(ScrollingListNode container) {
        container.AddNode([
            new LuminaMultiSelectNode<Addon> {
                GetLabelFunc = item => item.Text.ToString(),
                OnEdited = module.ModuleConfig.MarkDirty,
                FilterFunc = item => item.RowId is >= 12447 and <= 12449,
                Options = module.ModuleConfig.TrackedTasks,
                Height = 360.0f,
            },
        ]);   
    }
}
