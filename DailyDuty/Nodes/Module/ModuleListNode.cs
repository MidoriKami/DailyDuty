using System.Numerics;
using System.Threading.Tasks;
using DailyDuty.Enums;
using DailyDuty.Interfaces;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.UI;
using KamiToolKit.Core.Interfaces;
using KamiToolKit.UI.Nodes;
using KamiToolKit.UI.Nodes.ComponentNode;

namespace DailyDuty.Nodes.Module;

public class ModuleListNode : ListItemNode<IModule>, IListItemNode {
    public static float ItemHeight => 36.0f;

    private readonly CheckboxNode checkboxNode;
    private readonly IconImageNode erroringImageNode;
    private readonly ResNode enabledContainer;
    private readonly ResNode disabledContainer;
    private readonly TextNode moduleNameNode;
    private readonly TextNode moduleNameNodeFull;
    private readonly TextNode statusTextNode;

    public ModuleListNode() {
        checkboxNode = new CheckboxNode {
            OnChecked = ToggleModule,
        };
        checkboxNode.AttachNode(this);

        erroringImageNode = new IconImageNode {
            IconId = 61502,
        };
        erroringImageNode.AttachNode(this);

        enabledContainer = new ResNode { IsVisible = true };
        enabledContainer.AttachNode(this);

        moduleNameNode = new TextNode {
            TextFlags = TextFlags.Ellipsis,
            AlignmentType = AlignmentType.BottomLeft,
        };
        moduleNameNode.AttachNode(enabledContainer);

        statusTextNode = new TextNode {
            FontType = FontType.Axis,
            TextFlags = TextFlags.Ellipsis,
            AlignmentType = AlignmentType.TopLeft,
            TextColor = ColorHelper.GetColor(3),
        };
        statusTextNode.AttachNode(enabledContainer);

        disabledContainer = new ResNode { IsVisible = false };
        disabledContainer.AttachNode(this);

        moduleNameNodeFull = new TextNode {
            TextFlags = TextFlags.Ellipsis,
            AlignmentType = AlignmentType.Left,
            FontSize = 14,
        };
        moduleNameNodeFull.AttachNode(disabledContainer);
    }

    protected override void SetNodeData(IModule itemData) {
        checkboxNode.SetValueSilent(itemData.LoadState is State.Enabled);
        checkboxNode.IsEnabled = itemData.LoadState is not State.Errored;

        erroringImageNode.IsVisible = itemData.LoadState is State.Errored || itemData.LoadErrorMessage is not null;
        erroringImageNode.TextTooltip = itemData.LoadErrorMessage ?? string.Empty;

        moduleNameNode.String = itemData.DisplayName;
        moduleNameNodeFull.String = itemData.DisplayName;

        var isEnabled = itemData.LoadState is State.Enabled;
        EnableSelection = isEnabled;
        EnableHighlight = isEnabled;
    }

    public override void Update() {
        base.Update();

        if (ItemData is { LoadState: State.Enabled }) {
            statusTextNode.String = $"{Strings.DataNodeBase_Status}: {ItemData.GetStatus().ToString()}";
            enabledContainer.IsVisible = true;
            disabledContainer.IsVisible = false;
            EnableSelection = true;
            EnableHighlight = true;
        }
        else {
            enabledContainer.IsVisible = false;
            disabledContainer.IsVisible = true;
            EnableSelection = false;
            EnableHighlight = false;
        }
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        checkboxNode.Size = new Vector2(Height - 8.0f, Height - 8.0f);
        checkboxNode.Position = new Vector2(4.0f, 4.0f);

        erroringImageNode.Size = checkboxNode.Size - new Vector2(4.0f, 4.0f);
        erroringImageNode.Position = checkboxNode.Position + new Vector2(1.0f, 3.0f);

        enabledContainer.Size = new Vector2(Width - Height, Height);
        enabledContainer.Position = new Vector2(checkboxNode.Bounds.Right + 4.0f, 0.0f);

        moduleNameNode.Size = new Vector2(enabledContainer.Width, Height / 2.0f);
        moduleNameNode.Position = new Vector2(0.0f, 0.0f);

        statusTextNode.Size = new Vector2(enabledContainer.Width, Height / 2.0f);
        statusTextNode.Position = new Vector2(0.0f, Height / 2.0f);

        disabledContainer.Size = enabledContainer.Size;
        disabledContainer.Position = enabledContainer.Position;

        moduleNameNodeFull.Size = disabledContainer.Size;
        moduleNameNodeFull.Position = new Vector2(0.0f, 0.0f);
    }

    private void ToggleModule(bool shouldEnable) {
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
