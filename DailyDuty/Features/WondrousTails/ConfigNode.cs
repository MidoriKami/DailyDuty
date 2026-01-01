using System.Drawing;
using System.Numerics;
using DailyDuty.CustomNodes;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using KamiToolKit.Premade.Addons;
using KamiToolKit.Premade.Nodes;

namespace DailyDuty.Features.WondrousTails;

public class ConfigNode(WondrousTails module) : ConfigNodeBase<WondrousTails>(module) {
    private readonly WondrousTails module = module;
    private ColorPreviewNode? colorPreviewNode;

    protected override void BuildNode(VerticalListNode container) {
        HorizontalListNode colorEditNode;
        
        container.AddNode([
            new CheckboxNode {
                Height = 24.0f,
                String = "Instance Notifications",
                IsChecked = module.ModuleConfig.InstanceNotifications,
                OnClick = newValue => {
                    module.ModuleConfig.InstanceNotifications = newValue;
                    module.ModuleConfig.MarkDirty();
                },
            },
            new CheckboxNode {
                Height = 24.0f,
                String = "Sticker Available Notification",
                IsChecked = module.ModuleConfig.StickerAvailableNotice,
                OnClick = newValue => {
                    module.ModuleConfig.StickerAvailableNotice = newValue;
                    module.ModuleConfig.MarkDirty();
                },
            },
            new CheckboxNode {
                Height = 24.0f,
                String = "Unclaimed Book Notification",
                IsChecked = module.ModuleConfig.UnclaimedBookWarning,
                OnClick = newValue => {
                    module.ModuleConfig.UnclaimedBookWarning = newValue;
                    module.ModuleConfig.MarkDirty();
                },
            },
            new CheckboxNode {
                Height = 24.0f,
                String = "Shuffle Available Notification",
                IsChecked = module.ModuleConfig.ShuffleAvailableNotice,
                OnClick = newValue => {
                    module.ModuleConfig.ShuffleAvailableNotice = newValue;
                    module.ModuleConfig.MarkDirty();
                },
            },
            new CategoryHeaderNode {
                Label = "Duty Finder Integration",
            },
            new CheckboxNode {
                Height = 24.0f,
                String = "Recolor Duty Finder Entries",
                IsChecked = module.ModuleConfig.ColorDutyFinderText,
                OnClick = newValue => {
                    module.ModuleConfig.ColorDutyFinderText = newValue;
                    module.ModuleConfig.MarkDirty();
                },
            },
            colorEditNode = new HorizontalListNode {
                Height = 28.0f,
                ItemSpacing = 8.0f,
                InitialNodes = [
                    colorPreviewNode = new ColorPreviewNode {
                        Size = new Vector2(24.0f, 24.0f),
                        Color = module.ModuleConfig.DutyFinderColor,
                    },
                    new TextNode {
                        Size = new Vector2(100.0f, 24.0f),
                        String = "Entry Color",
                        AlignmentType = AlignmentType.Left,
                    },
                ],
            },
            new ResNode{ Height = 4.0f },
            new CheckboxNode {
                Height = 24.0f,
                String = "Show Clover Indicator",
                IsChecked = module.ModuleConfig.CloverIndicator,
                OnClick = newValue => {
                    module.ModuleConfig.CloverIndicator = newValue;
                    module.ModuleConfig.MarkDirty();
                },
            },
        ]);

        colorPreviewNode.CollisionNode.ShowClickableCursor = true;
        colorPreviewNode.CollisionNode.AddEvent(AtkEventType.MouseClick, OnColorEdit);
        
        colorEditNode.CollisionNode.ShowClickableCursor = true;
        colorEditNode.CollisionNode.AddEvent(AtkEventType.MouseClick, OnColorEdit);
    }

    private void OnColorEdit() {
        module.ColorPicker ??= new ColorPickerAddon {
            InternalName = "ColorPicker",
            Title = "Wondrous Tails Color Picker",
        };

        var originalColor = module.ModuleConfig.DutyFinderColor;
        module.ColorPicker.DefaultColor = KnownColor.Yellow.Vector();
        module.ColorPicker.InitialColor = module.ModuleConfig.DutyFinderColor;
            
        module.ColorPicker.OnColorPreviewed = color => {
            colorPreviewNode?.Color = color;
            module.ModuleConfig.DutyFinderColor = color;
        };
            
        module.ColorPicker.OnColorCancelled = () => {
            module.ModuleConfig.DutyFinderColor = originalColor;
            module.ModuleConfig.MarkDirty();
        };
            
        module.ColorPicker.OnColorConfirmed = color => {
            module.ModuleConfig.DutyFinderColor = color;
            module.ModuleConfig.MarkDirty();
        };
            
        module.ColorPicker.Toggle();
    }
}
