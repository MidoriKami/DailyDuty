using DailyDuty.Features;
using DailyDuty.Interfaces;
using KamiToolKit.Core.Interfaces;
using KamiToolKit.UI.Nodes;

namespace DailyDuty.Nodes.Overlay;

public class TimerOverlayListItemNode : ListItemNode<IModule>, IListItemNode {

    public static float ItemHeight => 24.0f;

    private readonly CheckboxNode checkboxNode;

    public bool IsChecked => checkboxNode.IsChecked;

    public TimerOverlayListItemNode() {
        DisableInteractions();

        checkboxNode = new CheckboxNode {
            OnChecked = _ => OnStateChanged?.Invoke(this),
        };
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

        switch (ReferenceData) {
            case TimerOverlayConfig timerConfig:
                checkboxNode.SetValueSilent(timerConfig.SelectedModules.Contains(itemData.DisplayName));
                break;
        }
    }

    private void OnModuleStateChanged(IModule module, bool enabled) {
        if (module == ItemData) {
            checkboxNode.IsEnabled = enabled;
        }
    }
}
