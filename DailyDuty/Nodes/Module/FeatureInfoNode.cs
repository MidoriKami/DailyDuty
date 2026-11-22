using DailyDuty.Interfaces;
using KamiToolKit.Components.Builders;
using KamiToolKit.Nodes;

namespace DailyDuty.Nodes.Module;

public class FeatureInfoNode : ResNode, INativeUpdatable {
    private readonly IFeature feature;

    private readonly VerticalListNode layoutContainer;

    public FeatureInfoNode(IFeature feature) {
        this.feature = feature;

        layoutContainer = NativeUiBuilder.Create(buildAction => buildAction
            .TextCentered(feature.DisplayName, 14)
            .Separator()
            .Dummy(1.0f)
            .BuildAction(BuildFeatureUi));

        layoutContainer.AttachNode(this);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        layoutContainer.Size = Size;
    }

    public void Update()
        => feature.UpdateConfigUi();

    private void BuildFeatureUi(NodeBuilder builder) => builder
        .VerticalList(feature.BuildConfigUi, node => {
            node.ItemSpacing = 4.0f;
        });
}
