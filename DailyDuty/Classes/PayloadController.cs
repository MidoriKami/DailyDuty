using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.Enums;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;

namespace DailyDuty.Classes;

public unsafe class PayloadController : IDisposable {
    private readonly Dictionary<PayloadId, DalamudLinkPayload> payloads = new();

    public PayloadController() {
        foreach (var payload in Enum.GetValues<PayloadId>()) {
            payloads.Add(payload, RegisterPayload(payload));
        }
    }
    
    public void Dispose() {
        foreach (var registeredPayload in payloads) {
            Services.ChatGui.RemoveChatLinkHandler((uint)registeredPayload.Key);
        }
    }

    public DalamudLinkPayload GetPayload(PayloadId id)
        => payloads.TryGetValue(id, out var value) ? value : throw new Exception("Tried to get payload that isn't registered.");

    private static DalamudLinkPayload RegisterPayload(PayloadId id) 
        => AddHandler(id, GetDelegateForPayload(id));

    private static Action<uint, SeString> GetDelegateForPayload(PayloadId payload) => payload switch {
        PayloadId.IdyllshireTeleport => (_, _) => Teleport(75),
        PayloadId.DomanEnclaveTeleport => (_, _) => Teleport(127),
        PayloadId.GoldSaucerTeleport => (_, _) => Teleport(62),
        PayloadId.OpenPartyFinder => (_, _) => Framework.Instance()->GetUIModule()->ExecuteMainCommand(57),
        PayloadId.UldahTeleport => (_, _) => Teleport(9),
        PayloadId.OpenChallengeLog => (_, _) => Framework.Instance()->GetUIModule()->ExecuteMainCommand(60),
        PayloadId.OpenWondrousTailsBook => OpenWondrousTailsBook,
        PayloadId.OpenDutyFinderRoulette => OpenDutyFinderRoulette,
        PayloadId.OpenDutyFinderRaid => OpenDutyFinderRaid,
        PayloadId.OpenDutyFinderAllianceRaid => OpenDutyFinderAllianceRaid,
        PayloadId.Unset => (_, _) => Services.PluginLog.Debug("Executed Unknown Payload."),
        _ => throw new ArgumentOutOfRangeException(nameof(payload), payload, null),
    };

    private static void OpenDutyFinderAllianceRaid(uint u, SeString s) {
        var currentAllianceRaid = Services.DataManager.GetExcelSheet<ContentFinderCondition>()
          .Where(cfc => cfc.ContentType.RowId is 5 && cfc is { RequiredExVersion.RowId: 0, Unknown28: true })
          .Last();

        AgentContentsFinder.Instance()->OpenRegularDuty(currentAllianceRaid.RowId);
    }

    private static void OpenDutyFinderRaid(uint u, SeString s) {
        var currentRaid = Services.DataManager.LimitedNormalRaidDuties.LastOrDefault();

        AgentContentsFinder.Instance()->OpenRegularDuty(currentRaid.RowId);
        ClearDutyFinderSelection();
    }

    private static void OpenDutyFinderRoulette(uint u, SeString s) {
        AgentContentsFinder.Instance()->OpenRouletteDuty(1);
        ClearDutyFinderSelection();
    }

    private static void OpenWondrousTailsBook(uint u, SeString s) {
        const uint wondrousTailsBookItemId = 2002023;

        if (InventoryManager.Instance()->GetInventoryItemCount(wondrousTailsBookItemId) == 1) {
            AgentInventoryContext.Instance()->UseItem(wondrousTailsBookItemId);
        }
    }

    private static DalamudLinkPayload AddHandler(PayloadId payloadId, Action<uint, SeString> action)
        => Services.ChatGui.AddChatLinkHandler((uint) payloadId, action);

    private static void ClearDutyFinderSelection() {
        var returnValue = stackalloc AtkValue[1];
        var command = stackalloc AtkValue[2];
        command[0].SetInt(12);
        command[1].SetInt(1);
                
        AgentContentsFinder.Instance()->ReceiveEvent(returnValue, command, 2, 0);
    }

    private static void Teleport(uint id) {
        if (!Services.DataManager.GetExcelSheet<Aetheryte>().TryGetRow(id, out var aetheryte)) return;
        
        Services.ChatGui.PrintTaggedMessage($"Teleporting to {aetheryte.PlaceName.Value.Name.ToString()}", "Teleport");
        Telepo.Instance()->Teleport(id, 0);
    }
}
