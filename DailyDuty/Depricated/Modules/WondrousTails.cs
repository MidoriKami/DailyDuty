// using System;
// using System.Collections.Generic;
// using System.Drawing;
// using System.Globalization;
// using System.Linq;
// using System.Numerics;
// using DailyDuty.Classes;
// using DailyDuty.CustomNodes;
// using DailyDuty.Models;
// using DailyDuty.Modules.BaseModules;
// using Dalamud.Bindings.ImGui;
// using Dalamud.Game.Addon.Lifecycle;
// using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
// using Dalamud.Interface;
// using FFXIVClientStructs.FFXIV.Client.Game.UI;
// using FFXIVClientStructs.FFXIV.Client.UI;
// using FFXIVClientStructs.FFXIV.Client.UI.Agent;
// using FFXIVClientStructs.FFXIV.Component.GUI;
// using KamiLib.Classes;
// using KamiToolKit.Classes;
// using KamiToolKit.Classes.Controllers;
// using KamiToolKit.Extensions;
// using Lumina.Excel.Sheets;
//
// namespace DailyDuty.Modules;
//
// public class WondrousTailsData : ModuleData {
// 	public int PlacedStickers;
// 	public uint SecondChance;
// 	public bool NewBookAvailable;
// 	public bool PlayerHasBook;
// 	public DateTime Deadline;
// 	public TimeSpan TimeRemaining;
// 	public bool BookExpired;
// 	public bool CloseToKhloe;
// 	public float DistanceToKhloe;
// 	public bool CastingTeleport;
// 	
// 	protected override void DrawModuleData() {
// 		DrawDataTable(
// 			("Placed Stickers", PlacedStickers.ToString()), 
// 			("Second Chance Points", SecondChance.ToString()),
// 			("New Book Available", NewBookAvailable.ToString()), 
// 			("Player Has Book", PlayerHasBook.ToString()), 
// 			("Deadline", Deadline.ToLocalTime().ToString(CultureInfo.CurrentCulture)), 
// 			("Time Remaining", TimeRemaining.FormatTimespan()), 
// 			("Book Expired", BookExpired.ToString()), 
// 			("Near Khloe", CloseToKhloe.ToString()), 
// 			("Distance to Khloe", DistanceToKhloe.ToString(CultureInfo.CurrentCulture)), 
// 			("Casting Teleport", CastingTeleport.ToString())
// 		);
// 	}
// }
//
// public class WondrousTailsConfig : ModuleConfig {
// 	public bool InstanceNotifications = true;
// 	public bool StickerAvailableNotice = true;
// 	public bool UnclaimedBookWarning = true;
// 	public bool ShuffleAvailableNotice;
// 	public bool ClickableLink = true;
// 	public bool CloverIndicator = true;
// 	public bool ColorDutyFinderText;
// 	public Vector4 DutyFinderColor = KnownColor.Yellow.Vector();
// 	
// 	protected override void DrawModuleConfig() {
// 		ConfigChanged |= ImGui.Checkbox("Instance Notifications", ref InstanceNotifications);
// 		ConfigChanged |= ImGui.Checkbox("Sticker Available Warning", ref StickerAvailableNotice);
// 		ConfigChanged |= ImGui.Checkbox("Unclaimed Book Warning", ref UnclaimedBookWarning);
// 		ConfigChanged |= ImGui.Checkbox("Shuffle Available Warning", ref ShuffleAvailableNotice);
// 		ConfigChanged |= ImGui.Checkbox("Clickable Link", ref ClickableLink);
// 		ConfigChanged |= ImGui.Checkbox("Duty Finder Clover", ref CloverIndicator);
// 		ConfigChanged |= ImGui.Checkbox("Duty Finder Text Color", ref ColorDutyFinderText);
//
// 		ImGui.ColorEdit4("Duty Finder Color", ref DutyFinderColor, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.AlphaBar);
// 		ConfigChanged |= ImGui.IsItemDeactivatedAfterEdit();
// 	}
// }
//
// public unsafe class WondrousTails : BaseModules.Modules.Weekly<WondrousTailsData, WondrousTailsConfig> {
// 	public override ModuleName ModuleName => ModuleName.WondrousTails;
//     
// 	public override bool HasClickableLink => Config.ClickableLink;
//     
// 	public override PayloadId ClickableLinkPayloadId => Data.NewBookAvailable ? PayloadId.IdyllshireTeleport : PayloadId.OpenWondrousTailsBook;
//
// 	private readonly NativeListController dutyListController;
// 	
// 	private readonly Dictionary<uint, WondrousTailsNode> imageNodes = [];
// 	private readonly Dictionary<uint, List<uint>> wondrousTailsDuties = [];
//
// 	private bool isShowingTooltip;
// 	
// 	public WondrousTails() {
// 		dutyListController = new NativeListController("ContentsFinder") {
// 			GetPopulatorNode = GetPopulatorNode,
// 			ShouldModifyElement = ShouldModifyElement,
// 			UpdateElement = UpdateElement,
// 			ResetElement = ResetElement,
// 		};
// 		dutyListController.OnOpen += OnContentsFinderSetup;
// 		dutyListController.OnClose += ResetState;
// 		
// 		Services.AddonLifecycle.RegisterListener(AddonEvent.PostDraw, "ContentsFinder", OnContentsFinderDraw);
//         
// 		Services.DutyState.DutyStarted += OnDutyStarted;
// 		Services.DutyState.DutyCompleted += OnDutyCompleted;
// 	}
//
// 	public override void Dispose() {
// 		Services.AddonLifecycle.UnregisterListener(OnContentsFinderDraw);
//         
// 		dutyListController.Dispose();
// 		
// 		Services.DutyState.DutyStarted -= OnDutyStarted;
// 		Services.DutyState.DutyCompleted -= OnDutyCompleted;
// 	}
//
// 	private AtkComponentListItemRenderer* GetPopulatorNode(AtkUnitBase* addon) {
// 		var contentsFinder = (AddonContentsFinder*) addon;
// 		return contentsFinder->DutyList->GetItemRendererByNodeId(6);
// 	}
//
// 	private bool ShouldModifyElement(AtkUnitBase* unitBase, ListItemData listItemData, AtkResNode** nodeList) {
// 		var contentId = listItemData.ItemInfo->ListItem->UIntValues[1];
// 		var contentEntry = AgentContentsFinder.Instance()->ContentList[contentId - 1];
// 		var contentData = contentEntry.Value->Id;
//
// 		if (Config is { CloverIndicator: false, ColorDutyFinderText: false }) return false;
// 		if (contentData.ContentType is not ContentsId.ContentsType.Regular) return false;
// 		
// 		var cfc = Services.DataManager.GetExcelSheet<ContentFinderCondition>().GetRow(contentData.Id);
// 		return IsTailsTask(cfc);
// 	}
//
// 	private void UpdateElement(AtkUnitBase* unitBase, ListItemData listItemData, AtkResNode** nodeList) {
// 		var dutyNameTextNode = (AtkTextNode*) nodeList[3];
// 		var levelTextNode = (AtkTextNode*) nodeList[4];
//
// 		var index = listItemData.ItemInfo->ListItem->Renderer->OwnerNode->NodeId;
// 		
// 		var contentId = listItemData.ItemInfo->ListItem->UIntValues[1];
// 		var contentEntry = AgentContentsFinder.Instance()->ContentList[contentId - 1];
// 		var contentData = contentEntry.Value->Id;
// 		var cfc = Services.DataManager.GetExcelSheet<ContentFinderCondition>().GetRow(contentData.Id);
//
// 		// If clover is enabled
// 		if (Config.CloverIndicator) {
// 			
// 			// And it is attached already, update it
// 			if (imageNodes.TryGetValue(index, out var node)) {
// 				node.IsVisible = true;
// 				node.IsTaskAvailable = IsTaskAvailable(cfc);
// 			}
// 			
// 			// else make it and attach it
// 			else {
// 				dutyNameTextNode->Width = (ushort) (dutyNameTextNode->Width - 24.0f);
//
// 				var newNode = new WondrousTailsNode {
// 					Size = new Vector2(24.0f, 24.0f),
// 					Position = new Vector2(dutyNameTextNode->X + dutyNameTextNode->Width, 0.0f),
// 					IsVisible = true,
// 					IsTaskAvailable = IsTaskAvailable(cfc),
// 				};
// 				newNode.AttachNode((AtkResNode*)dutyNameTextNode, NodePosition.AfterTarget);
//
// 				imageNodes.Add(index, newNode);
// 			}
// 		}
//
// 		if (Config.ColorDutyFinderText) {
// 			if (IsTaskAvailable(cfc)) {
// 				dutyNameTextNode->TextColor = Config.DutyFinderColor.ToByteColor();
// 			}
// 			else {
// 				dutyNameTextNode->TextColor = levelTextNode->TextColor;
// 			}
// 		}
// 		else {
// 			dutyNameTextNode->TextColor = levelTextNode->TextColor;
// 		}
// 	}
//
// 	private void ResetElement(AtkUnitBase* unitBase, ListItemData listItemData, AtkResNode** nodeList) {
// 		var dutyNameTextNode = (AtkTextNode*) nodeList[3];
// 		var levelTextNode = (AtkTextNode*) nodeList[4];
// 		var index = listItemData.ItemInfo->ListItem->Renderer->OwnerNode->NodeId;
//
// 		// Remove node
// 		if (imageNodes.TryGetValue(index, out var node)) {
// 			dutyNameTextNode->Width = (ushort) (dutyNameTextNode->Width + 24.0f);
//
// 			imageNodes.Remove(index);
// 			node.Dispose();
// 		}
// 		
// 		// Reset Color
// 		dutyNameTextNode->TextColor = levelTextNode->TextColor;
// 	}
//
// 	private void ResetState() {
// 		foreach (var node in imageNodes.Values) {
// 			node.Dispose();
// 		}
// 		imageNodes.Clear();
// 	}
//
// 	private void OnContentsFinderSetup() {
// 		wondrousTailsDuties.Clear();
// 		
// 		foreach (var index in Enumerable.Range(0, 16)) {
// 			var tailsTaskId = PlayerState.Instance()->WeeklyBingoOrderData[index];
// 			var tailsDuties = Services.DataManager.GetDutiesForOrderData(tailsTaskId).Select(cfc => cfc.RowId).ToList();
// 			wondrousTailsDuties.Add((uint) index, tailsDuties);
// 		}
// 	}
// 	
// 	private void OnContentsFinderDraw(AddonEvent type, AddonArgs args) {
// 		ref var cursor = ref UIInputData.Instance()->CursorInputs;
// 		
// 		foreach (var node in imageNodes.Values) {
// 			if (node.CheckCollision((short) cursor.PositionX, (short) cursor.PositionY)) {
// 				AtkStage.Instance()->TooltipManager.ShowTooltip(args.Addon.Id, (AtkResNode*)node, 
// 					"[DailyDuty] This duty has an associated task in the Wondrous Tails Book.");
// 				isShowingTooltip = true;
// 				return;
// 			}
// 		}
//
// 		if (isShowingTooltip) {
// 			AtkStage.Instance()->TooltipManager.HideTooltip(args.Addon.Id);
// 			isShowingTooltip = false;
// 		}
// 	}
//
// 	private bool IsTaskAvailable(ContentFinderCondition cfc) {
// 		var taskState = GetStatusForDuty(cfc.RowId);
// 		return taskState is PlayerState.WeeklyBingoTaskStatus.Claimable or PlayerState.WeeklyBingoTaskStatus.Open;
// 	}
//
// 	private bool IsTailsTask(ContentFinderCondition cfc)
// 		=> wondrousTailsDuties.Values.Any(dutyList => dutyList.Contains(cfc.RowId));
//
// 	public override void Update() {
// 		Data.PlacedStickers = TryUpdateData(Data.PlacedStickers, PlayerState.Instance()->WeeklyBingoNumPlacedStickers);
// 		Data.SecondChance = TryUpdateData(Data.SecondChance, PlayerState.Instance()->WeeklyBingoNumSecondChancePoints);
// 		Data.Deadline = TryUpdateData(Data.Deadline, DateTimeOffset.FromUnixTimeSeconds(PlayerState.Instance()->GetWeeklyBingoExpireUnixTimestamp()).DateTime);
// 		Data.PlayerHasBook = TryUpdateData(Data.PlayerHasBook, PlayerState.Instance()->HasWeeklyBingoJournal);
// 		Data.NewBookAvailable = TryUpdateData(Data.NewBookAvailable, DateTime.UtcNow > Data.Deadline - TimeSpan.FromDays(7));
// 		Data.BookExpired = TryUpdateData(Data.BookExpired, PlayerState.Instance()->IsWeeklyBingoExpired());
//         
// 		CheckKhloeDistance();
//
// 		base.Update();
// 	}
//
// 	private void CheckKhloeDistance() {
// 		var timeRemaining = Data.Deadline - DateTime.UtcNow;
// 		Data.TimeRemaining = timeRemaining > TimeSpan.Zero ? timeRemaining : TimeSpan.Zero;
// 		Data.DistanceToKhloe = 0.0f;
// 		var lastNearKhloe = Data.CloseToKhloe;
// 		var lastCastingTeleport = Data.CastingTeleport;
// 		Data.CloseToKhloe = false;
// 		Data.CastingTeleport = false;
//         
// 		const int idyllshireTerritoryType = 478;
// 		const uint khloeAliapohDataId = 1017653;
// 		if (Services.ClientState.TerritoryType is idyllshireTerritoryType && Config.ModuleEnabled) {
// 			var khloe = Services.ObjectTable.FirstOrDefault(obj => obj.BaseId is khloeAliapohDataId);
//
// 			if (khloe is not null && Services.ObjectTable.LocalPlayer is { Position: var playerPosition }) {
// 				Data.DistanceToKhloe = Vector3.Distance(playerPosition, khloe.Position);
// 				Data.CloseToKhloe = Data.DistanceToKhloe < 10.0f;
// 				Data.CastingTeleport = Services.ObjectTable.LocalPlayer is { IsCasting: true, CastActionId: 5 or 6 };
//
// 				var noLongerNearKhloe = lastNearKhloe && !Data.CloseToKhloe;
// 				var startedTeleportingAway = lastNearKhloe && !lastCastingTeleport && Data.CastingTeleport;
//                 
// 				if ((noLongerNearKhloe || startedTeleportingAway) && Data is { PlayerHasBook: false, NewBookAvailable: true }) {
// 					new StatusMessage {
// 						Message = "Wait! You forgot you book!", MessageChannel = GetChatChannel(), SourceModule = ModuleName,
// 					}.PrintMessage();
// 					UIGlobals.PlayChatSoundEffect(11);
// 				}
// 			}
// 		}
// 	}
// 	
// 	private void OnDutyStarted(object? sender, ushort e) {
// 		if (!Config.ModuleEnabled) return;
// 		if (!Config.InstanceNotifications) return;
// 		if (Data is not { PlayerHasBook: true, BookExpired: false }) return;
// 		if (GetModuleStatus() == ModuleStatus.Complete) return;
// 		if (!PlayerState.Instance()->HasWeeklyBingoJournal) return;
//
// 		var taskState = GetStatusForTerritory(e);
//
// 		switch (taskState) {
// 			case PlayerState.WeeklyBingoTaskStatus.Claimed when Data is { PlacedStickers: > 0, SecondChance: > 0}:
// 				PrintMessage($"{Data.SecondChance} Rerolls Available");
// 				break;
//             
// 			case PlayerState.WeeklyBingoTaskStatus.Claimable:
// 				PrintMessage("Sticker is Already Available", true);
// 				break;
//             
// 			case PlayerState.WeeklyBingoTaskStatus.Open:
// 				PrintMessage("Stickers Claimable");
// 				break;
// 		}
// 	}
//
// 	private void OnDutyCompleted(object? sender, ushort e) {
// 		if (!Config.ModuleEnabled) return;
// 		if (!Config.InstanceNotifications) return;
// 		if (Data is not { PlayerHasBook: true, BookExpired: false }) return;
// 		if (GetModuleStatus() == ModuleStatus.Complete) return;
// 		if (!PlayerState.Instance()->HasWeeklyBingoJournal) return;
//
// 		var taskState = GetStatusForTerritory(e);
//
// 		switch (taskState)
// 		{
// 			case PlayerState.WeeklyBingoTaskStatus.Claimable:
// 			case PlayerState.WeeklyBingoTaskStatus.Open:
// 				PrintMessage("Stamps Claimable", true);
// 				break;
// 		}
// 	}
// 	
// 	private void PrintMessage(string message, bool withPayload = false) {
// 		var statusMessage = new LinkedStatusMessage {
// 			LinkEnabled = withPayload,
// 			Message = message,
// 			Payload = PayloadId.OpenWondrousTailsBook,
// 			SourceModule = ModuleName,
// 			MessageChannel = GetChatChannel(),
// 		};
// 		
// 		statusMessage.PrintMessage();
// 	}
// 	
// 	private static PlayerState.WeeklyBingoTaskStatus? GetStatusForTerritory(uint territory) {
// 		foreach (var index in Enumerable.Range(0, 16)) {
// 			var territoriesForSlot = Services.DataManager.GetTerritoriesForOrderData(PlayerState.Instance()->WeeklyBingoOrderData[index]);
//
// 			if (territoriesForSlot.Any(terr => terr.RowId == territory)) {
// 				return PlayerState.Instance()->GetWeeklyBingoTaskStatus(index);
// 			}
// 		}
//
// 		return null;
// 	}
//
// 	private PlayerState.WeeklyBingoTaskStatus? GetStatusForDuty(uint cfc) { 
// 		foreach(var (taskId, dutyList) in wondrousTailsDuties) {
// 			if (dutyList.Contains(cfc)) {
// 				return PlayerState.Instance()->GetWeeklyBingoTaskStatus((int)taskId);
// 			}
// 		}
//
// 		return null;
// 	}
// 	
// 	protected override ModuleStatus GetModuleStatus() {
// 		if (Config.UnclaimedBookWarning && Data.NewBookAvailable) return ModuleStatus.Incomplete;
//
// 		return Data.PlacedStickers == 9 ? ModuleStatus.Complete : ModuleStatus.Incomplete;
// 	}
//     
// 	protected override StatusMessage GetStatusMessage() => Data switch {
// 		{ PlayerHasBook: true, BookExpired: false } when Config.StickerAvailableNotice && AnyTaskAvailableForSticker() => new LinkedStatusMessage {
// 			LinkEnabled = Config.ClickableLink,
// 			Message = "Sticker Available",
// 			Payload = PayloadId.OpenWondrousTailsBook,
// 		},
//
// 		{ SecondChance: > 7, PlacedStickers: >= 3 and <= 7, PlayerHasBook: true, BookExpired: false } when Config.ShuffleAvailableNotice  => new LinkedStatusMessage {
// 			LinkEnabled = Config.ClickableLink,
// 			Message = "Shuffle Available",
// 			Payload = PayloadId.OpenWondrousTailsBook,
// 		},
//
// 		{ NewBookAvailable: true } when Config.UnclaimedBookWarning  => new LinkedStatusMessage {
// 			LinkEnabled = Config.ClickableLink,
// 			Message = "New Book Available",
// 			Payload = PayloadId.IdyllshireTeleport,
// 		},
//
// 		_  => new LinkedStatusMessage {
// 			LinkEnabled = Config.ClickableLink,
// 			Message = $"{9 - Data.PlacedStickers} Stickers Remaining",
// 			Payload = PayloadId.OpenWondrousTailsBook,
// 		},
// 	};
//
// 	private static bool AnyTaskAvailableForSticker() 
// 		=> Enumerable.Range(0, 16).Select(index => PlayerState.Instance()->GetWeeklyBingoTaskStatus(index)).Any(taskStatus => taskStatus == PlayerState.WeeklyBingoTaskStatus.Claimable);
// }
