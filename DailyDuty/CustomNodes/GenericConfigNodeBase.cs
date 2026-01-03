using DailyDuty.Classes;
using KamiToolKit.Nodes;

namespace DailyDuty.CustomNodes;

public class GenericConfigNodeBase(ModuleBase module) : ConfigNodeBase<ModuleBase>(module) {
    protected override void BuildNode(ScrollingListNode container) { }
}
