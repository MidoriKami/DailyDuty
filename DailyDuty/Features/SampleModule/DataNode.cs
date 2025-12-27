using DailyDuty.Classes.Nodes;
using KamiToolKit.Nodes;

namespace DailyDuty.Features.SampleModule;

public class DataNode(SampleModule module) : DataNodeBase<SampleModule>(module) {

    protected override void BuildNode(VerticalListNode container) {
        container.AddNode([
            
        ]);
    }

    public override unsafe void Update() {
        base.Update();

        
    }
}
