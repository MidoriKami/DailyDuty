using System;
using System.Collections.Generic;
using DailyDuty.Models.Enums;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;

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
            
            _ => throw new ArgumentOutOfRangeException(nameof(id), id, null)
        };
    }

    private DalamudLinkPayload AddHandler(PayloadId payloadId, Action<uint, SeString> action)
    {
        return Service.PluginInterface.AddChatLinkHandler((uint) payloadId, action);
    }

}