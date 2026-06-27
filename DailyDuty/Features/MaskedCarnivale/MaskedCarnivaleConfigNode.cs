using DailyDuty.CustomNodes;
using KamiToolKit.BaseTypes;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;

namespace DailyDuty.Features.MaskedCarnivale;

public class MaskedCarnivaleConfigNode(MaskedCarnivale module) : ConfigNodeBase<MaskedCarnivale>(module) {
    private readonly MaskedCarnivale module = module;

    protected override NodeBase BuildNode() => new VerticalListNode {
        FitWidth = true,
        ItemSpacing = 4.0f,
        InitialNodes = [
            new LuminaMultiSelectNode<Addon> {
                GetLabelFunc = item => item.Text.ToString(),
                OnEdited = module.ModuleConfig.MarkDirty,
                FilterFunc = item => item.RowId is >= 12447 and <= 12449,
                Options = module.ModuleConfig.TrackedTasks,
                Height = 360.0f,
            },
        ],
    };
}
