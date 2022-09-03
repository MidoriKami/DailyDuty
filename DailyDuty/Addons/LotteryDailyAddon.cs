using System;
using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace DailyDuty.Addons;

internal unsafe class LotteryDailyAddon : IDisposable
{
    public event EventHandler<IntPtr>? OnShow;

    private delegate void* AgentShow(AgentInterface* agent, void* a2, void* a3);
    private readonly Hook<AgentShow>? agentShowHook;

    public LotteryDailyAddon()
    {
        SignatureHelper.Initialise(this);

        var agent = Framework.Instance()->UIModule->GetAgentModule()->GetAgentByInternalId(AgentId.LotteryDaily);

        agentShowHook ??= Hook<AgentShow>.FromAddress(new IntPtr(agent->VTable->Show), Show);
        agentShowHook?.Enable();
    }

    public void Dispose()
    {
        agentShowHook?.Dispose();
    }

    public void* Show(AgentInterface* addon, void* a2, void* a3)
    {
        try
        {
            OnShow?.Invoke(this, new IntPtr(addon));
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, "Unable to update Mini cactpot counts");
        }

        return agentShowHook!.Original(addon, a2, a3);
    }
}