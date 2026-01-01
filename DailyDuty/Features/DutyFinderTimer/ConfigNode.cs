using System.Numerics;
using DailyDuty.CustomNodes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using KamiToolKit.Premade.Addons;
using KamiToolKit.Premade.Nodes;

namespace DailyDuty.Features.DutyFinderTimer;

public class ConfigNode : SimpleComponentNode {
    private readonly DutyFinderTimer module;
    private readonly VerticalListNode listNode;
    private readonly ColorPreviewNode? colorPreviewNode;
    
    public ConfigNode(DutyFinderTimer module) {
        this.module = module;
        HorizontalListNode colorEditNode;
        
        listNode = new VerticalListNode {
            FitWidth = true,
            ItemSpacing = 8.0f,
            InitialNodes = [
                new CategoryHeaderNode {
                    Label= "Feature Configuration",
                    Alignment = AlignmentType.Bottom,
                },
                new CheckboxNode {
                    Height = 24.0f,
                    String = "Hide Seconds",
                    IsChecked = module.ModuleConfig.HideSeconds,
                    OnClick = newValue => {
                        module.ModuleConfig.HideSeconds = newValue;
                        module.ModuleConfig.MarkDirty();
                    },
                },
                new ResNode{ Height = 4.0f },
                colorEditNode = new HorizontalListNode {
                    Height = 28.0f,
                    ItemSpacing = 8.0f,
                    InitialNodes = [
                        colorPreviewNode = new ColorPreviewNode {
                            Size = new Vector2(24.0f, 24.0f),
                            Color = module.ModuleConfig.Color,
                        },
                        new TextNode {
                            Size = new Vector2(100.0f, 24.0f),
                            String = "Text Color",
                            AlignmentType = AlignmentType.Left,
                        },
                    ],
                },
            ],
        };
        
        listNode.RecalculateLayout();
        listNode.AttachNode(this);

        colorPreviewNode.CollisionNode.ShowClickableCursor = true;
        colorPreviewNode.CollisionNode.AddEvent(AtkEventType.MouseClick, OnColorEdit);
        
        colorEditNode.CollisionNode.ShowClickableCursor = true;
        colorEditNode.CollisionNode.AddEvent(AtkEventType.MouseClick, OnColorEdit);
    }

    private void OnColorEdit() {
        module.ColorPicker ??= new ColorPickerAddon {
            InternalName = "ColorPicker",
            Title = "Duty Finder Timer Color Picker",
        };

        var originalColor = module.ModuleConfig.Color;
        module.ColorPicker.DefaultColor = ColorHelper.GetColor(8);
        module.ColorPicker.InitialColor = module.ModuleConfig.Color;
            
        module.ColorPicker.OnColorPreviewed = color => {
            colorPreviewNode?.Color = color;
            module.ModuleConfig.Color = color;
        };
            
        module.ColorPicker.OnColorCancelled = () => {
            module.ModuleConfig.Color = originalColor;
            module.ModuleConfig.MarkDirty();
        };
            
        module.ColorPicker.OnColorConfirmed = color => {
            module.ModuleConfig.Color = color;
            module.ModuleConfig.MarkDirty();
        };
            
        module.ColorPicker.Toggle();
    }
    
    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        listNode.Size = Size;
        listNode.RecalculateLayout();
    }
}
