using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.Extensions;
using Lumina.Excel.Sheets;

namespace DailyDuty.Classes;

public enum PayloadId : uint {
    OpenWondrousTailsBook,
    IdyllshireTeleport,
    DomanEnclaveTeleport,
    OpenDutyFinderRoulette,
    OpenDutyFinderRaid,
    OpenDutyFinderAllianceRaid,
    GoldSaucerTeleport,
    OpenPartyFinder,
    UldahTeleport,
    Unknown,
    OpenChallengeLog,
}

public unsafe class PayloadController : IDisposable {
    private readonly Dictionary<PayloadId, DalamudLinkPayload> payloads = new();
    
    public PayloadController() {
        foreach (var payload in Enum.GetValues<PayloadId>()) {
            payloads.Add(payload, RegisterPayload(payload));
        }
    }
    
    public void Dispose() {
        foreach (var registeredPayload in payloads) {
            Service.Chat.RemoveChatLinkHandler((uint)registeredPayload.Key);
        }
    }

    public DalamudLinkPayload GetPayload(PayloadId id) {
        if (payloads.TryGetValue(id, out var value)) {
            return value;
        }
        
        throw new Exception("Tried to get payload that isn't registered.");
    }

    private static DalamudLinkPayload RegisterPayload(PayloadId id) 
        => AddHandler(id, GetDelegateForPayload(id));

    public static Action<uint, SeString> GetDelegateForPayload(PayloadId payload) => payload switch {
        PayloadId.OpenWondrousTailsBook => (_, _) => {
            const uint wondrousTailsBookItemId = 2002023;
                
            if (InventoryManager.Instance()->GetInventoryItemCount(wondrousTailsBookItemId) == 1) {
                AgentInventoryContext.Instance()->UseItem(wondrousTailsBookItemId);
            }
        },
        PayloadId.IdyllshireTeleport => (_, _) => {
            System.Teleporter.Teleport(75);
        },
        PayloadId.DomanEnclaveTeleport => (_, _) => {
            System.Teleporter.Teleport(127);
        },
        PayloadId.OpenDutyFinderRoulette => (_, _) => {
            AgentContentsFinder.Instance()->OpenRouletteDuty(1);
            ClearDutyFinderSelection();
        },
        PayloadId.OpenDutyFinderRaid => (_, _) => {
            var currentRaid = Service.DataManager.GetLimitedNormalRaidDuties().LastOrDefault();

            AgentContentsFinder.Instance()->OpenRegularDuty(currentRaid.RowId); 
            ClearDutyFinderSelection();
        },
        PayloadId.OpenDutyFinderAllianceRaid => (_, _) => {
            var currentAllianceRaid = Service.DataManager.GetExcelSheet<ContentFinderCondition>()
                .Where(cfc => cfc.ContentType.RowId is 5 && cfc is { RequiredExVersion.RowId: 0, Unknown28: true })
                .Last();

            AgentContentsFinder.Instance()->OpenRegularDuty(currentAllianceRaid.RowId);
        },
        PayloadId.GoldSaucerTeleport => (_, _) => {
            System.Teleporter.Teleport(62);
        },
        PayloadId.OpenPartyFinder => (_, _) => {
                Framework.Instance()->GetUIModule()->ExecuteMainCommand(57);
        },
        PayloadId.UldahTeleport => (_, _) => {
            System.Teleporter.Teleport(9);
        },
        PayloadId.Unknown => (_, _) => {
            Service.Log.Debug("Executed Unknown Payload.");
        },
        PayloadId.OpenChallengeLog => (_, _) => {
            Framework.Instance()->GetUIModule()->ExecuteMainCommand(60);
        },
        _ => throw new ArgumentOutOfRangeException(nameof(payload), payload, null),
    };
    
    private static DalamudLinkPayload AddHandler(PayloadId payloadId, Action<uint, SeString> action)
        => Service.Chat.AddChatLinkHandler((uint) payloadId, action);

    private static void ClearDutyFinderSelection() {
        var returnValue = stackalloc AtkValue[1];
        var command = stackalloc AtkValue[2];
        command[0].SetInt(12);
        command[1].SetInt(1);
                
        AgentContentsFinder.Instance()->ReceiveEvent(returnValue, command, 2, 0);
    }
}