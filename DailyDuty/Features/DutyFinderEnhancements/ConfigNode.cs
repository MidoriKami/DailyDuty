using DailyDuty.CustomNodes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace DailyDuty.Features.DutyFinderEnhancements;

public class ConfigNode : SimpleComponentNode {
    private readonly VerticalListNode listNode;
    
    public ConfigNode(DutyFinderEnhancements module) {
        var originalColor = module.ModuleConfig.Color;
        
        listNode = new VerticalListNode {
            FitWidth = true,
            ItemSpacing = 8.0f,
            InitialNodes = [
                new CategoryHeaderNode {
                    Label= "Timer Configuration",
                    Alignment = AlignmentType.Bottom,
                },
                new CheckboxNode {
                    Height = 28.0f,
                    String = "Hide Seconds",
                    IsChecked = module.ModuleConfig.HideSeconds,
                    OnClick = newValue => {
                        module.ModuleConfig.HideSeconds = newValue;
                        module.ModuleConfig.MarkDirty();
                    },
                },
                new ColorEditNode {
                    Height = 28.0f,
                    CurrentColor = originalColor,
                    DefaultColor = ColorHelper.GetColor(8),
                    Label = "Text Color",
                    OnColorCancelled = () => {
                        module.ModuleConfig.Color = originalColor;
                        module.ModuleConfig.MarkDirty();
                    },
                    OnColorConfirmed = color => {
                        module.ModuleConfig.Color = color;
                        module.ModuleConfig.MarkDirty();
                    },
                    OnColorPreviewed = color => {
                        module.ModuleConfig.Color = color;
                    },
                },
                new CategoryHeaderNode {
                    Label= "DailyDuty Button Configuration",
                    Alignment = AlignmentType.Bottom,
                },
                new CheckboxNode {
                    Height = 28.0f,
                    String = "Show \"Open DailyDuty\" Button",
                    IsChecked = module.ModuleConfig.OpenDailyDutyButton,
                    OnClick = newValue => {
                        module.ModuleConfig.OpenDailyDutyButton = newValue;
                        module.ModuleConfig.MarkDirty();
                    },
                },
            ],
        };
        
        listNode.RecalculateLayout();
        listNode.AttachNode(this);
    }
    
    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        listNode.Size = Size;
        listNode.RecalculateLayout();
    }
}
