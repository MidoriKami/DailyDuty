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

        System.Controller.OnModuleStateChanged += OnModuleStateChanged;
    }

    protected override void Dispose(bool isNativeDestructor) {
        base.Dispose(isNativeDestructor);
        System.Controller.OnModuleStateChanged -= OnModuleStateChanged;
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        checkboxNode.Size = Size;
    }

    protected override void SetNodeData(IModule itemData) {
        checkboxNode.String = itemData.DisplayName;
        checkboxNode.IsEnabled = itemData.Enabled;

        checkboxNode.OnChecked = null;

        switch (ReferenceData) {
            case TimerOverlayConfig timerConfig:
                checkboxNode.IsChecked = timerConfig.SelectedModules.Contains(itemData.DisplayName);
                break;
        }

        checkboxNode.OnChecked = _ => OnStateChanged?.Invoke(this);
    }

    private void OnModuleStateChanged(IModule module, bool enabled) {
        if (module == ItemData) {
            checkboxNode.IsEnabled = enabled;
        }
    }
}
