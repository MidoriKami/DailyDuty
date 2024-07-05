using System;
using System.ComponentModel;
using DailyDuty.Classes;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.Models;
using DailyDuty.Modules.BaseModules;
using ImGuiNET;
using KamiLib.Classes;

namespace DailyDuty.Modules;

public enum FashionReportMode {
	[Description("All")]
	All,
    
	[Description("Single")]
	Single,
    
	[Description("Plus80")]
	Plus80,
}

public class FashionReportData : ModuleData {
	public int AllowancesRemaining = 4;
	public int HighestWeeklyScore;
	public bool FashionReportAvailable;

	protected override void DrawModuleData() {
		DrawDataTable([
			(Strings.AllowancesRemaining, AllowancesRemaining.ToString()),
			(Strings.HighestWeeklyScore, HighestWeeklyScore.ToString()),
			(Strings.FashionReportAvailable, FashionReportAvailable.ToString()),
		]);
	}
}

public class FashionReportConfig : ModuleConfig {
	public FashionReportMode CompletionMode = FashionReportMode.Single;
	public bool ClickableLink = true;

	protected override bool DrawModuleConfig() {
		var configChanged = false;

		ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X / 2.0f);
		configChanged |= ImGuiTweaks.EnumCombo(Strings.CompletionMode, ref CompletionMode, Strings.ResourceManager);
		configChanged |= ImGui.Checkbox(Strings.ClickableLink, ref ClickableLink);

		return configChanged;
	}
}

public unsafe class FashionReport : Modules.Special<FashionReportData, FashionReportConfig>, IGoldSaucerMessageReceiver {
	public override ModuleName ModuleName => ModuleName.FashionReport;

	public override bool HasClickableLink => Config.ClickableLink;
    
	public override PayloadId ClickableLinkPayloadId => PayloadId.GoldSaucerTeleport;

	public override TimeSpan GetModulePeriod() => TimeSpan.FromDays(7);

	public override void Update() {
		var reportOpen = Time.NextWeeklyReset().AddDays(-4);
		var reportClosed = Time.NextWeeklyReset();
		var now = DateTime.UtcNow;

		Data.FashionReportAvailable = TryUpdateData(Data.FashionReportAvailable, now > reportOpen && now < reportClosed);
        
		base.Update();
	}

	public override void Reset() {
		Data.HighestWeeklyScore = 0;
		Data.AllowancesRemaining = 4;
		Data.FashionReportAvailable = false;
        
		base.Reset();
	}

	public override DateTime GetNextReset() => Time.NextFashionReportReset();

	protected override ModuleStatus GetModuleStatus() {
		if (Data.FashionReportAvailable == false) return ModuleStatus.Unavailable;

		return Config.CompletionMode switch {
			FashionReportMode.Single when Data.AllowancesRemaining < 4 => ModuleStatus.Complete,
			FashionReportMode.All when Data.AllowancesRemaining is 0 => ModuleStatus.Complete,
			FashionReportMode.Plus80 when Data.HighestWeeklyScore >= 80 => ModuleStatus.Complete,
			_ => ModuleStatus.Incomplete,
		};
	}

	protected override StatusMessage GetStatusMessage() => ConditionalStatusMessage.GetMessage(
		Config.ClickableLink,
		Config.CompletionMode switch {
			FashionReportMode.All => $"{Data.AllowancesRemaining} {Strings.AllowancesAvailable}",
			FashionReportMode.Single when Data.AllowancesRemaining == 4 => $"{Data.AllowancesRemaining} {Strings.AllowancesAvailable}",
			FashionReportMode.Plus80 when Data.HighestWeeklyScore <= 80 => $"{Data.HighestWeeklyScore} {Strings.HighestScore}",
			_ => throw new ArgumentOutOfRangeException(),
		},
		PayloadId.GoldSaucerTeleport
	);
    
	public void GoldSaucerUpdate(object? sender, GoldSaucerEventArgs data) {
		const int maskedRoseId = 1025176;
		if (Service.TargetManager.Target?.DataId != maskedRoseId) return;

		var allowances = Data.AllowancesRemaining;
		var score = Data.HighestWeeklyScore;

		switch (data.EventId) {
			case 5:     // When speaking to Masked Rose, gets update information
				allowances = data.Data[1];
				score = data.Data[0];
				break;

			case 3:     // During turn in, gets new score
				score = Math.Max(data.Data[0], score);
				break;
                    
			case 1:     // During turn in, gets new allowances
				allowances = data.Data[0];
				break;
		}
        
		Data.AllowancesRemaining = TryUpdateData(Data.AllowancesRemaining, allowances);
		Data.HighestWeeklyScore = TryUpdateData(Data.HighestWeeklyScore, score);
	}
}