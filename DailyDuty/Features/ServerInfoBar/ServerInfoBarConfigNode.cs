using DailyDuty.CustomNodes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;

namespace DailyDuty.Features.ServerInfoBar;

public class ServerInfoBarConfigNode : ResNode {
    private readonly VerticalListNode listNode;

    public ServerInfoBarConfigNode(ServerInfoBar module) {
        listNode = new VerticalListNode {
            FitWidth = true,
            ItemSpacing = 8.0f,
            InitialNodes = [
                new CategoryHeaderNode {
                    String = Strings.ServerInfoBar_FeatureConfig,
                    Alignment = AlignmentType.Bottom,
                },
                new CheckboxNode {
                    Height = 28.0f,
                    String = Strings.DutyFinderEnhancements_HideSeconds,
                    IsChecked = module.ModuleConfig.HideSeconds,
                    OnClick = newValue => {
                        module.ModuleConfig.HideSeconds = newValue;
                        module.ModuleConfig.MarkDirty();
                    },
                },
                new CheckboxNode {
                    Height = 28.0f,
                    String = Strings.ServerInfoBar_ToggleableTimer,
                    IsChecked = module.ModuleConfig.Combo,
                    OnClick = newValue => {
                        module.ModuleConfig.Combo = newValue;
                        module.ModuleConfig.MarkDirty();
                    },
                },
                new CheckboxNode {
                    Height = 28.0f,
                    String = Strings.ServerInfoBar_DailyTimer,
                    IsChecked = module.ModuleConfig.SoloDaily,
                    OnClick = newValue => {
                        module.ModuleConfig.SoloDaily = newValue;
                        module.ModuleConfig.MarkDirty();
                    },
                },
                new CheckboxNode {
                    Height = 28.0f,
                    String = Strings.ServerInfoBar_WeeklyTimer,
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
