using DailyDuty.Classes.Nodes;
using KamiToolKit.Nodes;

namespace DailyDuty.Features.GrandCompanyProvision;

public class ConfigNode(GrandCompanyProvision module) : ConfigNodeBase<GrandCompanyProvision>(module) {
    private readonly GrandCompanyProvision module = module;

    protected override void BuildNode(VerticalListNode container) {
        container.AddNode([
            new CheckboxNode {
                String = "Miner",
                Height = 24.0f,
                IsChecked = module.ModuleConfig.MinerEnabled,
                OnClick = newValue => {
                    module.ModuleConfig.MinerEnabled = newValue;
                    module.ModuleConfig.MarkDirty();
                },
            },
            new CheckboxNode {
                String = "Botanist",
                Height = 24.0f,
                IsChecked = module.ModuleConfig.BotanistEnabled,
                OnClick = newValue => {
                    module.ModuleConfig.BotanistEnabled = newValue;
                    module.ModuleConfig.MarkDirty();
                },
            },
            new CheckboxNode {
                String = "Fisher",
                Height = 24.0f,
                IsChecked = module.ModuleConfig.FisherEnabled,
                OnClick = newValue => {
                    module.ModuleConfig.FisherEnabled = newValue;
                    module.ModuleConfig.MarkDirty();
                },
            },
        ]);
    }
}
