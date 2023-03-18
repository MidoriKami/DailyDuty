using System.Collections.Generic;
using DailyDuty.Abstracts;
using DailyDuty.Models;
using DailyDuty.Models.Attributes;
using DailyDuty.Models.Enums;
using DailyDuty.System.Helpers;
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

public class GrandCompanyProvision : GrandCompanySupplyProvisionBase
{
    public override ModuleName ModuleName => ModuleName.GrandCompanyProvision;

    public override ModuleConfigBase ModuleConfig { get; protected set; } = new GrandCompanyProvisionConfig();
    public override ModuleDataBase ModuleData { get; protected set; } = new GrandCompanyProvisionData();
    private GrandCompanyProvisionConfig Config => ModuleConfig as GrandCompanyProvisionConfig ?? new GrandCompanyProvisionConfig();
    private GrandCompanyProvisionData Data => ModuleData as GrandCompanyProvisionData ?? new GrandCompanyProvisionData();

    public override void Load()
    {
        base.Load();

        var luminaUpdater = new LuminaTaskUpdater<ClassJob>(this, job => job.RowId is 16 or 17 or 18);
        luminaUpdater.UpdateConfig(Config.Tasks);
        luminaUpdater.UpdateData(Data.Tasks);
    }
}