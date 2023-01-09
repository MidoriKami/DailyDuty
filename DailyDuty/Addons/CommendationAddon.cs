using System;
using DailyDuty.DataModels;
using DailyDuty.System;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.Hooking;

namespace DailyDuty.Addons;

public unsafe class CommendationAddon : IDisposable
{
    private static CommendationAddon? _instance;
    public static CommendationAddon Instance => _instance ??= new CommendationAddon();
    
    public event EventHandler<nint>? Show;
    public event EventHandler<ReceiveEventArgs>? ReceiveEvent;

    private readonly Hook<Delegates.Agent.ReceiveEvent>? receiveEventHook;
    private readonly Hook<Delegates.Agent.Show>? showEventHook;

    private CommendationAddon()
    {
        AddonManager.AddAddon(this);
        
        var commendationAgentInterface = Framework.Instance()->UIModule->GetAgentModule()->GetAgentByInternalId(AgentId.ContentsMvp);

        receiveEventHook ??= Hook<Delegates.Agent.ReceiveEvent>.FromAddress(new nint(commendationAgentInterface->VTable->ReceiveEvent), OnReceiveEvent);
        showEventHook ??= Hook<Delegates.Agent.Show>.FromAddress(new nint(commendationAgentInterface->VTable->Show), OnShow);

        receiveEventHook?.Enable();
        showEventHook?.Enable();
    }

    public void Dispose()
    {
        receiveEventHook?.Dispose();
        showEventHook?.Dispose();
    }

    private nint OnReceiveEvent(AgentInterface* agent, nint rawData, AtkValue* eventArgs, uint eventArgsCount, ulong sender)
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
            Show?.Invoke(this, new nint(agent));

        });
        
        showEventHook!.Original(agent);
    }
}