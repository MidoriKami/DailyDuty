using System;
using DailyDuty.DataModels;
using DailyDuty.System;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.Hooking;

namespace DailyDuty.Addons;

public unsafe class AddonLotteryWeekly : IDisposable
{
    private static AddonLotteryWeekly? _instance;
    public static AddonLotteryWeekly Instance => _instance ??= new AddonLotteryWeekly();
    
    public event EventHandler<ReceiveEventArgs>? ReceiveEvent;

    private readonly Hook<Delegates.Agent.ReceiveEvent>? agentShowHook;

    private AddonLotteryWeekly()
    {
        AddonManager.AddAddon(this);
        
        var agent = Framework.Instance()->UIModule->GetAgentModule()->GetAgentByInternalId(AgentId.LotteryWeekly);

        agentShowHook ??= Hook<Delegates.Agent.ReceiveEvent>.FromAddress(new nint(agent->VTable->ReceiveEvent), OnReceiveEvent);
        agentShowHook?.Enable();
    }

    public void Dispose()
    {
        agentShowHook?.Dispose();
    }

    private nint OnReceiveEvent(AgentInterface* addon, nint a2, AtkValue* eventData, uint eventDataItemCount, ulong senderID)
    {
        Safety.ExecuteSafe(() =>
        {
            ReceiveEvent?.Invoke(this, new ReceiveEventArgs(addon, a2, eventData, eventDataItemCount, senderID));
        });
        
        return agentShowHook!.Original(addon, a2, eventData, eventDataItemCount, senderID);
    }
}