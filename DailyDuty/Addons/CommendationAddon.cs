using System;
using DailyDuty.DataModels;
using Dalamud.Hooking;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace DailyDuty.Addons;

internal unsafe class CommendationAddon : IDisposable
{
    public event EventHandler<IntPtr>? Show;
    public event EventHandler<ReceiveEventArgs>? ReceiveEvent;

    private delegate void AgentShow(AgentInterface* agent);
    private delegate void* AgentReceiveEvent(AgentInterface* agent, void* rawData, AtkValue* eventArgs, uint eventArgsCount, ulong sender);

    private readonly Hook<AgentReceiveEvent>? receiveEventHook;
    private readonly Hook<AgentShow>? showEventHook;

    public CommendationAddon()
    {
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
        try
        {
            ReceiveEvent?.Invoke(this, new ReceiveEventArgs(agent, rawData, eventArgs, eventArgsCount, sender));
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, "Something went wrong when the Commendations Addon was opened");
        }

        return receiveEventHook!.Original(agent, rawData, eventArgs, eventArgsCount, sender);
    }

    private void OnShow(AgentInterface* agent)
    {
        try
        {
            Show?.Invoke(this, new IntPtr(agent));
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, "Something went wrong when the Commendations Addon was opened");
        }

        showEventHook!.Original(agent);
    }
}