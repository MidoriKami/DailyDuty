using DailyDuty.Classes;
using KamiToolKit.Nodes;

namespace DailyDuty.CustomNodes;

public class GenericDataNodeBase(ModuleBase module) : DataNodeBase<ModuleBase>(module) {
    protected override void BuildNode(VerticalListNode container) { }
}
