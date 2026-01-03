using System;
using System.Linq;
using System.Numerics;
using DailyDuty.Classes;
using DailyDuty.CustomNodes;
using DailyDuty.Enums;
using DailyDuty.Utilities;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI;

namespace DailyDuty.Features.WondrousTails;

public unsafe class WondrousTails : Module<Config, DataBase> {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Wondrous Tails",
        FileName = "MiniCactpot",
        Type = ModuleType.Daily,
        ChangeLog = [
            new ChangeLogInfo(1, "Initial Re-Implementation"),
        ],
        Tags = [ "DoH", "DoL", "Exp" ],
    };

    private DutyController? dutyController;
    private ContentsFinderController? contentsFinderController;

    public override DataNodeBase DataNode => new DataNode(this);
    public override ConfigNodeBase ConfigNode => new ConfigNode(this);

    protected override void OnModuleEnable() {
        dutyController = new DutyController(this);
        contentsFinderController = new ContentsFinderController(this);
    }

    protected override void OnModuleDisable() {
        contentsFinderController?.Dispose();
        contentsFinderController = null;
        
        dutyController?.Dispose();
        dutyController = null;
    }

    protected override StatusMessage GetStatusMessage() => this switch {
        { PlayerHasBook: true, IsBookExpired: false } when ModuleConfig.StickerAvailableNotice && IsStickerAvailable => new StatusMessage {
            Message = "Sticker Available",
            PayloadId = PayloadId.OpenWondrousTailsBook,
        },
        
        { SecondChancePoints: > 7, PlacedStickers: >= 3 and <= 7, PlayerHasBook: true, IsBookExpired: false } when ModuleConfig.ShuffleAvailableNotice => new StatusMessage {
            Message = "Shuffle Available",
            PayloadId = PayloadId.OpenWondrousTailsBook,
        },
        
        { IsNewBookAvailable: true } when ModuleConfig.UnclaimedBookWarning  => new StatusMessage {
            Message = "New Book Available",
            PayloadId = PayloadId.IdyllshireTeleport,
        },
        
        _ => new StatusMessage {
            Message = $"{9 - PlacedStickers} Stickers Remaining",
            PayloadId = PayloadId.OpenWondrousTailsBook,
        },
    };

    protected override void OnModuleUpdate() {
        base.OnModuleUpdate();

        var lastNearKhloe = false;
        var lastCastingTeleport = false;
        
        const int idyllshireTerritoryType = 478;
        const uint khloeAliapohDataId = 1017653;
        if (Services.ClientState.TerritoryType is idyllshireTerritoryType && IsEnabled) {
            var khloe = Services.ObjectTable.FirstOrDefault(obj => obj.BaseId is khloeAliapohDataId);

            if (khloe is not null && Services.ObjectTable.LocalPlayer is { Position: var playerPosition }) {
                var distanceToKhloe = Vector3.Distance(playerPosition, khloe.Position);
                var closeToKhloe = distanceToKhloe < 10.0f;
                var castingTeleport = Services.ObjectTable.LocalPlayer is { IsCasting: true, CastActionId: 5 or 6 };

                var noLongerNearKhloe = lastNearKhloe && !closeToKhloe;
                var startedTeleportingAway = lastNearKhloe && !lastCastingTeleport && castingTeleport;
                
                if ((noLongerNearKhloe || startedTeleportingAway) && this is { PlayerHasBook: false, IsNewBookAvailable: true }) {
                    Services.ChatGui.PrintTaggedMessage("Wait! You forgot you book!", ModuleInfo.DisplayName);
                    UIGlobals.PlayChatSoundEffect(11);
                }
            }
        }
    }

    public override DateTime GetNextResetDateTime()
        => Time.NextWeeklyReset();

    public override TimeSpan GetResetPeriod()
        => TimeSpan.FromDays(7);

    protected override CompletionStatus GetCompletionStatus() {
        if (ModuleConfig.UnclaimedBookWarning && IsNewBookAvailable) return CompletionStatus.Incomplete;

        return PlacedStickers is 9 ? CompletionStatus.Complete : CompletionStatus.Incomplete;
    }

    public bool PlayerHasBook
        => PlayerState.Instance()->HasWeeklyBingoJournal;

    public DateTime Deadline
        => DateTimeOffset.FromUnixTimeSeconds(PlayerState.Instance()->GetWeeklyBingoExpireUnixTimestamp()).DateTime;

    public uint SecondChancePoints
        => PlayerState.Instance()->WeeklyBingoNumSecondChancePoints;

    public int PlacedStickers
        => PlayerState.Instance()->WeeklyBingoNumPlacedStickers;

    public bool IsNewBookAvailable
        => DateTime.UtcNow > Deadline - TimeSpan.FromDays(7);

    public bool IsBookExpired
        => PlayerState.Instance()->IsWeeklyBingoExpired();
    
    private bool IsStickerAvailable 
    	=> Enumerable.Range(0, 16).Select(index => PlayerState.Instance()->GetWeeklyBingoTaskStatus(index)).Any(taskStatus => taskStatus is PlayerState.WeeklyBingoTaskStatus.Claimable);
}
