using System;
using System.Collections.Generic;
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
using Lumina.Excel.GeneratedSheets;

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
		DrawDataTable([
			(Strings.PlacedStickers, PlacedStickers.ToString()),
			(Strings.SecondChancePoints, SecondChance.ToString()),
			(Strings.NewBookAvailable, NewBookAvailable.ToString()),
			(Strings.PlayerHasBook, PlayerHasBook.ToString()),
			(Strings.Deadline, Deadline.ToLocalTime().ToString(CultureInfo.CurrentCulture)),
			(Strings.TimeRemaining, TimeRemaining.FormatTimespan()),
			(Strings.BookExpired, BookExpired.ToString()),
			(Strings.NearKhloe, CloseToKhloe.ToString()),
			(Strings.DistanceToKhloe, DistanceToKhloe.ToString(CultureInfo.CurrentCulture)),
			(Strings.CastingTeleport, CastingTeleport.ToString()),
		]);
	}
}

public class WondrousTailsConfig : ModuleConfig {
	public bool InstanceNotifications = true;
	public bool StickerAvailableNotice = true;
	public bool UnclaimedBookWarning = true;
	public bool ShuffleAvailableNotice;
	public bool ClickableLink = true;
	
	protected override bool DrawModuleConfig() {
		var configChanged = false;

		configChanged |= ImGui.Checkbox(Strings.InstanceNotifications, ref InstanceNotifications);
		configChanged |= ImGui.Checkbox(Strings.StickerAvailableNotice, ref StickerAvailableNotice);
		configChanged |= ImGui.Checkbox(Strings.UnclaimedBookWarning, ref UnclaimedBookWarning);
		configChanged |= ImGui.Checkbox(Strings.ShuffleAvailableNotice, ref ShuffleAvailableNotice);
		configChanged |= ImGui.Checkbox(Strings.ClickableLink, ref ClickableLink);
		
		return configChanged;
	}
}

public unsafe class WondrousTails : Modules.Weekly<WondrousTailsData, WondrousTailsConfig> {
	public override ModuleName ModuleName => ModuleName.WondrousTails;
    
	public override bool HasClickableLink => Config.ClickableLink;
    
	public override PayloadId ClickableLinkPayloadId => Data.NewBookAvailable ? PayloadId.IdyllshireTeleport : PayloadId.OpenWondrousTailsBook;

	public WondrousTails() {
		Service.DutyState.DutyStarted += OnDutyStarted;
		Service.DutyState.DutyCompleted += OnDutyCompleted;
	}

	public override void Dispose() {
		Service.DutyState.DutyStarted -= OnDutyStarted;
		Service.DutyState.DutyCompleted -= OnDutyCompleted;
	}

	public override void Update() {
		Data.PlacedStickers = TryUpdateData(Data.PlacedStickers, PlayerState.Instance()->WeeklyBingoNumPlacedStickers);
		Data.SecondChance = TryUpdateData(Data.SecondChance, PlayerState.Instance()->WeeklyBingoNumSecondChancePoints);
		Data.Deadline = TryUpdateData(Data.Deadline, DateTimeOffset.FromUnixTimeSeconds(PlayerState.Instance()->GetWeeklyBingoExpireUnixTimestamp()).DateTime);
		Data.PlayerHasBook = TryUpdateData(Data.PlayerHasBook, PlayerState.Instance()->HasWeeklyBingoJournal);
		Data.NewBookAvailable = TryUpdateData(Data.NewBookAvailable, DateTime.UtcNow > Data.Deadline - TimeSpan.FromDays(7));
		Data.BookExpired = TryUpdateData(Data.BookExpired, PlayerState.Instance()->IsWeeklyBingoExpired());
        
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
					PrintMessage(Strings.ForgotBookWarning);
					UIModule.PlayChatSoundEffect(11);
				}
			}
		}

		base.Update();
	}

	private void OnDutyStarted(object? sender, ushort e) {
		if (!Config.ModuleEnabled) return;
		if (!Config.InstanceNotifications) return;
		if (Data is not { PlayerHasBook: true, BookExpired: false }) return;
		if (GetModuleStatus() == ModuleStatus.Complete) return;
		if (!PlayerState.Instance()->HasWeeklyBingoJournal) return;

		var taskState = GetStatusForTerritory(e);

		switch (taskState) {
			case PlayerState.WeeklyBingoTaskStatus.Claimed when Data is { PlacedStickers: > 0, SecondChance: > 0}:
				PrintMessage(Strings.RerollNotice);
				PrintMessage(string.Format(Strings.RerollsAvailable, Data.SecondChance), true);
				break;
            
			case PlayerState.WeeklyBingoTaskStatus.Claimable:
				PrintMessage(Strings.StampAlreadyAvailable, true);
				break;
            
			case PlayerState.WeeklyBingoTaskStatus.Open:
				PrintMessage(Strings.CompletionAvailable);
				break;
		}
	}
    
	private void OnDutyCompleted(object? sender, ushort e) {
		if (!Config.ModuleEnabled) return;
		if (!Config.InstanceNotifications) return;
		if (Data is not { PlayerHasBook: true, BookExpired: false }) return;
		if (GetModuleStatus() == ModuleStatus.Complete) return;
		if (!PlayerState.Instance()->HasWeeklyBingoJournal) return;

		var taskState = GetStatusForTerritory(e);

		switch (taskState)
		{
			case PlayerState.WeeklyBingoTaskStatus.Claimable:
			case PlayerState.WeeklyBingoTaskStatus.Open:
				PrintMessage(Strings.StampClaimable, true);
				break;
		}
	}

	protected override ModuleStatus GetModuleStatus() {
		if (Config.UnclaimedBookWarning && Data.NewBookAvailable) return ModuleStatus.Incomplete;

		return Data.PlacedStickers == 9 ? ModuleStatus.Complete : ModuleStatus.Incomplete;
	}
    
	protected override StatusMessage GetStatusMessage() => Data switch {
		{ PlayerHasBook: true, BookExpired: false } when Config.StickerAvailableNotice && AnyTaskAvailableForSticker() => 
			ConditionalStatusMessage.GetMessage(Config.ClickableLink, Strings.StickerAvailable, PayloadId.OpenWondrousTailsBook),

		{ SecondChance: > 7, PlacedStickers: >= 3 and <= 7, PlayerHasBook: true, BookExpired: false } when Config.ShuffleAvailableNotice => 
			ConditionalStatusMessage.GetMessage(Config.ClickableLink, Strings.ShuffleAvailable, PayloadId.OpenWondrousTailsBook),

		{ NewBookAvailable: true } when Config.UnclaimedBookWarning => 
			ConditionalStatusMessage.GetMessage(Config.ClickableLink, Strings.NewBookAvailable, PayloadId.IdyllshireTeleport),

		_ => ConditionalStatusMessage.GetMessage(Config.ClickableLink, string.Format(Strings.StickersRemaining, 9 - Data.PlacedStickers), PayloadId.OpenWondrousTailsBook),
	};

	private static PlayerState.WeeklyBingoTaskStatus? GetStatusForTerritory(uint territory) {
		foreach (var index in Enumerable.Range(0, 16)) {
			var dutyListForSlot = TaskLookup.GetInstanceListFromId(PlayerState.Instance()->WeeklyBingoOrderData[index]);

			if (dutyListForSlot.Contains(territory)) {
				return PlayerState.Instance()->GetWeeklyBingoTaskStatus(index);
			}
		}

		return null;
	}

	private bool AnyTaskAvailableForSticker() 
		=> Enumerable.Range(0, 16).Select(index => PlayerState.Instance()->GetWeeklyBingoTaskStatus(index)).Any(taskStatus => taskStatus == PlayerState.WeeklyBingoTaskStatus.Claimable);

	private void PrintMessage(string message, bool withPayload = false) {
		if (withPayload) {
			var conditionalMessage = ConditionalStatusMessage.GetMessage(Config.ClickableLink, message, PayloadId.OpenWondrousTailsBook);
			conditionalMessage.MessageChannel = GetChatChannel();
			conditionalMessage.SourceModule = ModuleName;
			conditionalMessage.PrintMessage();
		}
		else {
			var statusMessage = new StatusMessage {
				Message = message, 
				MessageChannel = GetChatChannel(),
				SourceModule = ModuleName,
			};
            
			statusMessage.PrintMessage();
		}
	}
}

internal static class TaskLookup {
	public static List<uint> GetInstanceListFromId(uint orderDataId) {
		var bingoOrderData = Service.DataManager.GetExcelSheet<WeeklyBingoOrderData>()!.GetRow(orderDataId);
		if (bingoOrderData is null) return [];
        
		switch (bingoOrderData.Type) {
			// Specific Duty
			case 0:
				return Service.DataManager.GetExcelSheet<ContentFinderCondition>()!
					.Where(c => c.Content == bingoOrderData.Data)
					.OrderBy(row => row.SortKey)
					.Select(c => c.TerritoryType.Row)
					.ToList();
            
			// Specific Level Dungeon
			case 1:
				return Service.DataManager.GetExcelSheet<ContentFinderCondition>()!
					.Where(m => m.ContentType.Row is 2)
					.Where(m => m.ClassJobLevelRequired == bingoOrderData.Data)
					.OrderBy(row => row.SortKey)
					.Select(m => m.TerritoryType.Row)
					.ToList();
            
			// Level Range Dungeon
			case 2:
				return Service.DataManager.GetExcelSheet<ContentFinderCondition>()!
					.Where(m => m.ContentType.Row is 2)
					.Where(m => m.ClassJobLevelRequired >= bingoOrderData.Data - (bingoOrderData.Data > 50 ? 9 : 49) && m.ClassJobLevelRequired <= bingoOrderData.Data - 1)
					.OrderBy(row => row.SortKey)
					.Select(m => m.TerritoryType.Row)
					.ToList();
            
			// Special categories
			case 3:
				return bingoOrderData.Unknown5 switch
				{
					// Treasure Map Instances are Not Supported
					1 => [],
                    
					// PvP Categories are Not Supported
					2 => [],
                    
					// Deep Dungeons
					3 => Service.DataManager.GetExcelSheet<ContentFinderCondition>()!
						.Where(m => m.ContentType.Row is 21)
						.OrderBy(row => row.SortKey)
						.Select(m => m.TerritoryType.Row)
						.ToList(),
                    
					_ => [],
				};
            
			// Multi-instance raids
			case 4:
				var raidIndex = (int)(bingoOrderData.Data - 11) * 2;
                
				return bingoOrderData.Data switch
				{
					// Binding Coil, Second Coil, Final Coil
					2 => [ 241, 242, 243, 244, 245 ],
					3 => [ 355, 356, 357, 358 ],
					4 => [ 193, 194, 195, 196 ],
                    
					// Gordias, Midas, The Creator
					5 => [ 442, 443, 444, 445 ],
					6 => [ 520, 521, 522, 523 ],
					7 => [ 580, 581, 582, 583 ],
                    
					// Deltascape, Sigmascape, Alphascape
					8 => [ 691, 692, 693, 694 ],
					9 => [ 748, 749, 750, 751 ],
					10 => [ 798, 799, 800, 801 ],

					// Eden's Gate: Resurrection or Descent
					11 => [ 849, 850 ],
					// Eden's Gate: Inundation or Sepulture
					12 => [ 851, 852 ],
					// Eden's Verse: Fulmination or Furor
					13 => [ 902, 903 ],
					// Eden's Verse: Iconoclasm or Refulgence
					14 => [ 904, 905 ],
					// Eden's Promise: Umbra or Litany
					15 => [ 942, 943 ],
					// Eden's Promise: Anamorphosis or Eternity
					16 => [ 944, 945 ],

					// Asphodelos: First or Second Circles
					17 => [ 1002, 1004 ],
					// Asphodelos: Third or Fourth Circles
					18 => [ 1006, 1008 ],
					// Abyssos: Fifth or Sixth Circles
					19 => [ 1081, 1083 ],
					// Abyssos: Seventh or Eight Circles
					20 => [ 1085, 1087 ],
					// Anabaseios: Ninth or Tenth Circles
					21 => [ 1147, 1149 ],
					// Anabaseios: Eleventh or Twelwth Circles
					22 => [ 1151, 1153 ],

					// Eden's Gate
					23 => [ 849, 850, 851, 852 ],
					// Eden's Verse
					24 => [ 902, 903, 904, 905 ],
					// Eden's Promise
					25 => [ 942, 943, 944, 945 ],

					// Alliance Raids (A Realm Reborn)
					26 => [ 174, 372, 151 ],
					// Alliance Raids (Heavensward)
					27 => [ 508, 556, 627 ],
					// Alliance Raids (Stormblood)
					28 => [ 734, 776, 826 ],
					// Alliance Raids (Shadowbringers)
					29 => [ 882, 917, 966 ],
					// Alliance Raids (Endwalker)
					30 => [ 1054, 1118, 1178 ],

					_ => [],
				};
		}
        
		Service.Log.Information($"[WondrousTails] Unrecognized ID: {orderDataId}");
		return [];
	}
}