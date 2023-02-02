using System;
using DailyDuty.System;
using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.Hooking;

namespace DailyDuty.Addons;

public unsafe class AddonLotteryDaily : IDisposable
{
    private static AddonLotteryDaily? _instance;
    public static AddonLotteryDaily Instance => _instance ??= new AddonLotteryDaily();
    
    public event EventHandler<nint>? Show;

    private delegate void* DailyLotteryAgentShow(AgentInterface* agent, void* a2, void* a3);

    [Signature("40 53 57 41 55 48 81 EC ?? ?? ?? ?? 48 8B 05", DetourName = nameof(OnShow))]
    private readonly Hook<DailyLotteryAgentShow>? agentShowHook = null;

    private AddonLotteryDaily()
    {
        SignatureHelper.Initialise(this);
        AddonManager.AddAddon(this);
        
        agentShowHook?.Enable();
    }

    public void Dispose()
    {
        agentShowHook?.Dispose();
    }

    public void* OnShow(AgentInterface* addon, void* a2, void* a3)
    {
        Safety.ExecuteSafe(() =>
        {
            Show?.Invoke(this, new nint(addon));
        });
        
        return agentShowHook!.Original(addon, a2, a3);
    }
}