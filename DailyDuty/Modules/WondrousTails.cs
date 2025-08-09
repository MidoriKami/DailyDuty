using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Numerics;
using DailyDuty.Classes;
using DailyDuty.CustomNodes;
using DailyDuty.Localization;
using DailyDuty.Models;
using DailyDuty.Modules.BaseModules;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.Classes;
using KamiToolKit.Classes;
using KamiToolKit.Extensions;
using Lumina.Excel.Sheets;

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
	public bool InstanceNotifications = true;
	public bool StickerAvailableNotice = true;
	public bool UnclaimedBookWarning = true;
	public bool ShuffleAvailableNotice;
	public bool ClickableLink = true;
	public bool CloverIndicator = true;
	public bool ColorDutyFinderText;
	public Vector4 DutyFinderColor = KnownColor.Yellow.Vector();
	
	protected override void DrawModuleConfig() {
		ConfigChanged |= ImGui.Checkbox(Strings.InstanceNotifications, ref InstanceNotifications);
		ConfigChanged |= ImGui.Checkbox(Strings.StickerAvailableNotice, ref StickerAvailableNotice);
		ConfigChanged |= ImGui.Checkbox(Strings.UnclaimedBookWarning, ref UnclaimedBookWarning);
		ConfigChanged |= ImGui.Checkbox(Strings.ShuffleAvailableNotice, ref ShuffleAvailableNotice);
		ConfigChanged |= ImGui.Checkbox(Strings.ClickableLink, ref ClickableLink);
		ConfigChanged |= ImGui.Checkbox("Duty Finder Clover", ref CloverIndicator);
		ConfigChanged |= ImGui.Checkbox("Duty Finder Text Color", ref ColorDutyFinderText);

		ImGui.ColorEdit4("Duty Finder Color", ref DutyFinderColor, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.AlphaBar);
		ConfigChanged |= ImGui.IsItemDeactivatedAfterEdit();
	}
}

public unsafe class WondrousTails : BaseModules.Modules.Weekly<WondrousTailsData, WondrousTailsConfig> {
	public override ModuleName ModuleName => ModuleName.WondrousTails;
    
	public override bool HasClickableLink => Config.ClickableLink;
    
	public override PayloadId ClickableLinkPayloadId => Data.NewBookAvailable ? PayloadId.IdyllshireTeleport : PayloadId.OpenWondrousTailsBook;

	private readonly ContentFinderDutyListController dutyListController;
	
	private readonly Dictionary<uint, WondrousTailsNode> imageNodes = [];
	private readonly Dictionary<uint, List<uint>> wondrousTailsDuties = [];

	private bool isShowingTooltip;
	
	public WondrousTails() {
		dutyListController = new ContentFinderDutyListController();
		dutyListController.Apply += ApplyListModification;
		dutyListController.Update += UpdateListModification;
		dutyListController.Reset += ResetListModification;
		dutyListController.OnOpen += OnContentsFinderSetup;
		dutyListController.OnClose += ResetState;
		
		Service.AddonLifecycle.RegisterListener(AddonEvent.PostDraw, "ContentsFinder", OnContentsFinderDraw);
        
		Service.DutyState.DutyStarted += OnDutyStarted;
		Service.DutyState.DutyCompleted += OnDutyCompleted;
	}

	private void ResetState() {
		foreach (var node in imageNodes.Values) {
			node.Dispose();
		}
		imageNodes.Clear();
	}

	public override void Dispose() {
		Service.AddonLifecycle.UnregisterListener(OnContentsFinderDraw);
        
		dutyListController.Dispose();
		
		Service.DutyState.DutyStarted -= OnDutyStarted;
		Service.DutyState.DutyCompleted -= OnDutyCompleted;
	}
	
	private void OnContentsFinderSetup() {
		wondrousTailsDuties.Clear();
		
		foreach (var index in Enumerable.Range(0, 16)) {
			var tailsTaskId = PlayerState.Instance()->WeeklyBingoOrderData[index];
			var tailsDuties = Service.DataManager.GetDutiesForOrderData(tailsTaskId).Select(cfc => cfc.RowId).ToList();
			wondrousTailsDuties.Add((uint) index, tailsDuties);
		}
	}
	
	private void OnContentsFinderDraw(AddonEvent type, AddonArgs args) {
		ref var cursor = ref UIInputData.Instance()->CursorInputs;
		
		foreach (var node in imageNodes.Values) {
			if (node.CheckCollision((short) cursor.PositionX, (short) cursor.PositionY)) {
				AtkStage.Instance()->TooltipManager.ShowTooltip(args.Addon.Id, (AtkResNode*)node, 
					"[DailyDuty] This duty has an associated task in the Wondrous Tails Book.");
				isShowingTooltip = true;
				return;
			}
		}

		if (isShowingTooltip) {
			AtkStage.Instance()->TooltipManager.HideTooltip(args.Addon.Id);
			isShowingTooltip = false;
		}
	}

	private void ApplyListModification(ListPopulatorData<AddonContentsFinder> obj) {
		var dutyNameTextNode = (AtkTextNode*) obj.NodeList[3];
		
		if (Config.CloverIndicator && IsTailsTask(obj)) {
			AttachCloverNode(obj, dutyNameTextNode);
		}

		if (Config.ColorDutyFinderText && IsTailsTask(obj) && IsTaskAvailable(obj)) {
			dutyNameTextNode->TextColor = Config.DutyFinderColor.ToByteColor();
		}
	}

	private void UpdateListModification(ListPopulatorData<AddonContentsFinder> obj) {
		var dutyNameTextNode = (AtkTextNode*) obj.NodeList[3];
		var levelTextNode = (AtkTextNode*) obj.NodeList[4];
		
		if (IsTailsTask(obj)) {
			if (IsTaskAvailable(obj)) {
				if (imageNodes.TryGetValue(obj.Index, out var node)) {
					node.IsVisible = true;
					node.IsTaskAvailable = true;
				}
				else {
					AttachCloverNode(obj, dutyNameTextNode);
				}
				
				if (Config.ColorDutyFinderText) {
					dutyNameTextNode->TextColor = Config.DutyFinderColor.ToByteColor();
				}
			}
			else {
				if (imageNodes.TryGetValue(obj.Index, out var node)) {
					node.IsVisible = true;
					node.IsTaskAvailable = false;
				}
				else {
					AttachCloverNode(obj, dutyNameTextNode);
				}

				dutyNameTextNode->TextColor = levelTextNode->TextColor;
			}
		}
		else {
			if (imageNodes.TryGetValue(obj.Index, out var node)) {
				node.IsVisible = false;
			}
			
			dutyNameTextNode->TextColor = levelTextNode->TextColor;
		}
	}

	private void AttachCloverNode(ListPopulatorData<AddonContentsFinder> obj, AtkTextNode* dutyNameTextNode) {
		if (!Config.CloverIndicator) return;

		dutyNameTextNode->Width = (ushort) (dutyNameTextNode->Width - 24.0f);

		var newNode = new WondrousTailsNode {
			Size = new Vector2(24.0f, 24.0f),
			Position = new Vector2(dutyNameTextNode->X + dutyNameTextNode->Width, 0.0f),
			IsVisible = true,
			IsTaskAvailable = IsTaskAvailable(obj),
		};
		System.NativeController.AttachNode(newNode, (AtkResNode*) dutyNameTextNode, NodePosition.AfterTarget);

		imageNodes.Add(obj.Index, newNode);
	}

	private void ResetListModification(ListPopulatorData<AddonContentsFinder> obj) {
		var dutyNameTextNode = (AtkTextNode*) obj.NodeList[3];
		var levelTextNode = (AtkTextNode*) obj.NodeList[4];

		// Remove node
		if (imageNodes.TryGetValue(obj.Index, out var node)) {
			dutyNameTextNode->Width = (ushort) (dutyNameTextNode->Width + 24.0f);

			System.NativeController.DetachNode(node);
			imageNodes.Remove(obj.Index);
			node.Dispose();
		}
		
		// Reset Color
		dutyNameTextNode->TextColor = levelTextNode->TextColor;
	}

	private static ContentFinderCondition GetContentFinderCondition(ListPopulatorData<AddonContentsFinder> obj) {
		var contentId = obj.ItemInfo->ListItem->UIntValues[1];
		var contentEntry = AgentContentsFinder.Instance()->ContentList[contentId - 1];
		var contentData = contentEntry.Value->Id;
		return Service.DataManager.GetExcelSheet<ContentFinderCondition>().GetRow(contentData.Id);
	}

	private bool IsTailsTask(ListPopulatorData<AddonContentsFinder> obj)
		=> IsTailsTask(GetContentFinderCondition(obj));

	private bool IsTaskAvailable(ListPopulatorData<AddonContentsFinder> obj) {
		var contentFinderEntry = GetContentFinderCondition(obj);
		var taskState = GetStatusForDuty(contentFinderEntry.RowId);
		return taskState is PlayerState.WeeklyBingoTaskStatus.Claimable or PlayerState.WeeklyBingoTaskStatus.Open;
	}

	private bool IsTailsTask(ContentFinderCondition cfc)
		=> wondrousTailsDuties.Values.Any(dutyList => dutyList.Contains(cfc.RowId));

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
	
	private void PrintMessage(string message, bool withPayload = false) {
		var statusMessage = new LinkedStatusMessage {
			LinkEnabled = withPayload,
			Message = message,
			Payload = PayloadId.OpenWondrousTailsBook,
			SourceModule = ModuleName,
			MessageChannel = GetChatChannel(),
		};
		
		statusMessage.PrintMessage();
	}
	
	private static PlayerState.WeeklyBingoTaskStatus? GetStatusForTerritory(uint territory) {
		foreach (var index in Enumerable.Range(0, 16)) {
			var territoriesForSlot = Service.DataManager.GetTerritoriesForOrderData(PlayerState.Instance()->WeeklyBingoOrderData[index]);

			if (territoriesForSlot.Any(terr => terr.RowId == territory)) {
				return PlayerState.Instance()->GetWeeklyBingoTaskStatus(index);
			}
		}

		return null;
	}

	private PlayerState.WeeklyBingoTaskStatus? GetStatusForDuty(uint cfc) { 
		foreach(var (taskId, dutyList) in wondrousTailsDuties) {
			if (dutyList.Contains(cfc)) {
				return PlayerState.Instance()->GetWeeklyBingoTaskStatus((int)taskId);
			}
		}

		return null;
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