using DailyDuty.CustomNodes;
using KamiToolKit.BaseTypes;
using KamiToolKit.Nodes;

namespace DailyDuty.Features.FauxHollows;

public class FauxHollowsConfigNode(FauxHollows module) : ConfigNodeBase<FauxHollows>(module) {
    private readonly FauxHollows module = module;

    protected override NodeBase BuildNode() => new VerticalListNode {
        FitWidth = true,
        ItemSpacing = 4.0f,
        InitialNodes = [
            new CheckboxNode {
                Height = 28.0f,
                String = Strings.FauxHollows_IncludeRetelling,
                IsChecked = module.ModuleConfig.IncludeRetelling,
                OnClick = newValue => {
                    module.ModuleConfig.IncludeRetelling = newValue;
                    module.ModuleConfig.MarkDirty();
                },
            },
        ],
    };
}
