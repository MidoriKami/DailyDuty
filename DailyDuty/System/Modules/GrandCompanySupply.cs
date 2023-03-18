using DailyDuty.Abstracts;
using DailyDuty.Models.Enums;
using DailyDuty.System.Helpers;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.System;

public class GrandCompanySupply : GrandCompanySupplyProvisionBase
{
    public override ModuleName ModuleName => ModuleName.GrandCompanySupply;

    public override void Load()
    {
        base.Load();

        var luminaUpdater = new LuminaTaskUpdater<ClassJob>(this, job => job.RowId is  >= 8 and <= 15);
        luminaUpdater.UpdateConfig(Config.Tasks);
        luminaUpdater.UpdateData(Data.Tasks);
    }
}