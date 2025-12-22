// using DailyDuty.Classes;
// using DailyDuty.Modules.BaseModules;
// using Lumina.Excel.Sheets;
//
// namespace DailyDuty.Modules;
//
// public class GrandCompanyProvision : GrandCompanySupplyProvisionBase {
// 	public override ModuleName ModuleName => ModuleName.GrandCompanyProvision;
//
// 	protected override void UpdateTaskLists() {
// 		var luminaUpdater = new LuminaTaskUpdater<ClassJob>(this, job => job.RowId is 16 or 17 or 18);
// 		luminaUpdater.UpdateConfig(Config.TaskConfig);
// 		luminaUpdater.UpdateData(Data.TaskData);
// 	}
// }
