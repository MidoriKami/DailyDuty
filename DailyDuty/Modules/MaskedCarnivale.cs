using System;
using System.Linq;
using DailyDuty.Classes;
using DailyDuty.Localization;
using DailyDuty.Models;
using DailyDuty.Modules.BaseModules;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Interface.Utility;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using ValueType = FFXIVClientStructs.FFXIV.Component.GUI.ValueType;

namespace DailyDuty.Modules;

public class MaskedCarnivaleConfig : ModuleTaskConfigBase<Addon> {
	public bool ClickableLink = true;
	
	protected override bool DrawModuleConfig() {
		var configChanged = ImGui.Checkbox(Strings.ClickableLink, ref ClickableLink);
		
		ImGuiHelpers.ScaledDummy(5.0f);
		return base.DrawModuleConfig() || configChanged;
	}
}

public unsafe class MaskedCarnivale : Module.WeeklyTaskModule<ModuleTaskDataBase<Addon>, MaskedCarnivaleConfig, Addon> {
	public override ModuleName ModuleName => ModuleName.MaskedCarnivale;

	public override bool HasClickableLink => true;
    
	public override PayloadId ClickableLinkPayloadId => PayloadId.UldahTeleport;
    
	public override bool HasTooltip => true;
    
	public override void Load() {
		base.Load();
        
		Service.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, "AOZContentResult", AozContentResultPostSetup);
	}

	public override void Unload() {
		base.Unload();
        
		Service.AddonLifecycle.UnregisterListener(AozContentResultPostSetup);
	}

	protected override void UpdateTaskLists() {
		var luminaTaskUpdater = new LuminaTaskUpdater<Addon>(this, addon => addon.RowId is 12449 or 12448 or 12447);
		luminaTaskUpdater.UpdateConfig(Config.TaskConfig);
		luminaTaskUpdater.UpdateData(Data.TaskData);
	}

	public override void Update() {
		if (AgentAozContentBriefing.Instance() is not null && AgentAozContentBriefing.Instance()->IsAgentActive()) {
			foreach (var task in Data.TaskData) {
				var status = task.RowId switch {
					12449 => AgentAozContentBriefing.Instance()->IsWeeklyChallengeComplete(AozWeeklyChallenge.Novice),
					12448 => AgentAozContentBriefing.Instance()->IsWeeklyChallengeComplete(AozWeeklyChallenge.Moderate),
					12447 => AgentAozContentBriefing.Instance()->IsWeeklyChallengeComplete(AozWeeklyChallenge.Advanced),
					_ => throw new ArgumentOutOfRangeException(),
				};

				if (task.Complete != status) {
					task.Complete = status;
					DataChanged = true;
				}
			}
		}
        
		base.Update();
	}

	private void AozContentResultPostSetup(AddonEvent eventType, AddonArgs addonInfo) {
		var addon = (AtkUnitBase*) addonInfo.Addon;
        
		if (addon->AtkValues[112] is not { Type: ValueType.UInt, UInt: var completionIndex }) throw new Exception("Type Mismatch Exception");
		if (addon->AtkValues[114] is not { Type: ValueType.Bool, Byte: var completionStatus }) throw new Exception("Type Mismatch Exception");
        
		var addonId = completionIndex switch {
			0 => 12449,
			1 => 12448,
			2 => 12447,

			_ => throw new ArgumentOutOfRangeException(),
		};

		var task = Data.TaskData.FirstOrDefault(task => task.RowId == addonId);

		if (task is not null && task.Complete != (completionStatus != 0)) {
			task.Complete = (completionStatus != 0);
			DataChanged = true;
		}
	}

	public override void Reset() {
		foreach (var task in Data.TaskData) {
			task.Complete = false;
		}
        
		base.Reset();
	}

	protected override ModuleStatus GetModuleStatus() 
		=> IncompleteTaskCount == 0 ? ModuleStatus.Complete : ModuleStatus.Incomplete;

	protected override StatusMessage GetStatusMessage() {
		var message = $"{IncompleteTaskCount} {Strings.ChallengesRemaining}";

		return ConditionalStatusMessage.GetMessage(Config.ClickableLink, message, PayloadId.UldahTeleport);
	}
}