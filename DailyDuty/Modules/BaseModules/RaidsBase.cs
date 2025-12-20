using System.Collections.Generic;
using System.Linq;
using DailyDuty.Classes;
using DailyDuty.Models;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Inventory;
using Dalamud.Game.Inventory.InventoryEventArgTypes;
using Dalamud.Interface.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Excel.Sheets;

namespace DailyDuty.Modules.BaseModules;

public class RaidsConfig : ModuleTaskConfig<ContentFinderCondition> {
	public bool ClickableLink = true;
	
	protected override void DrawModuleConfig() {
		ConfigChanged |= ImGui.Checkbox("Clickable Link", ref ClickableLink);
		
		ImGuiHelpers.ScaledDummy(5.0f);
		
		base.DrawModuleConfig();
	}
}

public abstract unsafe class RaidsBase : Modules.WeeklyTask<ModuleTaskData<ContentFinderCondition>, RaidsConfig, ContentFinderCondition> {
	protected override ModuleStatus GetModuleStatus() => IncompleteTaskCount == 0 ? ModuleStatus.Complete : ModuleStatus.Incomplete;
	private static AgentContentsFinder* Agent => AgentContentsFinder.Instance();
	
	public override bool HasClickableLink => Config.ClickableLink;

	protected abstract List<ContentFinderCondition> RaidDuties { get; set; }

	protected RaidsBase() {
		Service.GameInventory.ItemAdded += OnItemEvent;
		Service.GameInventory.ItemChanged += OnItemEvent;
	}

	public override void Dispose() {
		Service.GameInventory.ItemAdded -= OnItemEvent;
		Service.GameInventory.ItemChanged -= OnItemEvent;
	}

	protected override void UpdateTaskLists() {
		CheckForDutyListUpdate(RaidDuties);
	}

	public override void Update() {
		if (Agent is not null && Agent->IsAgentActive()) {
			var selectedDuty = Agent->SelectedDuty.Id;
			var task = Data.TaskData.FirstOrDefault(task => task.RowId == selectedDuty);
			var numRewards = Agent->NumCollectedRewards;
            
			if (task is not null && task.CurrentCount != numRewards) {
				task.CurrentCount = numRewards;
				DataChanged = true;
			}
		}
        
		base.Update();
	}

	public override void Reset() {
		Data.TaskData.Reset();
        
		base.Reset();
	}

	private void OnItemEvent(GameInventoryEvent type, InventoryEventArgs data) {
		// If the item event is not for main inventory, we don't care.
		if (data.Item.ContainerType is not (GameInventoryType.Inventory1 or GameInventoryType.Inventory2 or GameInventoryType.Inventory3 or GameInventoryType.Inventory4)) return;
		
		// If we are not in a tracked zone, return
		if (GetDataForCurrentZone() is not { } trackedRaid) return;

		// If we can't get the exd data for this item, return
		var item = Service.DataManager.GetExcelSheet<Item>().GetRow(data.Item.ItemId);
		if (item.RowId is 0) return;
		
		Service.Log.Debug($"InventoryEvent: {type}: {item.Name}");

		// If the item is a limited type that we care about, increment the current count
		switch (item.ItemUICategory.RowId) {
			case 34: // Head
			case 35: // Body
			case 36: // Legs
			case 37: // Hands
			case 38: // Feet
			case 61 when item.ItemAction.RowId == 0: // Miscellany with no itemAction
				trackedRaid.CurrentCount += 1;
				DataChanged = true;
				break;
		}
	}
    
	private LuminaTaskData<ContentFinderCondition>? GetDataForCurrentZone()
		=> Data.TaskData.FirstOrDefault(task => task.RowId == GameMain.Instance()->CurrentContentFinderConditionId);

	private bool IsDataStale(ICollection<ContentFinderCondition> dutyList) {
		// Are there any new duties that we might need to add?
		var newDutiesAvailable = dutyList.Any(duty => !Data.TaskData.Any(task => task.RowId == duty.RowId));
		
		// Are there any duties that we might have that we need to remove?
		var tooManyDuties = false;
		
		// Check every duty in saved task data
		foreach (var taskDataDuty in Data.TaskData) {
			
			// If this duty doesn't match ANY of the new duties, then we have too many.
			if (!dutyList.Any(newDuty => newDuty.RowId == taskDataDuty.RowId)) {
				tooManyDuties = true;
			}
			// else this duty is in the list, it's good to keep.
		}
		
		return newDutiesAvailable || tooManyDuties;
	}

	private void CheckForDutyListUpdate(List<ContentFinderCondition> dutyList) { 
		if (IsDataStale(dutyList) || Config.TaskConfig.Count is 0 || Data.TaskData.Count is 0) {
			Config.TaskConfig.Clear();
			Data.TaskData.Clear();
	
			foreach (var duty in dutyList) {
				Config.TaskConfig.Add(new LuminaTaskConfig<ContentFinderCondition> {
					RowId = duty.RowId,
					Enabled = false,
					TargetCount = 0,
				});
			             
				Data.TaskData.Add(new LuminaTaskData<ContentFinderCondition> {
					RowId = duty.RowId,
					Complete = false,
					CurrentCount = 0,
				});
			             
				SaveConfig();
				SaveData();
			}
		}
	}
}