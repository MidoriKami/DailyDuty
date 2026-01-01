using DailyDuty.CustomNodes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;

namespace DailyDuty.Features.ServerInfoBar;

public class ConfigNode : SimpleComponentNode {
    private readonly VerticalListNode listNode;

    public ConfigNode(ServerInfoBar module) {
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
                new CheckboxNode {
                    Height = 24.0f,
                    String = "Toggleable Timer",
                    IsChecked = module.ModuleConfig.Combo,
                    OnClick = newValue => {
                        module.ModuleConfig.Combo = newValue;
                        module.ModuleConfig.MarkDirty();
                    },
                },
                new CheckboxNode {
                    Height = 24.0f,
                    String = "Daily Timer",
                    IsChecked = module.ModuleConfig.SoloDaily,
                    OnClick = newValue => {
                        module.ModuleConfig.SoloDaily = newValue;
                        module.ModuleConfig.MarkDirty();
                    },
                },
                new CheckboxNode {
                    Height = 24.0f,
                    String = "Weekly Timer",
                    IsChecked = module.ModuleConfig.SoloWeekly,
                    OnClick = newValue => {
                        module.ModuleConfig.SoloWeekly = newValue;
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
