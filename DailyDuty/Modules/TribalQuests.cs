﻿using DailyDuty.Classes;
using DailyDuty.Localization;
using DailyDuty.Models;
using DailyDuty.Modules.BaseModules;
using Dalamud.Bindings.ImGui;
using FFXIVClientStructs.FFXIV.Client.Game;
using KamiLib.Classes;

namespace DailyDuty.Modules;

public class TribalQuestsData : ModuleData {
	public uint RemainingAllowances;
	
	protected override void DrawModuleData() {
		DrawDataTable([
			(Strings.AllowancesRemaining, RemainingAllowances.ToString()),
		]);
	}
}

public class TribalQuestsConfig : ModuleConfig {
	public int NotificationThreshold = 12;
	public ComparisonMode ComparisonMode = ComparisonMode.LessThan;
	
	protected override void DrawModuleConfig() {
		ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X / 2.0f);
		ConfigChanged |= ImGuiTweaks.EnumCombo(Strings.ComparisonMode, ref ComparisonMode);
        
		ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X / 2.0f);
		ConfigChanged |= ImGui.SliderInt(Strings.NotificationThreshold, ref NotificationThreshold, 1, 12);
	}
}

public unsafe class TribalQuests : BaseModules.Modules.Daily<TribalQuestsData, TribalQuestsConfig> {
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

	protected override StatusMessage GetStatusMessage() 
		=> $"{Data.RemainingAllowances} {Strings.AllowancesRemaining}";
}