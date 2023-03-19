using System;
using DailyDuty.Abstracts;
using DailyDuty.Models.Enums;
using DailyDuty.System.Helpers;
using KamiLib.Misc;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.System;

public class HuntMarksWeekly : HuntMarksBase
{
    public override ModuleName ModuleName => ModuleName.HuntMarksWeekly;
    
    public override TimeSpan GetResetPeriod() => TimeSpan.FromDays(7);
    protected override DateTime GetNextReset() => Time.NextWeeklyReset();
    
    public override void Load()
    {
        base.Load();

        var luminaUpdater = new LuminaTaskUpdater<MobHuntOrderType>(this, order => order.Type is 2);
        luminaUpdater.UpdateConfig(Config.Tasks);
        luminaUpdater.UpdateData(Data.Tasks);
    }
}