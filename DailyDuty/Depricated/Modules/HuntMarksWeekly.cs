// using System;
// using DailyDuty.Classes;
// using DailyDuty.Modules.BaseModules;
// using Lumina.Excel.Sheets;
//
// namespace DailyDuty.Modules;
//
// public class HuntMarksWeekly : HuntMarksBase {
// 	public override ModuleName ModuleName => ModuleName.HuntMarksWeekly;
// 	
// 	public override ModuleType ModuleType => ModuleType.Weekly;
//
// 	public override DateTime GetNextReset() => Time.NextWeeklyReset() + TimeSpan.FromMinutes(1);
//
// 	protected override void UpdateTaskLists() {
// 		var luminaUpdater = new LuminaTaskUpdater<MobHuntOrderType>(this, order => order.Type is 2);
// 		luminaUpdater.UpdateConfig(Config.TaskConfig);
// 		luminaUpdater.UpdateData(Data.TaskData);
// 	}
// }
