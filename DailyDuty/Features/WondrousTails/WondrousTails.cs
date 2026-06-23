using DailyDuty.Utilities;
using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using DailyDuty.Classes;
using DailyDuty.CustomNodes;
using DailyDuty.Enums;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI;

namespace DailyDuty.Features.WondrousTails;

public class WondrousTails : Module<WondrousTailsConfig, DataBase> {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = Strings.Wondrous_Tails,
        FileName = "WondrousTails",
        Type = ModuleType.Weekly,
        Tags = ["DoH", "DoL", "Exp"],
    };

    private WondrousTailsDutyController? dutyController;
    private WondrousTailsContentsFinderController? contentsFinderController;
    private bool closeToKhloe;
    private bool castingTeleport;

    public override DataNodeBase DataNode => new WondrousTailsDataNode(this);
    public override ConfigNodeBase ConfigNode => new WondrousTailsConfigNode(this);

    protected override async Task OnModuleEnable() {
        dutyController = new WondrousTailsDutyController(this);

        await Services.Framework.Run(() => {
            contentsFinderController = new WondrousTailsContentsFinderController(this);
        });
    }

    protected override async Task OnModuleDisable() {
        await Services.Framework.Run(() => {
            contentsFinderController?.Dispose();
        });
        contentsFinderController = null;

        dutyController?.Dispose();
        dutyController = null;
    }

    protected override StatusMessage GetStatusMessage() => this switch {
        { IsNewBookAvailable: true } when ModuleConfig.UnclaimedBookWarning => new StatusMessage {
            Message = Strings.New_Book_Available,
            PayloadId = PayloadId.IdyllshireTeleport,
        },

        { PlayerHasBook: true, IsBookExpired: false } when ModuleConfig.StickerAvailableNotice && IsStickerAvailable => new StatusMessage {
            Message = Strings.Sticker_Available,
            PayloadId = PayloadId.OpenWondrousTailsBook,
        },

        { SecondChancePoints: > 7, PlacedStickers: >= 3 and <= 7, PlayerHasBook: true, IsBookExpired: false } when ModuleConfig.ShuffleAvailableNotice => new StatusMessage {
            Message = Strings.Shuffle_Available,
            PayloadId = PayloadId.OpenWondrousTailsBook,
        },

        _ => new StatusMessage {
            Message = $"{9 - PlacedStickers} {Strings.Sticker_s__Remaining}",
            PayloadId = PayloadId.OpenWondrousTailsBook,
        },
    };

    protected override void OnModuleUpdate() {
        base.OnModuleUpdate();

        // Skip if this is being called from initialization, this state isn't required for module status.
        if (!ThreadSafety.IsMainThread) return;

        var lastNearKhloe = closeToKhloe;
        var lastCastingTeleport = castingTeleport;

        const int idyllshireTerritoryType = 478;
        const uint khloeAliapohDataId = 1017653;
        if (Services.ClientState.TerritoryType is idyllshireTerritoryType && IsEnabled) {
            var khloe = Services.ObjectTable.FirstOrDefault(obj => obj.BaseId is khloeAliapohDataId);

            if (khloe is not null && Services.ObjectTable.LocalPlayer is { Position: var playerPosition }) {
                var distanceToKhloe = Vector3.Distance(playerPosition, khloe.Position);
                closeToKhloe = distanceToKhloe < 10.0f;
                castingTeleport = Services.ObjectTable.LocalPlayer is { IsCasting: true, CastActionId: 5 or 6 };

                var noLongerNearKhloe = lastNearKhloe && !closeToKhloe;
                var startedTeleportingAway = lastNearKhloe && !lastCastingTeleport && castingTeleport;

                if ((noLongerNearKhloe || startedTeleportingAway) && this is { PlayerHasBook: false, IsNewBookAvailable: true }) {
                    Services.ChatGui.PrintTaggedMessage(Strings.Wait__You_forgot_your_Wondrous_Tails_book_, ModuleInfo.DisplayName);
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

    private unsafe bool IsStickerAvailable
        => Enumerable.Range(0, 16).Select(index => PlayerState.Instance()->GetWeeklyBingoTaskStatus(index)).Any(taskStatus => taskStatus is PlayerState.WeeklyBingoTaskStatus.Claimable);
}
