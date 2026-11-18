using DailyDuty.Interfaces;
using KamiToolKit.BaseTypes;
using KamiToolKit.Enums;
using KamiToolKit.Interfaces;
using KamiToolKit.Nodes;

namespace DailyDuty.Nodes.Module;

public class ConfigInfoNode : ResNode, INativeUpdatable, ILayoutFiller {

    /// <inheritdoc/>
    public LayoutFill LayoutFill { get; set; } = LayoutFill.Both;

    private NodeBase? attachedNode;

    public IToggleable? CurrentItem { get; private set; }

    public void SetModuleInfo(IModule module) {
        attachedNode?.Dispose();
        CurrentItem = module;

        attachedNode = new ModuleInfoNode(module) {
            Size = Size,
        };
        attachedNode.AttachNode(this);
    }

    public void SetFeatureInfo(IFeature feature) {
        attachedNode?.Dispose();
        CurrentItem = feature;

        attachedNode = new FeatureInfoNode(feature) {
            Size = Size,
        };
        attachedNode.AttachNode(this);
    }

    public void Clear() {
        attachedNode?.Dispose();
        attachedNode = null;
        CurrentItem = null;
    }

    public void Update() {
        if (attachedNode is INativeUpdatable updatable) {
            updatable.Update();
        }
    }
}
