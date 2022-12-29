using DailyDuty.Interfaces;
using DailyDuty.UserInterface.Components;
using System;
using DailyDuty.Addons;
using DailyDuty.DataModels;
using DailyDuty.Localization;
using DailyDuty.Utilities;
using Dalamud.Game;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.InfoBoxSystem;

namespace DailyDuty.Modules;

public class GrandCompanySquadronSettings : GenericSettings
{
    public bool MissionCompleted;
}

internal class GrandCompanySquadron : IModule
{
    public ModuleName Name => ModuleName.GrandCompanySquadron;
    public IConfigurationComponent ConfigurationComponent { get; }
    public IStatusComponent StatusComponent { get; }
    public ILogicComponent LogicComponent { get; }
    public ITodoComponent TodoComponent { get; }
    public ITimerComponent TimerComponent { get; }

    private static GrandCompanySquadronSettings Settings => Service.ConfigurationManager.CharacterConfiguration.GrandCompanySquadron;
    public GenericSettings GenericSettings => Settings;

    public GrandCompanySquadron()
    {
        ConfigurationComponent = new ModuleConfigurationComponent(this);
        StatusComponent = new ModuleStatusComponent(this);
        LogicComponent = new ModuleLogicComponent(this);
        TodoComponent = new ModuleTodoComponent(this);
        TimerComponent = new ModuleTimerComponent(this);
    }

    public void Dispose()
    {
        LogicComponent.Dispose();
    }

    private class ModuleConfigurationComponent : IConfigurationComponent
    {
        public IModule ParentModule { get; }
        public ISelectable Selectable => new ConfigurationSelectable(ParentModule, this);

        public ModuleConfigurationComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public void Draw()
        {
            InfoBox.Instance.DrawGenericSettings(this);

            InfoBox.Instance.DrawNotificationOptions(this);
        }
    }

    private class ModuleStatusComponent : IStatusComponent
    {
        public IModule ParentModule { get; }

        public ISelectable Selectable => new StatusSelectable(ParentModule, this, ParentModule.LogicComponent.GetModuleStatus);

        public ModuleStatusComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public void Draw()
        {
            InfoBox.Instance.DrawGenericStatus(this);
        }
    }

    private unsafe class ModuleLogicComponent : ILogicComponent
    {
        public IModule ParentModule { get; }
        public DalamudLinkPayload? DalamudLinkPayload => null;
        public bool LinkPayloadActive => false;
        private UIModule* UIModule => FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance()->GetUiModule();
        private AgentInterface* GcArmyExpeditionAgent => UIModule->GetAgentModule()->GetAgentByInternalId(AgentId.GcArmyExpedition);

        public ModuleLogicComponent(IModule parentModule)
        {
            ParentModule = parentModule;

            Service.AddonManager.Get<GcArmyExpeditionResult>().Setup += OnSetup;
            Service.Framework.Update += OnFrameworkUpdate;
        }
        
        public void Dispose()
        {
            Service.AddonManager.Get<GcArmyExpeditionResult>().Setup -= OnSetup;
            Service.Framework.Update -= OnFrameworkUpdate;
        }

        private void OnSetup(object? sender, ExpeditionResultArgs e)
        {
            if (e.MissionType == 3 && e.Successful)
            {
                Settings.MissionCompleted = true;
                Service.ConfigurationManager.Save();
            }
        }
        
        private void OnFrameworkUpdate(Framework framework)
        {
            if (!Settings.Enabled.Value) return;
            if (!GcArmyExpeditionAgent->IsAgentActive()) return;
            
            var selectedTab = *((byte*) GcArmyExpeditionAgent + 64);
            if (selectedTab != 2) return;

            // This data block contains all the information for the tasks in the selected tab
            var dataBlockAddress = new IntPtr(*(long*)((byte*) GcArmyExpeditionAgent + 40));
            var weeklyCompleted = *((byte*) dataBlockAddress.ToPointer() + 128) == 0;

            if (Settings.MissionCompleted != weeklyCompleted)
            {
                Settings.MissionCompleted = weeklyCompleted;
                Service.ConfigurationManager.Save();
            }
        }
        
        public string GetStatusMessage() => Strings.Module.GrandCompany.SquadronMessage;

        public DateTime GetNextReset() => Time.NextWeeklyReset();

        public void DoReset() => Settings.MissionCompleted = false;

        public ModuleStatus GetModuleStatus() => Settings.MissionCompleted ? ModuleStatus.Complete : ModuleStatus.Incomplete;
    }

    private class ModuleTodoComponent : ITodoComponent
    {
        public IModule ParentModule { get; }
        public CompletionType CompletionType => CompletionType.Weekly;
        public bool HasLongLabel => false;

        public ModuleTodoComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public string GetShortTaskLabel() => Strings.Module.GrandCompany.SquadronLabel;

        public string GetLongTaskLabel() => Strings.Module.GrandCompany.SquadronLabel;
    }

    private class ModuleTimerComponent : ITimerComponent
    {
        public IModule ParentModule { get; }

        public ModuleTimerComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public TimeSpan GetTimerPeriod() => TimeSpan.FromDays(7);
        public DateTime GetNextReset() => Time.NextWeeklyReset();
    }
}