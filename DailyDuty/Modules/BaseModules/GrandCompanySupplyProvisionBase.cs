using System;
using DailyDuty.Classes;
using DailyDuty.Localization;
using DailyDuty.Models;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Excel.Sheets;

namespace DailyDuty.Modules.BaseModules;

public abstract unsafe class GrandCompanySupplyProvisionBase : Modules.DailyTask<ModuleTaskData<ClassJob>, ModuleTaskConfig<ClassJob>, ClassJob> {
	private static AgentGrandCompanySupply* SupplyAgent => AgentGrandCompanySupply.Instance();

	public override DateTime GetNextReset() 
		=> Time.NextGrandCompanyReset();

	public override void Reset() {
		Data.TaskData.Reset();
        
		base.Reset();
	}

	public override void Update() {
		if (SupplyAgent is not null && SupplyAgent->IsAgentActive()) {
			Data.TaskData.Update(ref DataChanged, rowId => {
				var itemSpan = new Span<GrandCompanyItem>(SupplyAgent->ItemArray, SupplyAgent->NumItems);
				var adjustedIndex = (int)(rowId - 8);
				var agentData = itemSpan[adjustedIndex];

				return !agentData.IsTurnInAvailable;
			});
		}
        
		base.Update();
	}

	protected override ModuleStatus GetModuleStatus()
		=> IncompleteTaskCount == 0 ? ModuleStatus.Complete : ModuleStatus.Incomplete;

	protected override StatusMessage GetStatusMessage() 
		=> $"{IncompleteTaskCount} {Strings.AllowancesRemaining}";
}