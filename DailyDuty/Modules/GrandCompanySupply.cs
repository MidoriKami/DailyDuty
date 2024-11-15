using DailyDuty.Classes;
using DailyDuty.Modules.BaseModules;
using Lumina.Excel.Sheets;

namespace DailyDuty.Modules;

public class GrandCompanySupply : GrandCompanySupplyProvisionBase {
	public override ModuleName ModuleName => ModuleName.GrandCompanySupply;

	protected override void UpdateTaskLists() {
		var luminaUpdater = new LuminaTaskUpdater<ClassJob>(this, job => job.RowId is  >= 8 and <= 15);
		luminaUpdater.UpdateConfig(Config.TaskConfig);
		luminaUpdater.UpdateData(Data.TaskData);
	}
}