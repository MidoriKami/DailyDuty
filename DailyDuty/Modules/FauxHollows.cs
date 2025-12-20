using DailyDuty.Classes;
using DailyDuty.Models;
using DailyDuty.Modules.BaseModules;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;

namespace DailyDuty.Modules;

public class FauxHollowsData : ModuleData {
	public int FauxHollowsCompletions;
	
	protected override void DrawModuleData() {
		DrawDataTable([
			("Faux Hollows Completions", FauxHollowsCompletions.ToString()),
		]);
	}
}

public class FauxHollowsConfig : ModuleConfig {
	public bool IncludeRetelling = true;
	public bool ClickableLink = true;

	protected override void DrawModuleConfig() {
		ConfigChanged |= ImGui.Checkbox("Include Retelling", ref IncludeRetelling);
		ConfigChanged |= ImGui.Checkbox("Clickable Link", ref ClickableLink);
	}
}

public class FauxHollows : BaseModules.Modules.Weekly<FauxHollowsData, FauxHollowsConfig> {
	public override ModuleName ModuleName => ModuleName.FauxHollows;
    
	public override bool HasClickableLink => Config.ClickableLink;
    
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

	protected override StatusMessage GetStatusMessage() => new LinkedStatusMessage {
		LinkEnabled = Config.ClickableLink,
		Message = "Unreal Trial Available",
		Payload = PayloadId.OpenPartyFinder,
	};
}