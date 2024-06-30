using System.Linq;
using DailyDuty.Classes;
using DailyDuty.Localization;
using DailyDuty.Models;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.Modules.BaseModules;

public abstract unsafe class HuntMarksBase : Module.SpecialTaskModule<ModuleTaskData<MobHuntOrderType>, ModuleTaskConfig<MobHuntOrderType>, MobHuntOrderType> {
	private static MobHunt* HuntData => MobHunt.Instance();
    
	public override void Update() {
		foreach (var task in Data.TaskData) {
			// If we have the active mark bill
			var availableMarkId = HuntData->GetAvailableHuntOrderRowId((byte) task.RowId);
			var obtainedMarkId = HuntData->GetObtainedHuntOrderRowId((byte) task.RowId);
                
			if (availableMarkId == obtainedMarkId && !task.Complete) {
				var orderData = Service.DataManager.GetExcelSheet<MobHuntOrderType>()!.GetRow(task.RowId)!;
				var targetRow = orderData.OrderStart.Row + HuntData->ObtainedMarkId[(int) task.RowId] - 1;
                
				// Elite
				if (orderData.Type is 2 && IsEliteMarkComplete(targetRow, task.RowId)) {
					task.Complete = true;
					DataChanged = true;
				}
				// Regular Hunt
				else if (orderData.Type is 1 && IsNormalMarkComplete(targetRow, task.RowId)) {
					task.Complete = true;
					DataChanged = true;
				}
			}
		}
        
		base.Update();
	}

	private bool IsEliteMarkComplete(uint targetRow, uint markId) {
		var eliteTargetInfo = Service.DataManager.GetExcelSheet<MobHuntOrder>()!.GetRow(targetRow, 0)!;

		return HuntData->CurrentKills[(int) markId][0] == eliteTargetInfo.NeededKills;
	}

	private bool IsNormalMarkComplete(uint targetRow, uint markId) {
		var allTargetsKilled = Enumerable.Range(0, 5).All(index => {
			var huntData = Service.DataManager.GetExcelSheet<MobHuntOrder>()!.GetRow(targetRow, (uint) index)!;

			return HuntData->CurrentKills[(int) markId][index] == huntData.NeededKills;
		});

		return allTargetsKilled;
	}

	public override void Reset() {
		Data.TaskData.Reset();
        
		base.Reset();
	}

	protected override ModuleStatus GetModuleStatus() 
		=> IncompleteTaskCount == 0 ? ModuleStatus.Complete : ModuleStatus.Incomplete;

	protected override StatusMessage GetStatusMessage() => new() {
		Message = $"{IncompleteTaskCount} {Strings.HuntsRemaining}",
	};
}