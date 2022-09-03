using System;
using DailyDuty.Addons.DataModels;
using DailyDuty.Addons.Enums;
using DailyDuty.Interfaces;
using Dalamud.Hooking;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace DailyDuty.Addons;

internal unsafe class CommendationAddon : IAddon
{
    public AddonName Name => AddonName.Commendations;

    public event EventHandler<IntPtr>? OnShow;
    public event EventHandler<IntPtr>? OnDraw;
    public event EventHandler<IntPtr>? OnFinalize;
    public event EventHandler<IntPtr>? OnUpdate;
    public event EventHandler<IntPtr>? OnRefresh;
    public event EventHandler<ReceiveEventArgs>? OnReceiveEvent;

    private delegate void AgentShow(AgentInterface* agent);
    private delegate void* AgentReceiveEvent(AgentInterface* agent, void* rawData, AtkValue* eventArgs, uint eventArgsCount, ulong sender);

    private readonly Hook<AgentReceiveEvent>? receiveEventHook;
    private readonly Hook<AgentShow>? showEventHook;

    public CommendationAddon()
    {
        var commendationAgentInterface = Framework.Instance()->UIModule->GetAgentModule()->GetAgentByInternalId(AgentId.ContentsMvp);

        receiveEventHook ??= Hook<AgentReceiveEvent>.FromAddress(new IntPtr(commendationAgentInterface->VTable->ReceiveEvent), CommendationReceiveEvent);
        showEventHook ??= Hook<AgentShow>.FromAddress(new IntPtr(commendationAgentInterface->VTable->Show), CommendationShow);

        receiveEventHook?.Enable();
        showEventHook?.Enable();
    }

    public void Dispose()
    {
        receiveEventHook?.Dispose();
        showEventHook?.Dispose();
    }

    private void* CommendationReceiveEvent(AgentInterface* agent, void* rawData, AtkValue* eventArgs, uint eventArgsCount, ulong sender)
    {
        try
        {
            OnReceiveEvent?.Invoke(this, new ReceiveEventArgs(agent, rawData, eventArgs, eventArgsCount, sender));
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, "Something went wrong when the Commendations Addon was opened");
        }

        return receiveEventHook!.Original(agent, rawData, eventArgs, eventArgsCount, sender);
    }

    private void CommendationShow(AgentInterface* agent)
    {
        try
        {
            OnShow?.Invoke(this, new IntPtr(agent));
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, "Something went wrong when the Commendations Addon was opened");
        }

        showEventHook!.Original(agent);
    }
}