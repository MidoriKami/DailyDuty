using KamiToolKit.Nodes;

namespace DailyDuty.Classes.Nodes;

public class GenericConfigNodeBase(ModuleBase module) : ConfigNodeBase<ModuleBase>(module) {
    protected override void BuildNode(VerticalListNode container) { }
}
