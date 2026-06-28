using System.Numerics;
using System.Threading.Tasks;
using DailyDuty.Classes;
using DailyDuty.Enums;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Enums;
using KamiToolKit.Interfaces;
using KamiToolKit.Nodes;

namespace DailyDuty.CustomNodes;

public class ModuleOptionNode : TreeListItemNode<LoadedModule>, ITreeListItemNode {

    public static float ItemHeight => 36.0f;

    public ModuleOptionNode() {
        checkboxNode = new CheckboxNode();
        checkboxNode.AttachNode(this);

        erroringImageNode = new IconImageNode {
            IconId = 61502,
            FitTexture = true,
            TextTooltip = Strings.ModuleOptionNode_FailedToLoad,
        };
        erroringImageNode.AttachNode(this);

        modificationNameNode = new TextNode {
            TextFlags = TextFlags.Ellipsis,
            AlignmentType = AlignmentType.BottomLeft,
        };
        modificationNameNode.AttachNode(this);

        statusTextNode = new TextNode {
            FontType = FontType.Axis,
            TextFlags = TextFlags.Ellipsis,
            AlignmentType = AlignmentType.TopLeft,
            TextColor = ColorHelper.GetColor(3),
            IsVisible = false,
        };
        statusTextNode.AttachNode(this);

        configButtonNode = new CircleButtonNode {
            Icon = CircleButtonIcon.GearCog,
            TextTooltip = Strings.ModuleOptionNode_OpenConfig,
            OnClick = () => {
                ItemData?.FeatureBase.OpenConfigAction?.Invoke();
                OnClick?.Invoke(this);
            },
        };
        configButtonNode.AttachNode(this);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        checkboxNode.Size = new Vector2(Height - 8.0f, Height - 8.0f);
        checkboxNode.Position = new Vector2(4.0f, 4.0f);

        modificationNameNode.Size = new Vector2(Width - Height * 2.0f, Height / 2.0f);
        modificationNameNode.Position = new Vector2(checkboxNode.Bounds.Right + 4.0f, 0.0f);

        statusTextNode.Size = new Vector2(Width - Height * 2.5f, Height / 2.0f);
        statusTextNode.Position = new Vector2(Height / 2.0f + checkboxNode.Bounds.Right + 4.0f, Height / 2.0f);

        configButtonNode.Size = new Vector2(Height * 2.0f / 3.0f, Height * 2.0f / 3.0f);
        configButtonNode.Position = new Vector2(Width - Height + 4.0f, Height / 2.0f - configButtonNode.Height / 2.0f);

        erroringImageNode.Size = checkboxNode.Size - new Vector2(4.0f, 4.0f);
        erroringImageNode.Position = checkboxNode.Position + new Vector2(1.0f, 3.0f);
    }

    protected override void SetNodeData(LoadedModule itemData) {
        checkboxNode.OnClick = null;
        checkboxNode.IsEnabled = itemData.State is not LoadedState.Errored;
        checkboxNode.IsChecked = itemData.State is LoadedState.Enabled;
        checkboxNode.OnClick = shouldEnable => Task.Run(() => ToggleModification(shouldEnable));

        erroringImageNode.IsVisible = itemData.State is LoadedState.Errored;
        erroringImageNode.TextTooltip = itemData.ErrorMessage;

        if (itemData.FeatureBase is ModuleBase module) {

            if (itemData.State is LoadedState.Enabled) {
                modificationNameNode.Height = Height / 2.0f;
                modificationNameNode.AlignmentType = AlignmentType.BottomLeft;
            }
            else {
                modificationNameNode.Height = Height;
                modificationNameNode.AlignmentType = AlignmentType.Left;
            }

            statusTextNode.String = $"{Strings.DataNodeBase_Status}: {module.ModuleStatus.Description}";
            statusTextNode.IsVisible = itemData.State is LoadedState.Enabled;
        }
        else {
            modificationNameNode.Height = Height;
            modificationNameNode.AlignmentType = AlignmentType.Left;

            statusTextNode.IsVisible = false;
        }

        modificationNameNode.String = itemData.FeatureBase.ModuleInfo.DisplayName;

        configButtonNode.IsEnabled = itemData.State is LoadedState.Enabled;
        configButtonNode.IsVisible = itemData.FeatureBase.OpenConfigAction is not null;
    }

    private async Task ToggleModification(bool shouldEnableModification) {
        if (ItemData is null) return;

        if (shouldEnableModification && ItemData.State is LoadedState.Disabled) {
            await System.ModuleManager.TryEnableModule(ItemData);
        }
        else if (!shouldEnableModification && ItemData.State is LoadedState.Enabled) {
            await System.ModuleManager.TryDisableModification(ItemData);
        }

        if (ItemData.State is LoadedState.Errored) {
            checkboxNode.IsEnabled = false;
            erroringImageNode.IsVisible = true;
            erroringImageNode.TextTooltip = ItemData.ErrorMessage;
        }
        else {
            checkboxNode.IsEnabled = true;
            erroringImageNode.IsVisible = false;
        }

        unsafe {
            var addon = RaptureAtkUnitManager.Instance()->GetAddonByNode(this);
            if (addon is not null) {
                addon->UpdateCollisionNodeList(false);
            }
        }

        checkboxNode.IsChecked = ItemData.State is LoadedState.Enabled;

        configButtonNode.IsEnabled = ItemData.State is LoadedState.Enabled;
        configButtonNode.IsVisible = ItemData.FeatureBase.OpenConfigAction is not null;

        await Services.Framework.RunSafely(() => OnClick?.Invoke(this));

        if (ItemData.FeatureBase.OpenConfigAction is not null) {
            configButtonNode.IsVisible = true;
            configButtonNode.IsEnabled = ItemData.State is LoadedState.Enabled;
        }

        if (ItemData.FeatureBase is ModuleBase module) {
            if (ItemData.State is LoadedState.Enabled) {
                modificationNameNode.Height = Height / 2.0f;
                modificationNameNode.AlignmentType = AlignmentType.BottomLeft;
            }
            else {
                modificationNameNode.Height = Height;
                modificationNameNode.AlignmentType = AlignmentType.Left;
            }

            statusTextNode.IsVisible = ItemData.State is LoadedState.Enabled;
            statusTextNode.String = $"{Strings.DataNodeBase_Status}: {module.ModuleStatus.Description}";
        }
    }

    private readonly CheckboxNode checkboxNode;
    private readonly IconImageNode erroringImageNode;
    private readonly TextNode modificationNameNode;
    private readonly TextNode statusTextNode;
    private readonly CircleButtonNode configButtonNode;
}
