using DailyDuty.Classes.Nodes;
using KamiToolKit.Nodes;

namespace DailyDuty.Features.SampleModule;

public class ConfigNode(SampleModule module) : ConfigNodeBase<SampleModule>(module) {
    private readonly SampleModule module = module;
    
    protected override void BuildNode(VerticalListNode container) {
        container.AddNode([
            
        ]);
    }
}
