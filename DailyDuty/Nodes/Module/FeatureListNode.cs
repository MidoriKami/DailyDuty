using System.Numerics;
using System.Threading.Tasks;
using DailyDuty.Enums;
using DailyDuty.Interfaces;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Core.Interfaces;
using KamiToolKit.UI.Nodes;
using KamiToolKit.UI.Nodes.ComponentNode;

namespace DailyDuty.Nodes.Module;

public class FeatureListNode : ListItemNode<IFeature>, IListItemNode {

    public static float ItemHeight => 28.0f;
    private readonly CheckboxNode checkboxNode;
    private readonly IconImageNode erroringImageNode;
    private readonly TextNode moduleNameNode;

    public FeatureListNode() {
        checkboxNode = new CheckboxNode {
            OnChecked = ToggleFeature,
        };
        checkboxNode.AttachNode(this);

        erroringImageNode = new IconImageNode {
            IconId = 61502,
        };
        erroringImageNode.AttachNode(this);

        moduleNameNode = new TextNode {
            FontSize = 14,
            TextFlags = TextFlags.Ellipsis,
        };
        moduleNameNode.AttachNode(this);
    }

    protected override void SetNodeData(IFeature itemData) {
        checkboxNode.SetValueSilent(itemData.LoadState is State.Enabled);
        checkboxNode.IsEnabled = itemData.LoadState is not State.Errored;

        erroringImageNode.IsVisible = itemData.LoadState is State.Errored || itemData.LoadErrorMessage is not null;
        erroringImageNode.TextTooltip = itemData.LoadErrorMessage ?? string.Empty;

        moduleNameNode.String = itemData.DisplayName;

        var isEnabled = itemData.LoadState is State.Enabled;
        EnableSelection = isEnabled;
        EnableHighlight = isEnabled;
    }

    public override void Update() {
        base.Update();

        if (ItemData is { LoadState: State.Enabled }) {
            EnableSelection = true;
            EnableHighlight = true;
        }
        else {
            EnableSelection = false;
            EnableHighlight = false;
        }
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        checkboxNode.Size = new Vector2(Height, Height);
        checkboxNode.Position = new Vector2(0.0f, 0.0f);

        erroringImageNode.Size = checkboxNode.Size - new Vector2(4.0f, 4.0f);
        erroringImageNode.Position = checkboxNode.Position + new Vector2(1.0f, 3.0f);

        moduleNameNode.Size = new Vector2(Width - Height * 2.0f, Height);
        moduleNameNode.Position = new Vector2(checkboxNode.Bounds.Right + 4.0f, 0.0f);
    }

    private void ToggleFeature(bool shouldEnable) {
        if (ItemData is null) return;

        if (shouldEnable) {
            Task.Run(() => System.Controller.EnableModule(ItemData));
        }
        else {
            Task.Run(() => System.Controller.DisableModule(ItemData));
        }

        OnStateChanged?.Invoke(this);
    }
}
