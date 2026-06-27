using DailyDuty.CustomNodes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace DailyDuty.Features.DutyFinderEnhancements;

public class DutyFinderEnhancementsConfigNode : ResNode {
    private readonly VerticalListNode listNode;

    public DutyFinderEnhancementsConfigNode(DutyFinderEnhancements module) {
        var originalColor = module.ModuleDutyFinderEnhancementsConfig.Color;

        listNode = new VerticalListNode {
            FitWidth = true,
            ItemSpacing = 8.0f,
            InitialNodes = [
                new CategoryHeaderNode {
                    String = Strings.DutyFinderEnhancements_TimerConfig,
                    Alignment = AlignmentType.Bottom,
                },
                new CheckboxNode {
                    Height = 28.0f,
                    String = Strings.DutyFinderEnhancements_HideSeconds,
                    IsChecked = module.ModuleDutyFinderEnhancementsConfig.HideSeconds,
                    OnClick = newValue => {
                        module.ModuleDutyFinderEnhancementsConfig.HideSeconds = newValue;
                        module.ModuleDutyFinderEnhancementsConfig.MarkDirty();
                    },
                },
                new ColorEditNode {
                    Height = 28.0f,
                    CurrentColor = originalColor,
                    DefaultColor = ColorHelper.GetColor(8),
                    String = Strings.DutyFinderEnhancements_TextColor,
                    OnColorCancelled = () => {
                        module.ModuleDutyFinderEnhancementsConfig.Color = originalColor;
                        module.ModuleDutyFinderEnhancementsConfig.MarkDirty();
                    },
                    OnColorConfirmed = color => {
                        module.ModuleDutyFinderEnhancementsConfig.Color = color;
                        module.ModuleDutyFinderEnhancementsConfig.MarkDirty();
                    },
                    OnColorPreviewed = color => {
                        module.ModuleDutyFinderEnhancementsConfig.Color = color;
                    },
                },
                new CategoryHeaderNode {
                    String = Strings.DutyFinderEnhancements_ButtonConfig,
                    Alignment = AlignmentType.Bottom,
                },
                new CheckboxNode {
                    Height = 28.0f,
                    String = Strings.DutyFinderEnhancements_ShowOpenButton,
                    IsChecked = module.ModuleDutyFinderEnhancementsConfig.OpenDailyDutyButton,
                    OnClick = newValue => {
                        module.ModuleDutyFinderEnhancementsConfig.OpenDailyDutyButton = newValue;
                        module.ModuleDutyFinderEnhancementsConfig.MarkDirty();
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
