using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.Models.Enums;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using KamiLib.Caching;
using Lumina.Excel.GeneratedSheets;
using Condition = KamiLib.GameState.Condition;

namespace DailyDuty.System;

public unsafe class PayloadController : IDisposable
{
    private static PayloadController? _instance;
    public static PayloadController Instance => _instance ??= new PayloadController();
    
    private readonly Dictionary<PayloadId, DalamudLinkPayload> payloads = new();

    private PayloadController()
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

    private DalamudLinkPayload RegisterPayload(PayloadId id)
    {
        return id switch
        {
            PayloadId.OpenWondrousTailsBook => AddHandler(id, (_, _) =>
            {
                const uint wondrousTailsBookItemID = 2002023;
                
                if (InventoryManager.Instance()->GetInventoryItemCount(wondrousTailsBookItemID) == 1)
                {
                    AgentInventoryContext.Instance()->UseItem(wondrousTailsBookItemID);
                }
            }),
            PayloadId.OpenPartyFinder => AddHandler(id, (_, _) =>
            {
                var unlockComplete = QuestManager.IsQuestComplete(65781) && QuestManager.IsQuestComplete(66211);

                if (unlockComplete && !Condition.IsBoundByDuty())
                {
                    var partyFinderAgent = AgentModule.Instance()->GetAgentByInternalId(AgentId.LookingForGroup);
                    partyFinderAgent->Show();
                }
            }),
            PayloadId.IdyllshireTeleport => AddHandler(id, (_, _) =>
            {
                TeleporterController.Instance.Teleport(LuminaCache<Aetheryte>.Instance.GetRow(75)!);
            }),
            PayloadId.DomanEnclaveTeleport => AddHandler(id, (_, _) =>
            {
               TeleporterController.Instance.Teleport(LuminaCache<Aetheryte>.Instance.GetRow(127)!); 
            }),
            PayloadId.GoldSaucerTeleport => AddHandler(id, (_, _) =>
            {
                TeleporterController.Instance.Teleport(LuminaCache<Aetheryte>.Instance.GetRow(62)!); 
            }),
            PayloadId.UldahTeleport => AddHandler(id, (_, _) =>
            {
                TeleporterController.Instance.Teleport(LuminaCache<Aetheryte>.Instance.GetRow(9)!); 
            }),
            PayloadId.OpenDutyFinderRoulette => AddHandler(id, (_, _) =>
            {
                AgentContentsFinder.Instance()->OpenRouletteDuty(1);
            }),
            PayloadId.OpenDutyFinderRaid => AddHandler(id, (_, _) =>
            {
                var currentRaid = LuminaCache<ContentFinderCondition>.Instance
                    .Where(cfc => cfc.ContentType.Row is 5 && cfc.Unknown33 is 0 && cfc.Unknown28 is 1)
                    .Last();
                
                AgentContentsFinder.Instance()->OpenRegularDuty(currentRaid.RowId);
            }),
            PayloadId.OpenDutyFinderAllianceRaid => AddHandler(id, (_, _) =>
            {
                var currentAllianceRaid = LuminaCache<ContentFinderCondition>.Instance
                    .Where(cfc => cfc.ContentType.Row is 5 && cfc.Unknown33 is 0 && cfc.Unknown28 is 0)
                    .Last();

                AgentContentsFinder.Instance()->OpenRegularDuty(currentAllianceRaid.RowId);
            }),


            _ => throw new ArgumentOutOfRangeException(nameof(id), id, null)
        };
    }

    private DalamudLinkPayload AddHandler(PayloadId payloadId, Action<uint, SeString> action)
    {
        return Service.PluginInterface.AddChatLinkHandler((uint) payloadId, action);
    }
}