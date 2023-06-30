using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.Models.Enums;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using KamiLib.Caching;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.System;

public unsafe class PayloadController : IDisposable
{
    private static PayloadController? _instance;
    public static PayloadController Instance => _instance ??= new PayloadController();
    
    private readonly Dictionary<PayloadId, DalamudLinkPayload> payloads = new();
    
    public void Initialize()
    {
        foreach (var payload in Enum.GetValues<PayloadId>())
        {
            payloads.Add(payload, RegisterPayload(payload));
        }
    }

    public static void Cleanup()
    {
        _instance?.Dispose();
    }
    
    public void Dispose()
    {
        foreach (var registeredPayload in payloads)
        {
            Service.PluginInterface.RemoveChatLinkHandler((uint)registeredPayload.Key);
        }
    }

    public DalamudLinkPayload GetPayload(PayloadId id)
    {
        if (payloads.TryGetValue(id, out var value))
        {
            return value;
        }
        
        throw new Exception("Tried to get payload that isn't registered.");
    }

    private DalamudLinkPayload RegisterPayload(PayloadId id) => AddHandler(id, GetDelegateForPayload(id));

    public Action<uint, SeString> GetDelegateForPayload(PayloadId payload) => payload switch
    {
        PayloadId.OpenWondrousTailsBook => (_, _) =>
        {
            const uint wondrousTailsBookItemID = 2002023;
                
            if (InventoryManager.Instance()->GetInventoryItemCount(wondrousTailsBookItemID) == 1)
            {
                AgentInventoryContext.Instance()->UseItem(wondrousTailsBookItemID);
            }
        },
        PayloadId.IdyllshireTeleport => (_, _) =>
        {
            TeleporterController.Instance.Teleport(LuminaCache<Aetheryte>.Instance.GetRow(75)!);
        },
        PayloadId.DomanEnclaveTeleport => (_, _) =>
        {
            TeleporterController.Instance.Teleport(LuminaCache<Aetheryte>.Instance.GetRow(127)!);
        },
        PayloadId.OpenDutyFinderRoulette => (_, _) =>
        {
            AgentContentsFinder.Instance()->OpenRouletteDuty(1);
        },
        PayloadId.OpenDutyFinderRaid => (_, _) =>
        {
            var currentRaid = LuminaCache<ContentFinderCondition>.Instance
                .Where(cfc => cfc.ContentType.Row is 5 && cfc.Unknown33 is 0 && cfc.Unknown28 is 1)
                .Last();
                
            AgentContentsFinder.Instance()->OpenRegularDuty(currentRaid.RowId);
        },
        PayloadId.OpenDutyFinderAllianceRaid => (_, _) =>
        {
            var currentAllianceRaid = LuminaCache<ContentFinderCondition>.Instance
                .Where(cfc => cfc.ContentType.Row is 5 && cfc.Unknown33 is 0 && cfc.Unknown28 is 0)
                .Last();

            AgentContentsFinder.Instance()->OpenRegularDuty(currentAllianceRaid.RowId);
        },
        PayloadId.GoldSaucerTeleport => (_, _) =>
        {
            TeleporterController.Instance.Teleport(LuminaCache<Aetheryte>.Instance.GetRow(62)!);
        },
        PayloadId.OpenPartyFinder => (_, _) =>
        {
            Framework.Instance()->GetUiModule()->ExecuteMainCommand(57);
        },
        PayloadId.UldahTeleport => (_, _) =>
        {
            TeleporterController.Instance.Teleport(LuminaCache<Aetheryte>.Instance.GetRow(9)!);
        },
        PayloadId.Unknown => (_, _) =>
        {
            PluginLog.Debug("Executed Unknown Payload.");
        },
        PayloadId.OpenChallengeLog => (_, _) =>
        {
            Framework.Instance()->GetUiModule()->ExecuteMainCommand(60);
        },
        _ => throw new ArgumentOutOfRangeException(nameof(payload), payload, null)
    };
    

    private DalamudLinkPayload AddHandler(PayloadId payloadId, Action<uint, SeString> action)
    {
        return Service.PluginInterface.AddChatLinkHandler((uint) payloadId, action);
    }
}