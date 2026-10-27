using DailyDuty.CustomNodes;
using Dalamud.Plugin.Services;
using KamiToolKit.BaseTypes;
using KamiToolKit.Nodes;
using Dalamud.Excel.Sheets;

namespace DailyDuty.Features.GrandCompanyProvision;

public class GrandCompanyProvisionConfigNode(GrandCompanyProvision module) : ConfigNodeBase<GrandCompanyProvision>(module) {
    private readonly GrandCompanyProvision module = module;

    protected override NodeBase BuildNode() {
        var layoutNode = new VerticalListNode {
            FitWidth = true,
            ItemSpacing = 4.0f,
        };

        foreach (var (job, _) in module.ModuleData.ClassJobStatus) {
            var classJob = IDataManager.Get().GetExcelSheet<ClassJob>().GetRow(job);

            layoutNode.AddNode(new CheckboxNode {
                String = classJob.NameEnglish.ToString(),
                Height = 28.0f,
                IsChecked = module.ModuleConfig.TrackedClasses[job],
                OnClick = newValue => {
                    module.ModuleConfig.TrackedClasses[job] = newValue;
                    module.ModuleConfig.MarkDirty();
                },
            });
        }

        return layoutNode;
    }
}
