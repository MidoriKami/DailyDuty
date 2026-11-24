using DailyDuty.Features;
using DailyDuty.Interfaces;
using KamiToolKit.Core.Interfaces;
using KamiToolKit.UI.Nodes;

namespace DailyDuty.Nodes.Overlay;

public class TodoListItemNode : ListItemNode<IModule>, IListItemNode {

    public static float ItemHeight => 24.0f;

    private readonly CheckboxNode checkboxNode;

    public bool IsChecked => checkboxNode.IsChecked;

    public TodoListItemNode() {
        DisableInteractions();

        checkboxNode = new CheckboxNode {
            OnChecked = _ => OnStateChanged?.Invoke(this),
        };
        checkboxNode.AttachNode(this);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        checkboxNode.Size = Size;
    }

    protected override void SetNodeData(IModule itemData) {
        checkboxNode.String = itemData.DisplayName;

        switch (ReferenceData) {
            case TodoPanelConfig config:
                checkboxNode.SetValueSilent(config.Modules.Contains(itemData.DisplayName));
                break;
        }
    }
}
