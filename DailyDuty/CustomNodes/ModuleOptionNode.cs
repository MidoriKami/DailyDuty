using System;
using System.Numerics;
using DailyDuty.Classes;
using DailyDuty.Enums;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace DailyDuty.CustomNodes;

public class ModuleOptionNode : SimpleComponentNode {
    private readonly NineGridNode hoveredBackgroundNode;
    private readonly NineGridNode selectedBackgroundNode;
    private readonly CheckboxNode checkboxNode;
    private readonly IconImageNode erroringImageNode;
    private readonly TextNode modificationNameNode;
    private readonly TextNode statusTextNode;
    private readonly CircleButtonNode configButtonNode;

    public ModuleOptionNode() {
        hoveredBackgroundNode = new SimpleNineGridNode {
            TexturePath = "ui/uld/ListItemA.tex",
            TextureCoordinates = new Vector2(0.0f, 22.0f),
            TextureSize = new Vector2(64.0f, 22.0f),
            TopOffset = 6,
            BottomOffset = 6,
            LeftOffset = 16,
            RightOffset = 1,
            IsVisible = false,
        };
        hoveredBackgroundNode.AttachNode(this);
        
        selectedBackgroundNode = new SimpleNineGridNode {
            TexturePath = "ui/uld/ListItemA.tex",
            TextureCoordinates = new Vector2(0.0f, 0.0f),
            TextureSize = new Vector2(64.0f, 22.0f),
            TopOffset = 6,
            BottomOffset = 6,
            LeftOffset = 16,
            RightOffset = 1,
            IsVisible = false,
        };
        selectedBackgroundNode.AttachNode(this);
        
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
            TextFlags = TextFlags.AutoAdjustNodeSize | TextFlags.Ellipsis,
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
                OnClick?.Invoke();
            },
        };
        configButtonNode.AttachNode(this);

        CollisionNode.DrawFlags = DrawFlags.ClickableCursor;
        
        CollisionNode.AddEvent(AtkEventType.MouseOver, () => {
            if (!IsSelected) {
                IsHovered = true;
            }
        });
        
        CollisionNode.AddEvent(AtkEventType.MouseDown, () => {
            OnClick?.Invoke();
        });
        
        CollisionNode.AddEvent(AtkEventType.MouseOut, () => {
            IsHovered = false;
        });
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
        
        OnClick?.Invoke();
        RefreshConfigWindowButton();
    }

    public void Update() {
        if (Module.FeatureBase is ModuleBase module) {
            statusTextNode.IsVisible = true;
            statusTextNode.SeString = $"Status: {module.ModuleStatus.Description}";
        }
        else {
            statusTextNode.IsVisible = false;
        }
    }

    public Action? OnClick { get; set; }

    public bool IsHovered {
        get => hoveredBackgroundNode.IsVisible;
        set => hoveredBackgroundNode.IsVisible = value;
    }
    
    public bool IsSelected {
        get => selectedBackgroundNode.IsVisible;
        set {
            selectedBackgroundNode.IsVisible = value;
            hoveredBackgroundNode.IsVisible = !value;
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
        hoveredBackgroundNode.Size = Size;
        selectedBackgroundNode.Size = Size;

        checkboxNode.Size = new Vector2(Height, Height) * 3.0f / 4.0f;
        checkboxNode.Position = new Vector2(Height, Height) / 8.0f;

        modificationNameNode.Height = Height / 2.0f;
        modificationNameNode.Position = new Vector2(Height + Height / 3.0f, 0.0f);
        
        statusTextNode.Height = Height / 2.0f;
        statusTextNode.Position = new Vector2(Height * 2.0f, Height / 2.0f);

        configButtonNode.Size = new Vector2(Height * 2.0f / 3.0f, Height * 2.0f / 3.0f);
        configButtonNode.Position = new Vector2(Width - Height, Height / 2.0f - configButtonNode.Height / 2.0f);

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
