using DailyDuty.Interfaces;
using DailyDuty.Addons;
using DailyDuty.DataModels;
using DailyDuty.Localization;
using Dalamud.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace DailyDuty.Modules;

public class GrandCompanySquadronSettings : GenericSettings
{
    public bool MissionCompleted;
}

public unsafe class GrandCompanySquadron : Module
{
    public override ModuleName Name => ModuleName.GrandCompanySquadron;
    public override CompletionType CompletionType => CompletionType.Weekly;

    private static GrandCompanySquadronSettings Settings => Service.ConfigurationManager.CharacterConfiguration.GrandCompanySquadron;
    public override GenericSettings GenericSettings => Settings;

    private AgentInterface* GcArmyExpeditionAgent => AgentModule.Instance()->GetAgentByInternalId(AgentId.GcArmyExpedition);
    
    public GrandCompanySquadron()
    {
        AddonGcArmyExpeditionResult.Instance.Setup += OnSetup;
        Service.Framework.Update += OnFrameworkUpdate;
    }
        
    public override void Dispose()
    {
        AddonGcArmyExpeditionResult.Instance.Setup -= OnSetup;
        Service.Framework.Update -= OnFrameworkUpdate;
    }

    private void OnSetup(object? sender, ExpeditionResultArgs e)
    {
        if (e is { MissionType: 3, Successful: true })
        {
            Settings.MissionCompleted = true;
            Service.ConfigurationManager.Save();
        }
    }
        
    private void OnFrameworkUpdate(Framework framework)
    {
        if (!Settings.Enabled) return;
        if (!GcArmyExpeditionAgent->IsAgentActive()) return;
            
        var selectedTab = *((byte*) GcArmyExpeditionAgent + 64);
        if (selectedTab != 2) return;

        // This data block contains all the information for the tasks in the selected tab
        var dataBlockAddress = new nint(*(long*)((byte*) GcArmyExpeditionAgent + 40));
        var weeklyCompleted = *((byte*) dataBlockAddress.ToPointer() + 128) == 0;

        if (Settings.MissionCompleted != weeklyCompleted)
        {
            Settings.MissionCompleted = weeklyCompleted;
            Service.ConfigurationManager.Save();
        }
    }
        
    public override string GetStatusMessage() => Strings.GrandCompany_MissionAvailable;
    public override void DoReset() => Settings.MissionCompleted = false;
    public override ModuleStatus GetModuleStatus() => Settings.MissionCompleted ? ModuleStatus.Complete : ModuleStatus.Incomplete;
}