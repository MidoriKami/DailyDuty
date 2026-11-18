using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using DailyDuty.Classes;
using DailyDuty.Enums;
using DailyDuty.Interfaces;
using Dalamud.Game.DutyState;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI;
using KamiToolKit.Components.Builders;
using KamiToolKit.Nodes;
using Lumina.Text.ReadOnly;

namespace DailyDuty.Modules;

public class WondrousTails : Module<WondrousTailsData, WondrousTailsConfig>, IWeeklyResettable, IConfigurable, IFrameworkUpdatable, IClickableTooltip {

    public override string DisplayName => Strings.WondrousTails_DisplayName;

    public override WondrousTailsData Data { get; protected set; } = null!;
    public override WondrousTailsConfig Config { get; protected set; } = null!;

    private ChatLinkPayload? openWondrousTailsPayload;
    private ChatLinkPayload? teleportToIdyllshirePayload;

    private bool closeToKhloe;
    private bool castingTeleport;

    private TextNode? hasBook;
    private TextNode? deadline;
    private TextNode? secondChance;
    private TextNode? placedStickers;
    private TextNode? newBookAvailable;
    private TextNode? isExpired;

    public override async Task EnableAsync() {
        Data = await WondrousTailsData.Load();
        Config = await WondrousTailsConfig.Load();

        openWondrousTailsPayload = ChatLinkPayload.OpenWondrousTailsBook();
        teleportToIdyllshirePayload = ChatLinkPayload.TeleportToIdyllshire();

        IDutyState.Get().DutyStarted += OnDutyStarted;
        IDutyState.Get().DutyCompleted += OnDutyCompleted;
    }

    public override Task DisableAsync() {
        IDutyState.Get().DutyStarted -= OnDutyStarted;
        IDutyState.Get().DutyCompleted -= OnDutyCompleted;

        openWondrousTailsPayload?.Dispose();
        openWondrousTailsPayload = null;

        teleportToIdyllshirePayload?.Dispose();
        teleportToIdyllshirePayload = null;

        hasBook = null;
        deadline = null;
        secondChance = null;
        placedStickers = null;
        newBookAvailable = null;
        isExpired = null;

        return Task.CompletedTask;
    }

    public override void BuildDataUi(NodeBuilder builder) => builder
        .DataRow(Strings.WondrousTails_BookObtained, "HAS_BOOK", out hasBook)
        .DataRow(Strings.WondrousTails_Deadline, "DEADLINE", out deadline)
        .DataRow(Strings.WondrousTails_SecondChancePoints, "SECOND_CHANCE", out secondChance)
        .DataRow(Strings.WondrousTails_PlacedStickers, "PLACED_STICKERS", out placedStickers)
        .DataRow(Strings.WondrousTails_NewBookAvailable, "NEW_BOOK_AVAILABLE", out newBookAvailable)
        .DataRow(Strings.WondrousTails_BookExpired, "IS_EXPIRED", out isExpired);

    public override void UpdateDataUi() {
        var bookDeadline = Deadline.ToLocalTime();

        hasBook?.String = PlayerHasBook.ToString();
        deadline?.String = $"{bookDeadline.ToShortDateString()} {bookDeadline.ToShortTimeString()}";
        secondChance?.String = SecondChancePoints.ToString();
        placedStickers?.String = PlacedStickers.ToString();
        newBookAvailable?.String = IsNewBookAvailable.ToString();
        isExpired?.String = IsBookExpired.ToString();
    }

    public void BuildConfigUi(NodeBuilder builder) => builder
        .Checkbox(Strings.WondrousTails_InstanceNotifications, Config.InstanceNotifications, newValue => {
            Config.InstanceNotifications = newValue;
            Config.Save();
        })
        .Checkbox(Strings.WondrousTails_StickerNotification, Config.StickerAvailableNotice, newValue => {
            Config.StickerAvailableNotice = newValue;
            Config.Save();
        })
        .Checkbox(Strings.WondrousTails_UnclaimedNotification, Config.UnclaimedBookWarning, newValue => {
            Config.UnclaimedBookWarning = newValue;
            Config.Save();
        })
        .Checkbox(Strings.WondrousTails_ShuffleNotification, Config.ShuffleAvailableNotice, newValue => {
            Config.ShuffleAvailableNotice = newValue;
            Config.Save();
        });

    public void Update() {
        var lastNearKhloe = closeToKhloe;
        var lastCastingTeleport = castingTeleport;

        const uint idyllshireTerritoryType = 478;
        const uint khloeAliapohDataId = 1017653;

        if (IClientState.Get().TerritoryType.RowId is not idyllshireTerritoryType) return;

        var khloe = IObjectTable.Get().FirstOrDefault(obj => obj.BaseId is khloeAliapohDataId);

        if (khloe is not null && IObjectTable.Get().LocalPlayer is { Position: var playerPosition }) {
            var distanceToKhloe = Vector3.Distance(playerPosition, khloe.Position);

            closeToKhloe = distanceToKhloe < 10.0f;
            var noLongerNearKhloe = lastNearKhloe && !closeToKhloe;

            castingTeleport = IObjectTable.Get().LocalPlayer is { IsCasting: true, CastActionId: 5 or 6 };
            var startedTeleportingAway = lastNearKhloe && !lastCastingTeleport && castingTeleport;

            if ((noLongerNearKhloe || startedTeleportingAway) && IsNewBookAvailable && (!PlayerHasBook || IsBookExpired)) {
                UIGlobals.PlayChatSoundEffect(11);
                IChatGui.Get().PrintMessage(Config.MessageChatChannel, teleportToIdyllshirePayload, DisplayName, Strings.WondrousTails_ForgotBook);
            }
        }
    }

    public ReadOnlySeString GetTooltip() {
        if (IsNewBookAvailable) return Strings.WondrousTails_NewBookAvailable;

        return $"{9 - PlacedStickers} {Strings.WondrousTails_PlacedStickers}";
    }

    public void OnTooltipClicked() {
        if (IsNewBookAvailable) {
            teleportToIdyllshirePayload?.ClickAction();
        }
        else {
            openWondrousTailsPayload?.ClickAction();
        }
    }

    protected override Task ResetAsync()
        => Task.CompletedTask;

    protected override CompletionStatus GetCompletionStatus() {
        if (Config.UnclaimedBookWarning && IsNewBookAvailable) return CompletionStatus.Incomplete;

        return PlacedStickers is 9 ? CompletionStatus.Complete : CompletionStatus.Incomplete;
    }

    protected override void SendStatusMessage(ReadOnlySeString? message) => IChatGui.Get().PrintMessage(
        Config.MessageChatChannel,
        IsNewBookAvailable ? teleportToIdyllshirePayload : openWondrousTailsPayload,
        DisplayName,
        message ?? GetStatusMessage()
    );

    protected override void SendResetMessage(ReadOnlySeString? message) => IChatGui.Get().PrintMessage(
        Config.MessageChatChannel,
        openWondrousTailsPayload,
        DisplayName,
        message ?? "Module has been reset."
    );

    private ReadOnlySeString GetStatusMessage() => this switch {
        { IsNewBookAvailable: true } when Config.UnclaimedBookWarning => Strings.WondrousTails_NewBookAvailable,
        { PlayerHasBook: true, IsBookExpired: false } when Config.StickerAvailableNotice && IsStickerAvailable => Strings.WondrousTails_StickerAvailable,
        { SecondChancePoints: > 7, PlacedStickers: >= 3 and <= 7, PlayerHasBook: true, IsBookExpired: false } when Config.ShuffleAvailableNotice => Strings.WondrousTails_ShuffleAvailable,
        _ => $"{9 - PlacedStickers} {Strings.WondrousTails_PlacedStickers}",
    };

    public unsafe bool PlayerHasBook
        => PlayerState.Instance()->HasWeeklyBingoJournal;

    public unsafe DateTime Deadline
        => DateTimeOffset.FromUnixTimeSeconds(PlayerState.Instance()->GetWeeklyBingoExpireUnixTimestamp()).DateTime;

    public unsafe uint SecondChancePoints
        => PlayerState.Instance()->WeeklyBingoNumSecondChancePoints;

    public unsafe int PlacedStickers
        => PlayerState.Instance()->WeeklyBingoNumPlacedStickers;

    public bool IsNewBookAvailable
        => DateTime.UtcNow > Deadline - TimeSpan.FromDays(7);

    public unsafe bool IsBookExpired
        => PlayerState.Instance()->IsWeeklyBingoExpired();

    private static unsafe bool IsStickerAvailable
        => Enumerable.Range(0, 16).Select(index => PlayerState.Instance()->GetWeeklyBingoTaskStatus(index)).Any(taskStatus => taskStatus is PlayerState.WeeklyBingoTaskStatus.Claimable);

    private void OnDutyStarted(uint territoryType) {
        if (!Config.InstanceNotifications) return;
        if (!PlayerHasBook || IsBookExpired) return;
        if (GetCompletionStatus() is CompletionStatus.Complete) return;

        var taskState = GetStatusForTerritory(territoryType);

        switch (taskState) {
            case PlayerState.WeeklyBingoTaskStatus.Claimed when this is { PlacedStickers: > 0, SecondChancePoints: > 0 }:
                IChatGui.Get().PrintMessage(Config.MessageChatChannel, openWondrousTailsPayload, DisplayName, $"{SecondChancePoints} {Strings.WondrousTails_RerollsAvailable}");
                break;

            case PlayerState.WeeklyBingoTaskStatus.Claimable:
                IChatGui.Get().PrintMessage(Config.MessageChatChannel, openWondrousTailsPayload, DisplayName, Strings.WondrousTails_StickerAlreadyAvailable);
                break;

            case PlayerState.WeeklyBingoTaskStatus.Open:
                IChatGui.Get().PrintMessage(Config.MessageChatChannel, openWondrousTailsPayload, DisplayName, Strings.WondrousTails_CompletingDutyReward);
                break;
        }
    }

    private void OnDutyStarted(IDutyStateEventArgs args)
        => OnDutyStarted(args.TerritoryType.RowId);

    private void OnDutyCompleted(IDutyStateEventArgs args)
        => OnDutyCompleted(args.TerritoryType.RowId);

    private void OnDutyCompleted(uint territoryType) {
        if (!Config.InstanceNotifications) return;
        if (!PlayerHasBook || IsBookExpired) return;
        if (GetCompletionStatus() is CompletionStatus.Complete) return;

        Task.Run(async () => {

            // This is slow, we should do this in a task.
            // The user can wait up to a few frames for a result.
            var taskState = GetStatusForTerritory(territoryType);

            switch (taskState) {
                case PlayerState.WeeklyBingoTaskStatus.Claimable:
                case PlayerState.WeeklyBingoTaskStatus.Open:
                    await IFramework.Get().Run(() => {
                        IChatGui.Get().PrintMessage(Config.MessageChatChannel, openWondrousTailsPayload, DisplayName, Strings.WondrousTails_StickersClaimable);
                    });
                    break;
            }
        });
    }

    private static unsafe PlayerState.WeeklyBingoTaskStatus? GetStatusForTerritory(uint territory) {
        foreach (var index in Enumerable.Range(0, 16)) {
            var orderDataId = PlayerState.Instance()->WeeklyBingoOrderData[index];

            var territoriesForSlot = IDataManager.Get().GetTerritoriesForOrderData(orderDataId);
            if (territoriesForSlot.Any(t => t.RowId == territory)) {
                return PlayerState.Instance()->GetWeeklyBingoTaskStatus(index);
            }
        }

        return null;
    }
}

public class WondrousTailsData : ModuleDataFile<WondrousTailsData> {
    protected override string FileName => "WondrousTails.data.json";
}

public class WondrousTailsConfig : ModuleConfigFile<WondrousTailsConfig> {
    protected override string FileName => "WondrousTails.config.json";

    public bool InstanceNotifications { get; set; } = true;
    public bool StickerAvailableNotice { get; set; } = true;
    public bool UnclaimedBookWarning { get; set; } = true;
    public bool ShuffleAvailableNotice { get; set; }
}
