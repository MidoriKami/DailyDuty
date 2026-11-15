using DailyDuty.Interfaces;
using KamiToolKit.BaseTypes;
using KamiToolKit.Enums;
using KamiToolKit.Interfaces;
using KamiToolKit.Nodes;

namespace DailyDuty.Nodes;

public class ConfigInfoNode : ResNode, INativeUpdatable, ILayoutFiller {

    /// <inheritdoc/>
    public LayoutFill LayoutFill { get; set; } = LayoutFill.Both;

    private NodeBase? attachedNode;

    public void SetModuleInfo(IModule module) {
        attachedNode?.Dispose();

        attachedNode = new ModuleInfoNode(module) {
            Size = Size,
        };
        attachedNode.AttachNode(this);
    }

    public void SetFeatureInfo(IFeature feature) {
        attachedNode?.Dispose();

        attachedNode = new FeatureInfoNode(feature) {
            Size = Size,
        };
        attachedNode.AttachNode(this);
    }

    public void Clear() {
        attachedNode?.Dispose();
        attachedNode = null;
    }

    public void Update() {
        if (attachedNode is INativeUpdatable updatable) {
            updatable.Update();
        }
    }
}
