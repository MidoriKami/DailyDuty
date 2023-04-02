using DailyDuty.Abstracts;
using DailyDuty.Models.Enums;
using DailyDuty.System.Helpers;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.System;

public class GrandCompanyProvision : GrandCompanySupplyProvisionBase
{
    public override ModuleName ModuleName => ModuleName.GrandCompanyProvision;
    
    protected override void UpdateTaskLists()
    {
        var luminaUpdater = new LuminaTaskUpdater<ClassJob>(this, job => job.RowId is 16 or 17 or 18);
        luminaUpdater.UpdateConfig(Config.TaskConfig);
        luminaUpdater.UpdateData(Data.TaskData);
    }
}