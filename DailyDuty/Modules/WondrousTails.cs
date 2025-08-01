﻿using System;
using System.Globalization;
using System.Linq;
using System.Numerics;
using DailyDuty.Classes;
using DailyDuty.Localization;
using DailyDuty.Models;
using DailyDuty.Modules.BaseModules;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI;
using ImGuiNET;

namespace DailyDuty.Modules;

public class WondrousTailsData : ModuleData {
	public int PlacedStickers;
	public uint SecondChance;
	public bool NewBookAvailable;
	public bool PlayerHasBook;
	public DateTime Deadline;
	public TimeSpan TimeRemaining;
	public bool BookExpired;
	public bool CloseToKhloe;
	public float DistanceToKhloe;
	public bool CastingTeleport;
	
	protected override void DrawModuleData() {
		DrawDataTable(
			(Strings.PlacedStickers, PlacedStickers.ToString()), 
			(Strings.SecondChancePoints, SecondChance.ToString()),
			(Strings.NewBookAvailable, NewBookAvailable.ToString()), 
			(Strings.PlayerHasBook, PlayerHasBook.ToString()), 
			(Strings.Deadline, Deadline.ToLocalTime().ToString(CultureInfo.CurrentCulture)), 
			(Strings.TimeRemaining, TimeRemaining.FormatTimespan()), 
			(Strings.BookExpired, BookExpired.ToString()), 
			(Strings.NearKhloe, CloseToKhloe.ToString()), 
			(Strings.DistanceToKhloe, DistanceToKhloe.ToString(CultureInfo.CurrentCulture)), 
			(Strings.CastingTeleport, CastingTeleport.ToString())
		);
	}
}

public class WondrousTailsConfig : ModuleConfig {
	public bool StickerAvailableNotice = true;
	public bool UnclaimedBookWarning = true;
	public bool ShuffleAvailableNotice;
	public bool ClickableLink = true;
	
	protected override void DrawModuleConfig() {
		ConfigChanged |= ImGui.Checkbox(Strings.StickerAvailableNotice, ref StickerAvailableNotice);
		ConfigChanged |= ImGui.Checkbox(Strings.UnclaimedBookWarning, ref UnclaimedBookWarning);
		ConfigChanged |= ImGui.Checkbox(Strings.ShuffleAvailableNotice, ref ShuffleAvailableNotice);
		ConfigChanged |= ImGui.Checkbox(Strings.ClickableLink, ref ClickableLink);
	}
}

public unsafe class WondrousTails : BaseModules.Modules.Weekly<WondrousTailsData, WondrousTailsConfig> {
	public override ModuleName ModuleName => ModuleName.WondrousTails;
    
	public override bool HasClickableLink => Config.ClickableLink;
    
	public override PayloadId ClickableLinkPayloadId => Data.NewBookAvailable ? PayloadId.IdyllshireTeleport : PayloadId.OpenWondrousTailsBook;

	public override void Update() {
		Data.PlacedStickers = TryUpdateData(Data.PlacedStickers, PlayerState.Instance()->WeeklyBingoNumPlacedStickers);
		Data.SecondChance = TryUpdateData(Data.SecondChance, PlayerState.Instance()->WeeklyBingoNumSecondChancePoints);
		Data.Deadline = TryUpdateData(Data.Deadline, DateTimeOffset.FromUnixTimeSeconds(PlayerState.Instance()->GetWeeklyBingoExpireUnixTimestamp()).DateTime);
		Data.PlayerHasBook = TryUpdateData(Data.PlayerHasBook, PlayerState.Instance()->HasWeeklyBingoJournal);
		Data.NewBookAvailable = TryUpdateData(Data.NewBookAvailable, DateTime.UtcNow > Data.Deadline - TimeSpan.FromDays(7));
		Data.BookExpired = TryUpdateData(Data.BookExpired, PlayerState.Instance()->IsWeeklyBingoExpired());
        
		CheckKhloeDistance();

		base.Update();
	}

	private void CheckKhloeDistance() {
		var timeRemaining = Data.Deadline - DateTime.UtcNow;
		Data.TimeRemaining = timeRemaining > TimeSpan.Zero ? timeRemaining : TimeSpan.Zero;
		Data.DistanceToKhloe = 0.0f;
		var lastNearKhloe = Data.CloseToKhloe;
		var lastCastingTeleport = Data.CastingTeleport;
		Data.CloseToKhloe = false;
		Data.CastingTeleport = false;
        
		const int idyllshireTerritoryType = 478;
		const uint khloeAliapohDataId = 1017653;
		if (Service.ClientState.TerritoryType is idyllshireTerritoryType && Config.ModuleEnabled) {
			var khloe = Service.ObjectTable.FirstOrDefault(obj => obj.DataId is khloeAliapohDataId);

			if (khloe is not null && Service.ClientState.LocalPlayer is { Position: var playerPosition }) {
				Data.DistanceToKhloe = Vector3.Distance(playerPosition, khloe.Position);
				Data.CloseToKhloe = Data.DistanceToKhloe < 10.0f;
				Data.CastingTeleport = Service.ClientState.LocalPlayer is { IsCasting: true, CastActionId: 5 or 6 };

				var noLongerNearKhloe = lastNearKhloe && !Data.CloseToKhloe;
				var startedTeleportingAway = lastNearKhloe && !lastCastingTeleport && Data.CastingTeleport;
                
				if ((noLongerNearKhloe || startedTeleportingAway) && Data is { PlayerHasBook: false, NewBookAvailable: true }) {
					new StatusMessage {
						Message = Strings.ForgotBookWarning, MessageChannel = GetChatChannel(), SourceModule = ModuleName,
					}.PrintMessage();
					UIGlobals.PlayChatSoundEffect(11);
				}
			}
		}
	}

	protected override ModuleStatus GetModuleStatus() {
		if (Config.UnclaimedBookWarning && Data.NewBookAvailable) return ModuleStatus.Incomplete;

		return Data.PlacedStickers == 9 ? ModuleStatus.Complete : ModuleStatus.Incomplete;
	}
    
	protected override StatusMessage GetStatusMessage() => Data switch {
		{ PlayerHasBook: true, BookExpired: false } when Config.StickerAvailableNotice && AnyTaskAvailableForSticker() => new LinkedStatusMessage {
			LinkEnabled = Config.ClickableLink,
			Message = Strings.StickerAvailable,
			Payload = PayloadId.OpenWondrousTailsBook,
		},

		{ SecondChance: > 7, PlacedStickers: >= 3 and <= 7, PlayerHasBook: true, BookExpired: false } when Config.ShuffleAvailableNotice  => new LinkedStatusMessage {
			LinkEnabled = Config.ClickableLink,
			Message = Strings.ShuffleAvailable,
			Payload = PayloadId.OpenWondrousTailsBook,
		},

		{ NewBookAvailable: true } when Config.UnclaimedBookWarning  => new LinkedStatusMessage {
			LinkEnabled = Config.ClickableLink,
			Message = Strings.NewBookAvailable,
			Payload = PayloadId.IdyllshireTeleport,
		},

		_  => new LinkedStatusMessage {
			LinkEnabled = Config.ClickableLink,
			Message = string.Format(Strings.StickersRemaining, 9 - Data.PlacedStickers),
			Payload = PayloadId.OpenWondrousTailsBook,
		},
	};

	private static bool AnyTaskAvailableForSticker() 
		=> Enumerable.Range(0, 16).Select(index => PlayerState.Instance()->GetWeeklyBingoTaskStatus(index)).Any(taskStatus => taskStatus == PlayerState.WeeklyBingoTaskStatus.Claimable);
}