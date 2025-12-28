using DailyDuty.Classes.Nodes;
using KamiToolKit.Nodes;

namespace DailyDuty.Features.FauxHollows;

public class ConfigNode(FauxHollows module) : ConfigNodeBase<FauxHollows>(module) {
    private readonly FauxHollows module = module;

    protected override void BuildNode(VerticalListNode container) {
        container.AddNode([
            new CheckboxNode {
                Height = 24.0f,
                String = "Include Retelling",
                IsChecked = module.ModuleConfig.IncludeRetelling,
                OnClick = newValue => {
                    module.ModuleConfig.IncludeRetelling = newValue;
                    module.ModuleConfig.SavePending = true;
                },
            },
        ]);
    }
}
