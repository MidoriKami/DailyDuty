using Resources;
using DailyDuty.Classes;
using DailyDuty.CustomNodes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using KamiToolKit.Premade.Node.Color;
using KamiToolKit.Premade.Node.Simple;

namespace DailyDuty.Features.DutyFinderEnhancements;

public class DutyFinderEnhancementsConfigNode : SimpleComponentNode {
    private readonly VerticalListNode listNode;

    public DutyFinderEnhancementsConfigNode(DutyFinderEnhancements module) {
        var originalColor = module.ModuleDutyFinderEnhancementsConfig.Color;

        listNode = new VerticalListNode {
            FitWidth = true,
            ItemSpacing = 8.0f,
            InitialNodes = [
                new CategoryHeaderNode {
                    String = Strings.ResourceManager.GetString("Timer Configuration", Strings.Culture) ?? "Timer Configuration",
                    Alignment = AlignmentType.Bottom,
                },
                new CheckboxNode {
                    Height = 28.0f,
                    String = Strings.ResourceManager.GetString("Hide Seconds", Strings.Culture) ?? "Hide Seconds",
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
                    String = Strings.ResourceManager.GetString("Text Color", Strings.Culture) ?? "Text Color",
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
                    String = Strings.ResourceManager.GetString("DailyDuty Button Configuration", Strings.Culture) ?? "DailyDuty Button Configuration",
                    Alignment = AlignmentType.Bottom,
                },
                new CheckboxNode {
                    Height = 28.0f,
                    String = Strings.ResourceManager.GetString("Show \"Open DailyDuty\" Button", Strings.Culture) ?? "Show \"Open DailyDuty\" Button",
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
