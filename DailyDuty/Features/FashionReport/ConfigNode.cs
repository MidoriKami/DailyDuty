using System;
using System.Linq;
using System.Numerics;
using DailyDuty.CustomNodes;
using DailyDuty.Enums;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;

namespace DailyDuty.Features.FashionReport;

public class ConfigNode(FashionReport module) : ConfigNodeBase<FashionReport>(module) {
    private readonly FashionReport module = module;

    protected override void BuildNode(VerticalListNode container) {
        container.AddNode(new HorizontalListNode {
            Height = 28.0f,
            InitialNodes = [
                new TextNode {
                    Size = new Vector2(150.0f, 28.0f),
                    String = "Completion Mode",
                    AlignmentType = AlignmentType.Left,
                },
                new TextDropDownNode {
                    Size = new Vector2(200.0f, 28.0f),
                    Options = Enum.GetValues<FashionReportMode>().Select(mode => mode.Description).ToList(),
                    SelectedOption = module.ModuleConfig.CompletionMode.Description,
                    OnOptionSelected = newOption => {
                        module.ModuleConfig.CompletionMode = FashionReportModeExtensions.Parse(newOption);
                        module.ModuleConfig.MarkDirty();
                    },
                },
            ],
        });
    }
}
