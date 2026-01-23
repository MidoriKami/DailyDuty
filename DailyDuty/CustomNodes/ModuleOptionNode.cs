using System.Numerics;
using DailyDuty.Classes;
using DailyDuty.Enums;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace DailyDuty.CustomNodes;

public class ModuleOptionNode : SelectableNode {
    private readonly CheckboxNode checkboxNode;
    private readonly IconImageNode erroringImageNode;
    private readonly TextNode modificationNameNode;
    private readonly TextNode statusTextNode;
    private readonly CircleButtonNode configButtonNode;

    public ModuleOptionNode() {
        checkboxNode = new CheckboxNode {
            OnClick = ToggleModification,
        };
        checkboxNode.AttachNode(this);

        erroringImageNode = new IconImageNode {
            IconId = 61502,
            FitTexture = true,
            TextTooltip = "Module Failed To Load",
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
        };
        statusTextNode.AttachNode(this);
        
        configButtonNode = new CircleButtonNode {
            Icon = ButtonIcon.GearCog,
            TextTooltip = "Open Configuration",
            OnClick = () => {
                Module?.FeatureBase.OpenConfigAction?.Invoke();
                OnClick?.Invoke(this);
            },
        };
        configButtonNode.AttachNode(this);
    }

    public ModuleInfo ModuleInfo => Module.FeatureBase.ModuleInfo;
    
    public required LoadedModule Module {
        get;
        set {
            field = value;
            modificationNameNode.String = value.FeatureBase.ModuleInfo.DisplayName;
            
            RefreshConfigWindowButton();

            checkboxNode.IsChecked = value.State is LoadedState.Enabled;

            UpdateDisabledState();
            
            if (Module.FeatureBase is ModuleBase) {
                modificationNameNode.Height = Height / 2.0f;
                modificationNameNode.AlignmentType = AlignmentType.BottomLeft;
            }
            else {
                modificationNameNode.Height = Height;
                modificationNameNode.AlignmentType = AlignmentType.Left;
            }
        }
    }
    
    private void ToggleModification(bool shouldEnableModification) {
        if (shouldEnableModification && Module.State is LoadedState.Disabled) {
            ModuleManager.TryEnableModule(Module);
        }
        else if (!shouldEnableModification && Module.State is LoadedState.Enabled) {
            ModuleManager.TryDisableModification(Module);
        }

        UpdateDisabledState();
        
        OnClick?.Invoke(this);
        RefreshConfigWindowButton();
    }

    public void Update() {
        if (Module.FeatureBase is ModuleBase module) {
            statusTextNode.IsVisible = true;
            statusTextNode.String = $"Status: {module.ModuleStatus.Description}";
            modificationNameNode.Height = Height / 2.0f;
        }
        else {
            statusTextNode.IsVisible = false;
            modificationNameNode.Height = Height;
        }
    }

    private void RefreshConfigWindowButton() {
        if (Module.FeatureBase.OpenConfigAction is not null) {
            configButtonNode.IsVisible = true;
            configButtonNode.IsEnabled = Module.State is LoadedState.Enabled;
        }
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

    private void UpdateDisabledState() {
        if (Module.State is LoadedState.Errored) {
            checkboxNode.IsEnabled = false;
            erroringImageNode.IsVisible = true;
            erroringImageNode.TextTooltip = Module.ErrorMessage;
        }
        else {
            checkboxNode.IsEnabled = true;
            erroringImageNode.IsVisible = false;
        }

        UpdateCollisionForNode(this);

        checkboxNode.IsChecked = Module.State is LoadedState.Enabled;
        configButtonNode.IsEnabled = Module.State is LoadedState.Enabled;
        configButtonNode.IsVisible = Module.FeatureBase.OpenConfigAction is not null;

        RefreshConfigWindowButton();
    }

    private static unsafe void UpdateCollisionForNode(NodeBase node) {
        var addon = RaptureAtkUnitManager.Instance()->GetAddonByNode(node);
        if (addon is not null) {
            addon->UpdateCollisionNodeList(false);
        }
    }
}
