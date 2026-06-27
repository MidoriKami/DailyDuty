using DailyDuty.Classes;
using KamiToolKit.BaseTypes;

namespace DailyDuty.CustomNodes;

public class GenericConfigNodeBase(ModuleBase module) : ConfigNodeBase<ModuleBase>(module) {
    protected override NodeBase? BuildNode() => null;
}
