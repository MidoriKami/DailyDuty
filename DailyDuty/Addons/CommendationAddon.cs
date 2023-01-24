using System;
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

    private readonly Hook<Delegates.Agent.Show>? showCommendationAgent;
    private readonly Hook<Delegates.Agent.Show>? showBannerCommendationAgent;

    private CommendationAddon()
    {
        AddonManager.AddAddon(this);
        
        var commendationAgentInterface = Framework.Instance()->UIModule->GetAgentModule()->GetAgentByInternalId(AgentId.ContentsMvp);
        showCommendationAgent ??= Hook<Delegates.Agent.Show>.FromAddress(new nint(commendationAgentInterface->VTable->Show), OnCommendationShow);
        showCommendationAgent?.Enable();
        
        var commendationBannerAgentInterface = Framework.Instance()->UIModule->GetAgentModule()->GetAgentByInternalId(AgentId.BannerMIP);
        showBannerCommendationAgent ??= Hook<Delegates.Agent.Show>.FromAddress(new nint(commendationBannerAgentInterface->VTable->Show), OnBannerCommendationShow);
        showBannerCommendationAgent?.Enable();
    }

    public void Dispose()
    {
        showCommendationAgent?.Dispose();
        showBannerCommendationAgent?.Dispose();
    }

    private void OnCommendationShow(AgentInterface* agent)
    {
        Safety.ExecuteSafe(() =>
        {
            Show?.Invoke(this, new nint(agent));
        });
        
        showCommendationAgent!.Original(agent);
    }
    
    private void OnBannerCommendationShow(AgentInterface* agent)
    {
        Safety.ExecuteSafe(() =>
        {
            Show?.Invoke(this, new nint(agent));
        });
        
        showBannerCommendationAgent!.Original(agent);
    }
}