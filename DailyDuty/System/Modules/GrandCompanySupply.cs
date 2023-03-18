using System.Collections.Generic;
using DailyDuty.Abstracts;
using DailyDuty.Models;
using DailyDuty.Models.Attributes;
using DailyDuty.Models.Enums;
using DailyDuty.System.Helpers;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.System;

public class GrandCompanySupplyConfig : ModuleConfigBase
{
    [SelectableTasks]
    public List<LuminaTaskConfig<ClassJob>> Tasks = new();
}

public class GrandCompanySupplyData : ModuleDataBase
{
    [SelectableTasks] 
    public List<LuminaTaskData<ClassJob>> Tasks = new();
}

public class GrandCompanySupply : GrandCompanySupplyProvisionBase
{
    public override ModuleName ModuleName => ModuleName.GrandCompanySupply;

    public override ModuleConfigBase ModuleConfig { get; protected set; } = new GrandCompanySupplyConfig();
    public override ModuleDataBase ModuleData { get; protected set; } = new GrandCompanySupplyData();
    private GrandCompanySupplyConfig Config => ModuleConfig as GrandCompanySupplyConfig ?? new GrandCompanySupplyConfig();
    private GrandCompanySupplyData Data => ModuleData as GrandCompanySupplyData ?? new GrandCompanySupplyData();

    public override void Load()
    {
        base.Load();

        var luminaUpdater = new LuminaTaskUpdater<ClassJob>(this, job => job.RowId is  >= 8 and <= 15);
        luminaUpdater.UpdateConfig(Config.Tasks);
        luminaUpdater.UpdateData(Data.Tasks);
    }
}