using DailyDuty.Abstracts;
using DailyDuty.Models.Enums;
using DailyDuty.System.Helpers;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.System;

public class GrandCompanySupply : GrandCompanySupplyProvisionBase
{
    public override ModuleName ModuleName => ModuleName.GrandCompanySupply;
    
    protected override void UpdateTaskLists()
    {
        var luminaUpdater = new LuminaTaskUpdater<ClassJob>(this, job => job.RowId is  >= 8 and <= 15);
        luminaUpdater.UpdateConfig(Config.TaskConfig);
        luminaUpdater.UpdateData(Data.TaskData);
    }
}