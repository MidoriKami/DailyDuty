using System;
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
    public LuminaTaskConfigList<ClassJob> TaskConfig = new();
}

public class GrandCompanySupplyProvisioningData : ModuleDataBase
{
    [SelectableTasks] 
    public LuminaTaskDataList<ClassJob> TaskData = new();
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
        Data.TaskData.Reset();
        
        base.Reset();
    }

    public override void Update()
    {
        if (SupplyAgent is not null && SupplyAgent->AgentInterface.IsAgentActive())
        {
            Data.TaskData.Update(ref DataChanged, rowId =>
            {
                var itemSpan = new Span<GrandCompanyItem>(SupplyAgent->ItemArray, SupplyAgent->NumItems);
                var adjustedIndex = (int)(rowId - 8);
                var agentData = itemSpan[adjustedIndex];

                return !agentData.IsTurnInAvailable;
            });
        }
        
        base.Update();
    }
    
    protected override ModuleStatus GetModuleStatus() => GetIncompleteCount(Config.TaskConfig, Data.TaskData) == 0 ? ModuleStatus.Complete : ModuleStatus.Incomplete;

    protected override StatusMessage GetStatusMessage() => new()
    {
        Message = $"{GetIncompleteCount(Config.TaskConfig, Data.TaskData)} {Strings.AllowancesRemaining}",
    };
}