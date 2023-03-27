using System;
using System.Collections.Generic;
using DailyDuty.Models;
using DailyDuty.Models.Attributes;
using DailyDuty.Models.Enums;
using DailyDuty.System.Localization;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using KamiLib.Misc;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.Abstracts;

public class GrandCompanySupplyProvisioningConfig : ModuleConfigBase
{
    [SelectableTasks]
    public List<LuminaTaskConfig<ClassJob>> Tasks = new();
}

public class GrandCompanySupplyProvisioningData : ModuleDataBase
{
    [SelectableTasks] 
    public List<LuminaTaskData<ClassJob>> Tasks = new();
}

public abstract unsafe class GrandCompanySupplyProvisionBase : Module.DailyModule
{
    public override ModuleConfigBase ModuleConfig { get; protected set; } = new GrandCompanySupplyProvisioningConfig();
    public override ModuleDataBase ModuleData { get; protected set; } = new GrandCompanySupplyProvisioningData();
    protected GrandCompanySupplyProvisioningConfig Config => ModuleConfig as GrandCompanySupplyProvisioningConfig ?? new GrandCompanySupplyProvisioningConfig();
    protected GrandCompanySupplyProvisioningData Data => ModuleData as GrandCompanySupplyProvisioningData ?? new GrandCompanySupplyProvisioningData();

    private AgentGrandCompanySupply* SupplyAgent => (AgentGrandCompanySupply*) AgentModule.Instance()->GetAgentByInternalId(AgentId.GrandCompanySupply);

    protected override DateTime GetNextReset() => Time.NextGrandCompanyReset();

    public override void Reset()
    {
        foreach (var data in Data.Tasks)
        {
            data.Complete = false;
        }
        
        base.Reset();
    }

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
    
    protected override ModuleStatus GetModuleStatus() => GetIncompleteCount(Config.Tasks, Data.Tasks) == 0 ? ModuleStatus.Complete : ModuleStatus.Incomplete;

    protected override StatusMessage GetStatusMessage() => new()
    {
        Message = $"{GetIncompleteCount(Config.Tasks, Data.Tasks)} {Strings.AllowancesRemaining}",
    };
}