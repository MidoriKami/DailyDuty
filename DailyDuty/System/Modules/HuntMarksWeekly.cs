using System;
using DailyDuty.Abstracts;
using DailyDuty.Models.Enums;
using DailyDuty.System.Helpers;
using KamiLib.Game;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.System;

public class HuntMarksWeekly : HuntMarksBase
{
    public override ModuleName ModuleName => ModuleName.HuntMarksWeekly;
    public override ModuleType ModuleType => ModuleType.Weekly;

    protected override DateTime GetNextReset() => Time.NextWeeklyReset() + TimeSpan.FromMinutes(1);
    
    protected override void UpdateTaskLists()
    {
        var luminaUpdater = new LuminaTaskUpdater<MobHuntOrderType>(this, order => order.Type is 2);
        luminaUpdater.UpdateConfig(Config.TaskConfig);
        luminaUpdater.UpdateData(Data.TaskData);
    }
}