using DailyDuty.CustomNodes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace DailyDuty.Features.DutyFinderTimer;

public class ConfigNode : SimpleComponentNode {
    private readonly VerticalListNode listNode;
    
    public ConfigNode(DutyFinderTimer module) {
        var originalColor = module.ModuleConfig.Color;
        
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
                new ColorEditNode {
                    Height = 28.0f,
                    CurrentColor = originalColor,
                    DefaultColor = ColorHelper.GetColor(8),
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
