using System;
using DailyDuty.DataModels;
using DailyDuty.System;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.ExceptionSafety;

namespace DailyDuty.Addons;

public unsafe class CommendationAddon : IDisposable
{
    private static CommendationAddon? _instance;
    public static CommendationAddon Instance => _instance ??= new CommendationAddon();
    
    public event EventHandler<IntPtr>? Show;
    public event EventHandler<ReceiveEventArgs>? ReceiveEvent;

    private delegate void AgentShow(AgentInterface* agent);
    private delegate void* AgentReceiveEvent(AgentInterface* agent, void* rawData, AtkValue* eventArgs, uint eventArgsCount, ulong sender);

    private readonly Hook<AgentReceiveEvent>? receiveEventHook;
    private readonly Hook<AgentShow>? showEventHook;

    private CommendationAddon()
    {
        AddonManager.AddAddon(this);
        
        var commendationAgentInterface = Framework.Instance()->UIModule->GetAgentModule()->GetAgentByInternalId(AgentId.ContentsMvp);

        receiveEventHook ??= Hook<AgentReceiveEvent>.FromAddress(new IntPtr(commendationAgentInterface->VTable->ReceiveEvent), OnReceiveEvent);
        showEventHook ??= Hook<AgentShow>.FromAddress(new IntPtr(commendationAgentInterface->VTable->Show), OnShow);

        receiveEventHook?.Enable();
        showEventHook?.Enable();
    }

    public void Dispose()
    {
        receiveEventHook?.Dispose();
        showEventHook?.Dispose();
    }

    private void* OnReceiveEvent(AgentInterface* agent, void* rawData, AtkValue* eventArgs, uint eventArgsCount, ulong sender)
    {
        Safety.ExecuteSafe(() =>
        {
            ReceiveEvent?.Invoke(this, new ReceiveEventArgs(agent, rawData, eventArgs, eventArgsCount, sender));

        });

        return receiveEventHook!.Original(agent, rawData, eventArgs, eventArgsCount, sender);
    }

    private void OnShow(AgentInterface* agent)
    {
        Safety.ExecuteSafe(() =>
        {
            Show?.Invoke(this, new IntPtr(agent));

        });
        
        showEventHook!.Original(agent);
    }
}