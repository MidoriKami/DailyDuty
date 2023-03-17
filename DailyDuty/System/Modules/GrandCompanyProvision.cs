using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.Abstracts;
using DailyDuty.Models;
using DailyDuty.Models.Attributes;
using DailyDuty.Models.Enums;
using DailyDuty.System.Helpers;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using KamiLib.Misc;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.System;

public class GrandCompanyProvisionConfig : ModuleConfigBase
{
    [SelectableTasks]
    public List<LuminaTaskConfig<ClassJob>> Tasks = new();
}

public class GrandCompanyProvisionData : ModuleDataBase
{
    [SelectableTasks] 
    public List<LuminaTaskData<ClassJob>> Tasks = new();
}

public unsafe class GrandCompanyProvision : Module.DailyModule
{
    public override ModuleName ModuleName => ModuleName.GrandCompanyProvision;

    public override ModuleConfigBase ModuleConfig { get; protected set; } = new GrandCompanyProvisionConfig();
    public override ModuleDataBase ModuleData { get; protected set; } = new GrandCompanyProvisionData();
    private GrandCompanyProvisionConfig Config => ModuleConfig as GrandCompanyProvisionConfig ?? new GrandCompanyProvisionConfig();
    private GrandCompanyProvisionData Data => ModuleData as GrandCompanyProvisionData ?? new GrandCompanyProvisionData();

    private AgentGrandCompanySupply* SupplyAgent => (AgentGrandCompanySupply*) AgentModule.Instance()->GetAgentByInternalId(AgentId.GrandCompanySupply);

    protected override DateTime GetNextReset() => Time.NextGrandCompanyReset();

    public override void Load()
    {
        base.Load();

        var luminaUpdater = new LuminaTaskUpdater<ClassJob>(this, job => job.RowId is 16 or 17 or 18);
        luminaUpdater.UpdateConfig(Config.Tasks);
        luminaUpdater.UpdateData(Data.Tasks);
    }

    public override void Update()
    {
        if (SupplyAgent is null) return;
        if (!SupplyAgent->AgentInterface.IsAgentActive()) return;

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