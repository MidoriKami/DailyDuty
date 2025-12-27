using DailyDuty.Classes.Nodes;
using KamiToolKit.Nodes;

namespace DailyDuty.Features.DomanEnclave;

public class ConfigNode(DomanEnclave module) : ConfigNodeBase<DomanEnclave>(module) {
    protected override void BuildNode(VerticalListNode container) { }
}
