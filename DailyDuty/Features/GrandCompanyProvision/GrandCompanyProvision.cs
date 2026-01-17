using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.Classes;
using DailyDuty.CustomNodes;
using DailyDuty.Enums;
using DailyDuty.Utilities;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Excel.Sheets;
using Newtonsoft.Json.Linq;

namespace DailyDuty.Features.GrandCompanyProvision;

public unsafe class GrandCompanyProvision : Module<GrandCompanyProvisionConfig, GrandCompanyProvisionData> {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Grand Company Provision",
        FileName = "GrandCompanyProvision",
        Type = ModuleType.Daily,
        ChangeLog = [
            new ChangeLogInfo(1, "Initial Re-Implementation"),
        ],
        Tags = [ "GrandCompany", "GC", "Gil", "Company Seals", "Seals" ],
    };

    public override DataNodeBase DataNode => new GrandCompanyProvisionDataNode(this);
    public override ConfigNodeBase ConfigNode => new GrandCompanyProvisionConfigNode(this);

    protected override GrandCompanyProvisionConfig MigrateConfig(JObject objectData)
        => GrandCompanyProvisionMigration.Migrate(objectData);

    protected override StatusMessage GetStatusMessage()
        => $"{GetIncompleteCount()} Deliveries Available";

    public override DateTime GetNextResetDateTime()
        => Time.NextGrandCompanyReset();

    public override TimeSpan GetResetPeriod()
        => TimeSpan.FromDays(1);

    protected override TodoTooltip GetTooltip() 
        => string.Join("\n", GetIncompleteJobs());

    public override void Reset() {
        foreach (var entry in ModuleData.ClassJobStatus.Keys) {
            ModuleData.ClassJobStatus[entry] = false;
        }
    }

    protected override void OnModuleUpdate() {
        var agent = AgentGrandCompanySupply.Instance();
        if (!agent->IsAgentActive()) return;
        if (agent->NumItems < 11) return;

        // Offset ClassJob by -8, as there's 8 non-DoH/DoL jobs at the start of the sheet that native omits
        // Grab items 8, 9, 10 from the Agent
        foreach (uint index in Enumerable.Range(8, 3)) {
            if (ModuleData.ClassJobStatus[index + 8] != !agent->ItemArray[index].IsTurnInAvailable) {
                ModuleData.ClassJobStatus[index + 8] = !agent->ItemArray[index].IsTurnInAvailable;
                ModuleData.MarkDirty();
            }
        }
    }

    protected override CompletionStatus GetCompletionStatus() {
        foreach (var job in ModuleConfig.TrackedClasses.Where(pair => pair.Value)) {
            if (!ModuleData.ClassJobStatus[job.Key]) return CompletionStatus.Incomplete;
        }
        
        return CompletionStatus.Complete;
    }

    private int GetIncompleteCount()
        => ModuleConfig.TrackedClasses
            .Where(pair => pair.Value)
            .Count(job => !ModuleData.ClassJobStatus[job.Key]);

    private IEnumerable<string> GetIncompleteJobs()
        => ModuleConfig.TrackedClasses
            .Where(pair => pair.Value)
            .Where(job => !ModuleData.ClassJobStatus[job.Key])
            .Select(job => Services.DataManager.GetExcelSheet<ClassJob>().GetRow(job.Key).NameEnglish.ToString());
}
