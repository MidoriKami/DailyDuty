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
    public override ModuleType ModuleType => ModuleType.Daily;

    protected override DateTime GetNextReset() => Time.NextDailyReset() + TimeSpan.FromMinutes(1);

    protected override void UpdateTaskLists()
    {
        var luminaUpdater = new LuminaTaskUpdater<MobHuntOrderType>(this, order => order.Type is 1);
        luminaUpdater.UpdateConfig(Config.TaskConfig);
        luminaUpdater.UpdateData(Data.TaskData);
    }
}