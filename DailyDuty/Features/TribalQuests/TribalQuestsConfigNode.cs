using System;
using System.Linq;
using DailyDuty.CustomNodes;
using DailyDuty.Enums;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;

namespace DailyDuty.Features.TribalQuests;

public class TribalQuestsConfigNode(TribalQuests module) : ConfigNodeBase<TribalQuests>(module) {
    private readonly TribalQuests module = module;

    protected override NodeBase BuildNode() => new VerticalListNode {
        FitWidth = true,
        ItemSpacing = 4.0f,
        InitialNodes = [
            new HorizontalFlexNode {
                Height = 28.0f,
                AlignmentFlags = FlexFlags.CenterVertically | FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        String = Strings.TribalQuests_QuestAmount,
                        AlignmentType = AlignmentType.Left,
                    },
                    new SliderNode {
                        Range = ..12,
                        Value = module.ModuleConfig.NotificationThreshold,
                        OnValueChanged = newValue => {
                            if (newValue != module.ModuleConfig.NotificationThreshold) {
                                module.ModuleConfig.NotificationThreshold = newValue;
                                module.ModuleConfig.MarkDirty();
                            }
                        },
                    },
                ],
            },
            new HorizontalFlexNode {
                Height = 28.0f,
                AlignmentFlags = FlexFlags.CenterVertically | FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        String = Strings.CustomDelivery_WarningTrigger,
                        AlignmentType = AlignmentType.Left,
                    },
                    new StringDropDownNode {
                        Options = Enum.GetValues<ComparisonMode>().Select(mode => mode.Description).ToList(),
                        SelectedOption = module.ModuleConfig.ComparisonMode.Description,
                        OnOptionSelected = newOption => {
                            module.ModuleConfig.ComparisonMode = ComparisonModeExtensions.Parse(newOption);
                            module.ModuleConfig.MarkDirty();
                        },
                    },
                ],
            },
        ],
    };
}
