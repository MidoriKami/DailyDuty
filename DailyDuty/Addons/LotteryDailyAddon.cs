using System;
using DailyDuty.Addons.DataModels;
using DailyDuty.Addons.Enums;
using DailyDuty.Interfaces;
using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace DailyDuty.Addons;

internal unsafe class LotteryDailyAddon : IAddon
{
    public AddonName Name => AddonName.MiniCactpot;

    public event EventHandler<IntPtr>? OnShow;
    public event EventHandler<IntPtr>? OnDraw;
    public event EventHandler<IntPtr>? OnFinalize;
    public event EventHandler<IntPtr>? OnUpdate;
    public event EventHandler<IntPtr>? OnRefresh;
    public event EventHandler<ReceiveEventArgs>? OnReceiveEvent;

    private delegate void* AgentShow(AgentInterface* agent, void* a2, void* a3);
    private readonly Hook<AgentShow>? agentShowHook;

    public LotteryDailyAddon()
    {
        SignatureHelper.Initialise(this);

        var agent = Framework.Instance()->UIModule->GetAgentModule()->GetAgentByInternalId(AgentId.LotteryDaily);

        agentShowHook ??= Hook<AgentShow>.FromAddress(new IntPtr(agent->VTable->Show), LotteryDaily_Show);
        agentShowHook?.Enable();
    }

    public void Dispose()
    {
        agentShowHook?.Dispose();
    }

    public void* LotteryDaily_Show(AgentInterface* addon, void* a2, void* a3)
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