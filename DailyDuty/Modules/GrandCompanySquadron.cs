using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using DailyDuty.Classes;
using DailyDuty.Localization;
using DailyDuty.Models;
using DailyDuty.Modules.BaseModules;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.Classes;
using Lumina.Excel.Sheets;
using ValueType = FFXIVClientStructs.FFXIV.Component.GUI.ValueType;

namespace DailyDuty.Modules;

public class GrandCompanySquadronConfig : ModuleConfig;

public class GrandCompanySquadronData : ModuleData {
	public bool MissionCompleted;
	public bool MissionStarted;
	public DateTime MissionCompleteTime = DateTime.MinValue;
	public TimeSpan TimeUntilMissionComplete = TimeSpan.MinValue;

	protected override void DrawModuleData() {
		DrawDataTable(
			(Strings.MissionCompleted, MissionCompleted.ToString()),
			(Strings.MissionStarted, MissionStarted.ToString()),
			(Strings.MissionCompleteTime, MissionCompleteTime.ToLocalTime().ToString(CultureInfo.CurrentCulture)),
			(Strings.TimeUntilMissionComplete, TimeUntilMissionComplete.FormatTimespan())
		);
	}
}

public unsafe partial class GrandCompanySquadron : BaseModules.Modules.Weekly<GrandCompanySquadronData, GrandCompanySquadronConfig> {
	public override ModuleName ModuleName => ModuleName.GrandCompanySquadron;

	private Hook<AgentGcArmyExpedition.Delegates.ReceiveEvent>? onReceiveEventHook;

	[GeneratedRegex("[^\\p{L}\\p{N}]")]
	private static partial Regex Alphanumeric();

	public override void Load() {
		base.Load();

		Service.AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, "GcArmyExpeditionResult", GcArmyExpeditionResultPreFinalize);
                
		onReceiveEventHook ??= Service.Hooker.HookFromAddress<AgentGcArmyExpedition.Delegates.ReceiveEvent>(AgentGcArmyExpedition.Instance()->VirtualTable->ReceiveEvent, OnReceiveEvent);
		onReceiveEventHook?.Enable();
	}

	// The mission is no longer in progress when the window closes
	private void GcArmyExpeditionResultPreFinalize(AddonEvent eventType, AddonArgs addonInfo) {
		var addon = (AtkUnitBase*) addonInfo.Addon;
                
		Data.MissionStarted = false;
		DataChanged = true;

		if (addon->AtkValues[4].Type is not ValueType.String) throw new Exception("Type Mismatch Exception");
		if (addon->AtkValues[2].Type is not ValueType.Int) throw new Exception("Type Mismatch Exception");
                
		var missionText = Alphanumeric().Replace(addon->AtkValues[4].GetValueAsString().ToLower(), string.Empty);
		var missionSuccessful = addon->AtkValues[2].Int == 1;

		var missionInfo = Service.DataManager.GetExcelSheet<GcArmyExpedition>()
			.FirstOrDefault(mission => Alphanumeric().Replace(mission.Name.ToString().ToLower(), string.Empty) == missionText);

		if (missionInfo is { GcArmyExpeditionType.RowId: 3 } && missionSuccessful) {
			Data.MissionCompleted = true;
			DataChanged = true;
		}
	}

	public override void Unload() {
		base.Unload();
                
		Service.AddonLifecycle.UnregisterListener(GcArmyExpeditionResultPreFinalize);
                
		onReceiveEventHook?.Disable();
	}

	public override void Dispose() {
		onReceiveEventHook?.Dispose();
	}

	public override void Reset() {
		Data.MissionCompleted = false;
                
		base.Reset();
	}


	public override void Update() {
		var gcAgent = AgentGcArmyExpedition.Instance();
		
		if (gcAgent->IsAgentActive() && gcAgent->SelectedTab == 2) {
			Data.MissionCompleted = TryUpdateData(Data.MissionCompleted, gcAgent->ExpeditionData->MissionInfo[0].Available == 0);
		}

		if (Data.MissionCompleteTime > DateTime.UtcNow) {
			Data.TimeUntilMissionComplete = Data.MissionCompleteTime - DateTime.UtcNow;
		}
		else {
			Data.TimeUntilMissionComplete = TimeSpan.Zero;
		}
                
		base.Update();
	}
            
	private AtkValue* OnReceiveEvent(AgentGcArmyExpedition* thisPtr, AtkValue* returnValue, AtkValue* args, uint argCount, ulong sender) {
		var result = onReceiveEventHook!.Original(thisPtr, returnValue, args, argCount, sender);
                
		HookSafety.ExecuteSafe(() => {
			if (sender == 1 && args[0].Int == 0) {
				Data.MissionStarted = true;
				var missionCompleteDateTime = DateTime.UtcNow + TimeSpan.FromHours(18);
				Data.MissionCompleteTime = new DateTime(
					missionCompleteDateTime.Year,
					missionCompleteDateTime.Month,
					missionCompleteDateTime.Day,
					missionCompleteDateTime.Hour,
					missionCompleteDateTime.Minute,
					missionCompleteDateTime.Second,
					Data.NextReset.Millisecond,
					Data.NextReset.Microsecond
				);
				DataChanged = true;
			}
		}, Service.Log);

		return result;
	}

	protected override ModuleStatus GetModuleStatus() {
		if (Data.MissionStarted && Data.TimeUntilMissionComplete != TimeSpan.Zero) return ModuleStatus.InProgress;
                
		return Data.MissionCompleted ? ModuleStatus.Complete : ModuleStatus.Incomplete;
	}

	protected override StatusMessage GetStatusMessage() 
		=> Data.MissionStarted && Data.TimeUntilMissionComplete == TimeSpan.Zero ? Strings.MissionCompleted : Strings.MissionAvailable;
}