using DailyDuty.Addons.DataModels;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using Dalamud.Logging;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.System.Framework;

namespace DailyDuty.Addons;

internal unsafe class WeeklyPuzzleAddon : IDisposable
{
    public event EventHandler<IntPtr>? Show;
    public event EventHandler<ReceiveEventArgs>? ReceiveEvent;

    private delegate IntPtr WeeklyPuzzleOnSetup(AtkUnitBase* root);
    [Signature("48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC 30 48 8B F9 48 81 C1", DetourName = nameof(WeeklyPuzzle_OnSetup))]
    private readonly Hook<WeeklyPuzzleOnSetup>? onSetupHook = null;

    private delegate void* AgentReceiveEvent(AgentInterface* agent, void* rawData, AtkValue* eventArgs, uint eventArgsCount, ulong sender);

    private readonly Hook<AgentReceiveEvent>? onReceiveEventHook;

    public WeeklyPuzzleAddon()
    {
        SignatureHelper.Initialise(this);

        var agent = Framework.Instance()->GetUiModule()->GetAgentModule()->GetAgentByInternalID(245);

        onReceiveEventHook ??= Hook<AgentReceiveEvent>.FromAddress(new IntPtr(agent->VTable->ReceiveEvent), WeeklyPuzzle_ReceiveEvent);
        onReceiveEventHook?.Enable();

        onSetupHook?.Enable();
    }

    public void Dispose()
    {
        onReceiveEventHook?.Dispose();
        onSetupHook?.Dispose();
    }

    private void* WeeklyPuzzle_ReceiveEvent(AgentInterface* agent, void* rawData, AtkValue* eventArgs, uint eventArgsCount, ulong sender)
    {
        var result = onReceiveEventHook!.Original(agent, rawData, eventArgs, eventArgsCount, sender);

        try
        {
            var args = new ReceiveEventArgs(agent, rawData, eventArgs, eventArgsCount, sender);

            ReceiveEvent?.Invoke(this, args);

            //args.PrintData();
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, "Something when wrong on Weekly Puzzle Receive Event");
        }

        return result;
    }

    public IntPtr WeeklyPuzzle_OnSetup(AtkUnitBase* root)
    {
        try
        {
            Show?.Invoke(this, new IntPtr(root));
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, "Something went wrong opening Weekly Puzzle");
        }

        return onSetupHook!.Original(root);
    }
}