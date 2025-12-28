using KamiToolKit.Nodes;

namespace DailyDuty.Classes.Nodes;

public class GenericDataNodeBase(ModuleBase module) : DataNodeBase<ModuleBase>(module) {
    protected override void BuildNode(VerticalListNode container) { }
}
