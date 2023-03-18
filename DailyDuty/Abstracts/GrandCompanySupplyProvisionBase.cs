using System;
using System.Linq;
using DailyDuty.Models;
using DailyDuty.Models.Enums;
using DailyDuty.System;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using KamiLib.Misc;

namespace DailyDuty.Abstracts;

public abstract unsafe class GrandCompanySupplyProvisionBase : Module.DailyModule
{
    public override ModuleConfigBase ModuleConfig { get; protected set; } = new GrandCompanySupplyConfig();
    public override ModuleDataBase ModuleData { get; protected set; } = new GrandCompanySupplyData();
    private GrandCompanySupplyConfig Config => ModuleConfig as GrandCompanySupplyConfig ?? new GrandCompanySupplyConfig();
    private GrandCompanySupplyData Data => ModuleData as GrandCompanySupplyData ?? new GrandCompanySupplyData();

    private AgentGrandCompanySupply* SupplyAgent => (AgentGrandCompanySupply*) AgentModule.Instance()->GetAgentByInternalId(AgentId.GrandCompanySupply);

    protected override DateTime GetNextReset() => Time.NextGrandCompanyReset();
    
    public override void Update()
    {
        if (SupplyAgent is not null && SupplyAgent->AgentInterface.IsAgentActive())
        {
            var itemSpan = new Span<GrandCompanyItem>(SupplyAgent->ItemArray, SupplyAgent->NumItems);
        
            foreach (var data in Data.Tasks)
            {
                var adjustedIndex = (int)(data.RowId - 8);
                var agentData = itemSpan[adjustedIndex];

                if (data.Complete != !agentData.IsTurnInAvailable)
                {
                    data.Complete = !agentData.IsTurnInAvailable;
                    DataChanged = true;
                }
            }
        }
        
        base.Update();
    }
    
    protected override ModuleStatus GetModuleStatus() => GetIncompleteCount() == 0 ? ModuleStatus.Complete : ModuleStatus.Incomplete;

    protected override StatusMessage GetStatusMessage() => new()
    {
        Message = $"{GetIncompleteCount()} Turn-ins Available",
    };

    private int GetIncompleteCount()
    {
        var taskData = from config in Config.Tasks
            join data in Data.Tasks on config.RowId equals data.RowId
            where config.Enabled
            where !data.Complete
            select new
            {
                config.RowId,
                config.Enabled,
                data.Complete
            };

        return taskData.Count();
    }
}