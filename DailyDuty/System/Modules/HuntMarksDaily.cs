using System;
using DailyDuty.Abstracts;
using DailyDuty.Models.Enums;
using DailyDuty.System.Helpers;
using KamiLib.Misc;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.System;

public class HuntMarksDaily : HuntMarksBase
{
    public override ModuleName ModuleName => ModuleName.HuntMarksDaily;
    
    public override TimeSpan GetResetPeriod() => TimeSpan.FromDays(1);
    protected override DateTime GetNextReset() => Time.NextDailyReset();

    public override void Load()
    {
        base.Load();

        var luminaUpdater = new LuminaTaskUpdater<MobHuntOrderType>(this, order => order.Type is 1);
        luminaUpdater.UpdateConfig(Config.Tasks);
        luminaUpdater.UpdateData(Data.Tasks);
    }
}