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

namespace DailyDuty.Features.GrandCompanySupply;

public unsafe class GrandCompanySupply : Module<GrandCompanySupplyConfig, GrandCompanySupplyData> {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Grand Company Supply",
        FileName = "GrandCompanySupply",
        Type = ModuleType.Daily,
        ChangeLog = [
            new ChangeLogInfo(1, "Initial Re-Implementation"),
        ],
        Tags = [ "GrandCompany", "GC", "Gil", "Company Seals", "Seals" ],
    };

    public override DataNodeBase DataNode => new GrandCompanySupplyDataNode(this);
    public override ConfigNodeBase ConfigNode => new GrandCompanySupplyConfigNode(this);

    protected override GrandCompanySupplyConfig MigrateConfig(JObject objectData)
        => GrandCompanySupplyMigration.Migrate(objectData);
    
    protected override StatusMessage GetStatusMessage()
        => $"{GetIncompleteCount()} Deliveries Available";
    
    protected override TodoTooltip GetTooltip() 
        => string.Join("\n", GetIncompleteJobs());
    
    public override DateTime GetNextResetDateTime()
        => Time.NextGrandCompanyReset();

    public override TimeSpan GetResetPeriod()
        => TimeSpan.FromDays(1);

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
        // Grab items 0 -> 8 from the Agent
        foreach (uint index in Enumerable.Range(0, 8)) {
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
