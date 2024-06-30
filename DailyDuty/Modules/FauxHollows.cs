using DailyDuty.Classes;
using DailyDuty.Localization;
using DailyDuty.Models;
using DailyDuty.Modules.BaseModules;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using ImGuiNET;

namespace DailyDuty.Modules;

public class FauxHollowsData : ModuleData {
	public int FauxHollowsCompletions;
	
	protected override void DrawModuleData() {
		DrawDataTable([
			(Strings.FauxHollowsCompletions, FauxHollowsCompletions.ToString()),
		]);
	}
}

public class FauxHollowsConfig : ModuleConfig {
	public bool IncludeRetelling = true;
	public bool ClickableLink = true;

	protected override bool DrawModuleConfig() {
		var configChanged = false;

		configChanged |= ImGui.Checkbox(Strings.IncludeRetelling, ref IncludeRetelling);
		configChanged |= ImGui.Checkbox(Strings.ClickableLink, ref ClickableLink);

		return configChanged;
	}
}

public class FauxHollows : Module.WeeklyModule<FauxHollowsData, FauxHollowsConfig> {
	public override ModuleName ModuleName => ModuleName.FauxHollows;
    
	public override bool HasClickableLink => true;
    
	public override PayloadId ClickableLinkPayloadId => PayloadId.IdyllshireTeleport;

	public override void Load() {
		base.Load();
        
		Service.AddonLifecycle.RegisterListener(AddonEvent.PreSetup, "WeeklyPuzzle", WeeklyPuzzlePreSetup);
	}

	public override void Unload() {
		base.Unload();
        
		Service.AddonLifecycle.UnregisterListener(WeeklyPuzzlePreSetup);
	}

	private void WeeklyPuzzlePreSetup(AddonEvent eventType, AddonArgs addonInfo) {
		Data.FauxHollowsCompletions += 1;
		DataChanged = true;
	}

	public override void Reset() {
		Data.FauxHollowsCompletions = 0;
        
		base.Reset();
	}

	protected override ModuleStatus GetModuleStatus() => Config.IncludeRetelling switch {
		true when Data.FauxHollowsCompletions is 2 => ModuleStatus.Complete,
		false when Data.FauxHollowsCompletions is 1 => ModuleStatus.Complete,
		_ => ModuleStatus.Incomplete,
	};

	protected override StatusMessage GetStatusMessage() => 
		ConditionalStatusMessage.GetMessage(Config.ClickableLink, Strings.UnrealTrialAvailable, PayloadId.OpenPartyFinder);
}