using DailyDuty.CustomNodes;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;

namespace DailyDuty.Features.GrandCompanyProvision;

public class ConfigNode(GrandCompanyProvision module) : ConfigNodeBase<GrandCompanyProvision>(module) {
    private readonly GrandCompanyProvision module = module;

    protected override void BuildNode(ScrollingListNode container) {
        foreach (var (job, _) in module.ModuleData.ClassJobStatus) {
            var classJob = Services.DataManager.GetExcelSheet<ClassJob>().GetRow(job);

            container.AddNode(new CheckboxNode {
                String = classJob.NameEnglish.ToString(),
                Height = 24.0f,
                IsChecked = module.ModuleConfig.TrackedClasses[job],
                OnClick = newValue => {
                    module.ModuleConfig.TrackedClasses[job] = newValue;
                    module.ModuleConfig.MarkDirty();
                },
            });
        }
    }
}
