using DailyDuty.Interfaces;
using KamiToolKit.Enums;
using KamiToolKit.Interfaces;
using KamiToolKit.Nodes;

namespace DailyDuty.Nodes.Module;

public class FeatureInfoNode(IFeature _) : ResNode, ILayoutFiller {
    public IFeature Unknown { get; } = _;

    /// <inheritdoc/>
    public LayoutFill LayoutFill { get; set; } = LayoutFill.Vertical;
}
