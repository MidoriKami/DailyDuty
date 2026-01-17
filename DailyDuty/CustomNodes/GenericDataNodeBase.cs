using DailyDuty.Classes;
using KamiToolKit;
using KamiToolKit.Nodes;

namespace DailyDuty.CustomNodes;

public class GenericDataNodeBase(ModuleBase module) : DataNodeBase<ModuleBase>(module) {
    protected override NodeBase BuildDataNode()
        => new ResNode();
}
