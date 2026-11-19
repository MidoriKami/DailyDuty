using DailyDuty.Features;
using DailyDuty.Interfaces;
using KamiToolKit.Interfaces;
using KamiToolKit.Nodes;

namespace DailyDuty.Nodes.Overlay;

public class TimerOverlayListItemNode : ListItemNode<IModule>, IListItemNode {

    public static float ItemHeight => 24.0f;

    private readonly CheckboxNode checkboxNode;

    public bool IsChecked => checkboxNode.IsChecked;

    public TimerOverlayListItemNode() {
        DisableInteractions();

        checkboxNode = new CheckboxNode();
        checkboxNode.AttachNode(this);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        checkboxNode.Size = Size;
    }

    protected override void SetNodeData(IModule itemData) {
        checkboxNode.String = itemData.DisplayName;
        checkboxNode.OnChecked = null;
        checkboxNode.IsEnabled = itemData.Enabled;

        switch (ReferenceData) {
            case TimerOverlayConfig timerConfig:
                checkboxNode.IsChecked = timerConfig.SelectedModules.Contains(itemData.DisplayName);
                break;
        }

        checkboxNode.OnChecked = _ => OnStateChanged?.Invoke(this);
    }

    public override void Update() {
        base.Update();

        checkboxNode.IsEnabled = ItemData?.Enabled == true;
    }
}
