using System;
using DailyDuty.Classes;
using DailyDuty.Classes.Nodes;
using DailyDuty.Enums;
using DailyDuty.Utilities;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Text.ReadOnly;

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

    public override DataNodeBase DataNode => new DataNode(this);
    public override ConfigNodeBase ConfigNode => new ConfigNode(this);

    protected override ReadOnlySeString GetStatusMessage()
        => $"{GetIncompleteCount()} Provision Deliveries Available";

    public override DateTime GetNextResetDateTime()
        => Time.NextGrandCompanyReset();

    public override TimeSpan GetResetPeriod()
        => TimeSpan.FromDays(1);

    public override void Reset() {
        ModuleData.MinerComplete = false;
        ModuleData.BotanistComplete = false;
        ModuleData.FisherComplete = false;
    }

    protected override void Update() {
        base.Update();

        var agent = AgentGrandCompanySupply.Instance();
        if (!agent->IsAgentActive()) return;
        if (agent->NumItems < 11) return;

        if (ModuleData.MinerComplete != !agent->ItemArray[8].IsTurnInAvailable) {
            ModuleData.MinerComplete = !agent->ItemArray[8].IsTurnInAvailable;
            ModuleData.MarkDirty();
        }

        if (ModuleData.BotanistComplete != !agent->ItemArray[9].IsTurnInAvailable) {
            ModuleData.BotanistComplete = !agent->ItemArray[9].IsTurnInAvailable;
            ModuleData.MarkDirty();
        }

        if (ModuleData.FisherComplete != !agent->ItemArray[10].IsTurnInAvailable) {
            ModuleData.FisherComplete = !agent->ItemArray[10].IsTurnInAvailable;
            ModuleData.MarkDirty();
        }
    }

    protected override CompletionStatus GetCompletionStatus() {
        if (ModuleConfig.MinerEnabled && !ModuleData.MinerComplete) return CompletionStatus.Incomplete;
        if (ModuleConfig.BotanistEnabled && !ModuleData.BotanistComplete) return CompletionStatus.Incomplete;
        if (ModuleConfig.FisherEnabled && !ModuleData.FisherComplete) return CompletionStatus.Incomplete;
        
        return CompletionStatus.Complete;
    }

    private int GetIncompleteCount() {
        var count = 0;

        if (ModuleConfig.MinerEnabled && !ModuleData.MinerComplete) count++;
        if (ModuleConfig.BotanistEnabled && !ModuleData.BotanistComplete) count++;
        if (ModuleConfig.FisherEnabled && !ModuleData.FisherComplete) count++;

        return count;
    }
}
