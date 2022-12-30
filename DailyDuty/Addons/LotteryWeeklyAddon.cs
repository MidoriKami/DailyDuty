using System;
using DailyDuty.DataModels;
using DailyDuty.System;
using Dalamud.Hooking;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace DailyDuty.Addons;

public unsafe class LotteryWeeklyAddon : IDisposable
{
    private static LotteryWeeklyAddon? _instance;
    public static LotteryWeeklyAddon Instance => _instance ??= new LotteryWeeklyAddon();
    
    public event EventHandler<ReceiveEventArgs>? ReceiveEvent;

    private delegate void* AgentReceiveEvent(AgentInterface* addon, void* a2, AtkValue* eventData, uint eventDataItemCount, ulong senderID);
    private readonly Hook<AgentReceiveEvent>? agentShowHook;

    private LotteryWeeklyAddon()
    {
        AddonManager.AddAddon(this);
        
        var agent = Framework.Instance()->UIModule->GetAgentModule()->GetAgentByInternalId(AgentId.LotteryWeekly);

        agentShowHook ??= Hook<AgentReceiveEvent>.FromAddress(new IntPtr(agent->VTable->ReceiveEvent), OnReceiveEvent);
        agentShowHook?.Enable();
    }

    public void Dispose()
    {
        agentShowHook?.Dispose();
    }

    public void* OnReceiveEvent(AgentInterface* addon, void* a2, AtkValue* eventData, uint eventDataItemCount, ulong senderID)
    {
        try
        {
            ReceiveEvent?.Invoke(this, new ReceiveEventArgs(addon, a2, eventData, eventDataItemCount, senderID));
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, "LotteryWeekly Receive Event ran into a problem");
        }

        return agentShowHook!.Original(addon, a2, eventData, eventDataItemCount, senderID);
    }
}