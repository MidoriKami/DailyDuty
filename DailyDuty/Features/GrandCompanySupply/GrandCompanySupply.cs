using System;
using System.Linq;
using DailyDuty.Classes;
using DailyDuty.Classes.Nodes;
using DailyDuty.Enums;
using DailyDuty.Utilities;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Text.ReadOnly;

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

    public override DataNodeBase DataNode => new DataNode(this);
    public override ConfigNodeBase ConfigNode => new ConfigNode(this);

    protected override ReadOnlySeString GetStatusMessage()
        => $"{GetIncompleteCount()} Supply Deliveries Available";

    public override DateTime GetNextResetDateTime()
        => Time.NextGrandCompanyReset();

    public override TimeSpan GetResetPeriod()
        => TimeSpan.FromDays(1);

    public override void Reset() {
    }

    protected override void Update() {
        base.Update();

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
}
