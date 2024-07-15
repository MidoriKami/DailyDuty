using System;
using DailyDuty.Classes;
using DailyDuty.Modules.BaseModules;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.Modules;

public class HuntMarksDaily : HuntMarksBase {
	public override ModuleName ModuleName => ModuleName.HuntMarksDaily;

	public override ModuleType ModuleType => ModuleType.Daily;

	public override DateTime GetNextReset() => Time.NextDailyReset() + TimeSpan.FromMinutes(1);

	public override TimeSpan GetModulePeriod() => TimeSpan.FromDays(1);

	protected override void UpdateTaskLists() {
		var luminaUpdater = new LuminaTaskUpdater<MobHuntOrderType>(this, order => order.Type is 1);
		luminaUpdater.UpdateConfig(Config.TaskConfig);
		luminaUpdater.UpdateData(Data.TaskData);
	}
}