using System.Collections.Generic;
using System.Linq;
using DailyDuty.Classes;
using DailyDuty.Localization;
using DailyDuty.Models;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Interface.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.Modules.BaseModules;

public class RaidsConfig : ModuleTaskConfig<ContentFinderCondition> {
	public bool ClickableLink = true;
	
	protected override bool DrawModuleConfig() {
		var configChanged = ImGui.Checkbox(Strings.ClickableLink, ref ClickableLink);
		
		ImGuiHelpers.ScaledDummy(5.0f);
		return base.DrawModuleConfig() || configChanged;
	}
}

public interface IChatMessageReceiver {
	void OnChatMessage(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled);
}

public abstract unsafe class RaidsBase : Modules.WeeklyTask<ModuleTaskData<ContentFinderCondition>, RaidsConfig, ContentFinderCondition>, IChatMessageReceiver {
	protected override ModuleStatus GetModuleStatus() => IncompleteTaskCount == 0 ? ModuleStatus.Complete : ModuleStatus.Incomplete;
	private static AgentContentsFinder* Agent => AgentContentsFinder.Instance();
	
	public override bool HasClickableLink => Config.ClickableLink;
	
	public override void Update() {
		if (Agent is not null && Agent->IsAgentActive()) {
			var selectedDuty = Agent->SelectedDutyId;
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

	public void OnChatMessage(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled) {
		// If message is a loot message
		if (((int)type & 0x7F) != 0x3E) return;

		// If we are in a zone that we are tracking
		if (GetDataForCurrentZone() is not { } trackedRaid) return;

		// If the message does NOT contain a player payload
		if (message.Payloads.FirstOrDefault(p => p is PlayerPayload) is PlayerPayload) return;

		// If the message DOES contain an item
		if (message.Payloads.FirstOrDefault(p => p is ItemPayload) is not ItemPayload { Item: { } item } ) return;

		switch (item.ItemUICategory.Row) {
			case 34: // Head
			case 35: // Body
			case 36: // Legs
			case 37: // Hands
			case 38: // Feet
			case 61 when item.ItemAction.Row == 0: // Miscellany with no itemAction
				trackedRaid.CurrentCount += 1;
				DataChanged = true;
				break;
		}
	}
    
	private LuminaTaskData<ContentFinderCondition>? GetDataForCurrentZone()
		=> Data.TaskData.FirstOrDefault(task => task.RowId == GameMain.Instance()->CurrentContentFinderConditionId);

	private bool IsDataStale(ICollection<ContentFinderCondition> dutyList) 
		=> Data.TaskData.Any(task => !dutyList.Any(duty => duty.RowId != task.RowId));
    
	protected void CheckForDutyListUpdate(List<ContentFinderCondition> dutyList) { 
		if (IsDataStale(dutyList) || Config.TaskConfig.Count == 0 || Data.TaskData.Count == 0) {
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