using System;
using System.Linq;
using DailyDuty.CustomNodes;
using DailyDuty.Enums;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace DailyDuty.Features.CustomDelivery;

public class ConfigNode(CustomDelivery module) : ConfigNodeBase<CustomDelivery>(module) {
    private readonly CustomDelivery module = module;

    protected override void BuildNode(ScrollingListNode container) {
        container.AddNode(new HorizontalFlexNode {
            Height = 28.0f,
            AlignmentFlags = FlexFlags.CenterVertically | FlexFlags.FitHeight | FlexFlags.FitWidth,
            InitialNodes = [
                new TextNode {
                    String = "Allowance Amount",
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
        });
        
        container.AddNode(new HorizontalFlexNode {
            Height = 28.0f,
            AlignmentFlags = FlexFlags.CenterVertically | FlexFlags.FitHeight | FlexFlags.FitWidth,
            InitialNodes = [
                new TextNode {
                    String = "Warning Trigger",
                    AlignmentType = AlignmentType.Left,
                },
                new TextDropDownNode {
                    Options = Enum.GetValues<ComparisonMode>().Select(mode => mode.Description).ToList(),
                    SelectedOption = module.ModuleConfig.ComparisonMode.Description,
                    OnOptionSelected = newOption => {
                        module.ModuleConfig.ComparisonMode = ComparisonModeExtensions.Parse(newOption);
                        module.ModuleConfig.MarkDirty();
                    },
                },
            ],
        });
    }
}
