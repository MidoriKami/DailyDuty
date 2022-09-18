using DailyDuty.Addons.DataModels;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.System.Framework;

namespace DailyDuty.Addons;

internal unsafe class WeeklyPuzzleAddon : IDisposable
{
    public event EventHandler<IntPtr>? Show;
    public event EventHandler<ReceiveEventArgs>? ReceiveEvent;

    private delegate void* AgentShow(AgentInterface* agent, void* a2, void* a3);
    private delegate void* AgentReceiveEvent(AgentInterface* agent, void* rawData, AtkValue* eventArgs, uint eventArgsCount, ulong sender);

    private readonly Hook<AgentReceiveEvent>? onReceiveEventHook;
    private readonly Hook<AgentShow>? onShowHook;

    public WeeklyPuzzleAddon()
    {
        var agent = Framework.Instance()->GetUiModule()->GetAgentModule()->GetAgentByInternalID(245);

        onReceiveEventHook ??= Hook<AgentReceiveEvent>.FromAddress(new IntPtr(agent->VTable->ReceiveEvent), WeeklyPuzzle_ReceiveEvent);
        onReceiveEventHook?.Enable();

        onShowHook ??= Hook<AgentShow>.FromAddress(new IntPtr(agent->VTable->Show), WeeklyPuzzle_Show);
        onShowHook?.Enable();
    }

    public void Dispose()
    {
        onReceiveEventHook?.Dispose();
        onShowHook?.Dispose();
    }

    private void* WeeklyPuzzle_ReceiveEvent(AgentInterface* agent, void* rawData, AtkValue* eventArgs, uint eventArgsCount, ulong sender)
    {
        var result = onReceiveEventHook!.Original(agent, rawData, eventArgs, eventArgsCount, sender);

        try
        {
            var args = new ReceiveEventArgs(agent, rawData, eventArgs, eventArgsCount, sender);

            ReceiveEvent?.Invoke(this, args);

            args.PrintData();
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, "Something when wrong on Weekly Puzzle Receive Event");
        }

        return result;
    }

    public void* WeeklyPuzzle_Show(AgentInterface* addon, void* a2, void* a3)
    {
        try
        {
            Show?.Invoke(this, new IntPtr(addon));
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, "Unable to update Mini cactpot counts");
        }

        return onShowHook!.Original(addon, a2, a3);
    }
}