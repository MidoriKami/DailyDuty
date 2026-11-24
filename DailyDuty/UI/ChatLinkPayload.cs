using System;
using System.Linq;
using System.Threading;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.Interop;

namespace DailyDuty.UI;

public class ChatLinkPayload : IDisposable {
    private static uint commandIdCounter = 1;

    public DalamudLinkPayload Payload { get; }
    public Action ClickAction { get; }
    private uint CommandId { get; }

    /// <summary>
    /// Creates and registers a chat payload for use with dalamud.
    /// </summary>
    public ChatLinkPayload(Action payloadAction) {
        ClickAction = payloadAction;

        CommandId = commandIdCounter;
        Interlocked.Increment(ref commandIdCounter);

        Payload = IChatGui.Get().AddChatLinkHandler(CommandId, (_, _) => ClickAction());
    }

    public void Dispose()
        => IChatGui.Get().RemoveChatLinkHandler(CommandId);

    public static unsafe ChatLinkPayload TeleportToGrandCompany() => new(() => {
        uint? aetheryteId = IPlayerState.Get().GrandCompany.RowId switch {
            0 => 8, // Maelstorm - Limsa
            1 => 2, // Twin Adder - Gridania
            2 => 9, // Immortal Flames - Ul'dah
            _ => null,
        };

        if (aetheryteId is { } realAetheryteId) {
            Telepo.Instance()->Teleport(realAetheryteId, 0);
        }
    });

    public static unsafe ChatLinkPayload TeleportToGoldSaucer() => new(() =>
        Telepo.Instance()->Teleport(62, 0));

    public static unsafe ChatLinkPayload TeleportToDomanEnclave() => new(() =>
        Telepo.Instance()->Teleport(127, 0));

    public static unsafe ChatLinkPayload TeleportToIdyllshire() => new(() =>
        Telepo.Instance()->Teleport(75, 0));

    public static unsafe ChatLinkPayload OpenChallengeLog() => new(() =>
        UIModule.Instance()->ExecuteMainCommand(60));

    public static unsafe ChatLinkPayload OpenDutyRoulette() => new(() => {
        AgentContentsFinder.Instance()->OpenRouletteDuty(1);
        ClearDutyFinderSelection();
    });

    public static unsafe ChatLinkPayload OpenDutyFinderRaid() => new(() => {
        var currentRaid = IDataManager.Get().LimitedNormalRaidDuties.LastOrDefault();

        AgentContentsFinder.Instance()->OpenRegularDuty(currentRaid.RowId);
        ClearDutyFinderSelection();
    });

    public static unsafe ChatLinkPayload OpenDutyFinderAllianceRaid() => new(() => {
        var currentAllianceRaid = IDataManager.Get().LimitedAllianceRaidDuties.LastOrDefault();

        AgentContentsFinder.Instance()->OpenRegularDuty(currentAllianceRaid.RowId);
        ClearDutyFinderSelection();
    });

    public static unsafe ChatLinkPayload OpenWondrousTailsBook() => new(() => {
        const uint wondrousTailsBookItemId = 2002023;

        if (InventoryManager.Instance()->GetInventoryItemCount(wondrousTailsBookItemId) is 1) {
            AgentInventoryContext.Instance()->UseItem(wondrousTailsBookItemId);
        }
    });

    private static unsafe void ClearDutyFinderSelection() {
        using var returnValue = new RentedAtkValues(1);
        using var command = new RentedAtkValues(2);
        command[0].SetInt(12);
        command[1].SetInt(1);

        AgentContentsFinder.Instance()->ReceiveEvent(returnValue, command, (uint)command.Span.Length, 0);
    }
}
