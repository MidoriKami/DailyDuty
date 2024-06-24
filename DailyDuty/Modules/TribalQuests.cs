using DailyDuty.Classes;
using DailyDuty.Localization;
using DailyDuty.Models;
using DailyDuty.Modules.BaseModules;
using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiNET;
using KamiLib.Components;

namespace DailyDuty.Modules;

public class TribalQuestsData : ModuleDataBase {
	public uint RemainingAllowances;
	
	protected override void DrawModuleData() {
		DrawDataTable([
			(Strings.AllowancesRemaining, RemainingAllowances.ToString()),
		]);
	}
}

public class TribalQuestsConfig : ModuleConfigBase {
	public int NotificationThreshold = 12;
	public ComparisonMode ComparisonMode = ComparisonMode.LessThan;
	
	protected override bool DrawModuleConfig() {
		var configChanged = false;

		ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X / 2.0f);
		configChanged |= ImGuiTweaks.EnumCombo(Strings.ComparisonMode, ref ComparisonMode, Strings.ResourceManager);
        
		ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X / 2.0f);
		configChanged |= ImGui.SliderInt(Strings.NotificationThreshold, ref NotificationThreshold, 1, 12);

		return configChanged;
	}
}

public unsafe class TribalQuests : Module.DailyModule<TribalQuestsData, TribalQuestsConfig> {
	public override ModuleName ModuleName => ModuleName.TribalQuests;

	public override void Update() {
		Data.RemainingAllowances = TryUpdateData(Data.RemainingAllowances, QuestManager.Instance()->GetBeastTribeAllowance());
        
		base.Update();
	}

	public override void Reset() {
		Data.RemainingAllowances = 12;
        
		base.Reset();
	}

	protected override ModuleStatus GetModuleStatus() => Config.ComparisonMode switch {
		ComparisonMode.LessThan when Config.NotificationThreshold > Data.RemainingAllowances => ModuleStatus.Complete,
		ComparisonMode.EqualTo when Config.NotificationThreshold == Data.RemainingAllowances => ModuleStatus.Complete,
		ComparisonMode.LessThanOrEqual when Config.NotificationThreshold >= Data.RemainingAllowances => ModuleStatus.Complete,
		_ => ModuleStatus.Incomplete,
	};
    
	protected override StatusMessage GetStatusMessage() => new() {
		Message = $"{Data.RemainingAllowances} {Strings.AllowancesRemaining}",
	};
}