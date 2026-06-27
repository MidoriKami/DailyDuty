using DailyDuty.CustomNodes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;

namespace DailyDuty.Features.RaidsNormal;

public class RaidsNormalConfigNode(RaidsNormal module) : ConfigNodeBase<RaidsNormal>(module) {
    private readonly RaidsNormal module = module;

    protected override NodeBase BuildNode() {
        var layoutNode = new VerticalListNode {
            FitWidth = true,
            ItemSpacing = 4.0f,
        };

        if (module.ModuleConfig.TrackedTasks.Count is 0) {
            layoutNode.AddNode(new HorizontalListNode {
                Height = 56.0f,
                InitialNodes = [
                    new TextNode {
                        Height = 56.0f,
                        LineSpacing = 14,
                        TextFlags = TextFlags.WordWrap | TextFlags.MultiLine,
                        AlignmentType = AlignmentType.Center,
                        String = Strings.Raids_NoneLimited,
                    },
                ],
            });

            return layoutNode;
        }

        foreach (var (raid, raidStatus) in module.ModuleConfig.TrackedTasks) {
            if (!Services.DataManager.GetExcelSheet<ContentFinderCondition>().TryGetRow(raid, out var row)) continue;

            layoutNode.AddNode([
                new CheckboxNode {
                    Height = 28.0f,
                    String = row.Name.ToString(),
                    IsChecked = raidStatus,
                    OnClick = newValue => {
                        module.ModuleConfig.TrackedTasks[raid] = newValue;
                        module.ModuleConfig.MarkDirty();
                    },
                },
            ]);
        }

        return layoutNode;
    }
}
