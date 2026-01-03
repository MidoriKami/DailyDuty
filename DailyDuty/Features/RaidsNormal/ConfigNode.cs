using DailyDuty.CustomNodes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;

namespace DailyDuty.Features.RaidsNormal;

public class ConfigNode(RaidsNormal module) : ConfigNodeBase<RaidsNormal>(module) {
    private readonly RaidsNormal module = module;
    
    protected override void BuildNode(ScrollingListNode container) {
        if (module.ModuleConfig.TrackedTasks.Count is 0) {
            container.AddNode(new HorizontalListNode {
                Height = 56.0f,
                InitialNodes = [
                    new TextNode {
                        Height = 56.0f,
                        LineSpacing = 14,
                        TextFlags = TextFlags.WordWrap | TextFlags.MultiLine,
                        AlignmentType = AlignmentType.Center,
                        String = "No raids are currently limited.",
                    },
                ],
            });

            return;
        }
        
        foreach (var (raid, raidStatus) in module.ModuleConfig.TrackedTasks) {
            if (!Services.DataManager.GetExcelSheet<ContentFinderCondition>().TryGetRow(raid, out var row)) continue;
            
            container.AddNode([
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
    }
}
