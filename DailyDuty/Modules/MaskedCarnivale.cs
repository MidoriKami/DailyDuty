using DailyDuty.Interfaces;
using DailyDuty.UserInterface.Components;
using System;
using System.Linq;
using DailyDuty.Addons;
using DailyDuty.DataModels;
using DailyDuty.Localization;
using DailyDuty.System;
using DailyDuty.Utilities;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.Configuration;
using KamiLib.InfoBoxSystem;
using KamiLib.Interfaces;
using KamiLib.Utilities;

namespace DailyDuty.Modules;

public class MaskedCarnivaleSettings : GenericSettings
{
    public TrackedMaskedCarnivale[] TrackedTasks =
    {
        new(CarnivaleTask.Novice, new Setting<bool>(true), false),
        new(CarnivaleTask.Moderate, new Setting<bool>(true), false),
        new(CarnivaleTask.Advanced, new Setting<bool>(true), false)
    };

    public Setting<bool> EnableClickableLink = new(true);
}

internal class MaskedCarnivale : IModule
{
    public ModuleName Name => ModuleName.MaskedCarnivale;
    public IConfigurationComponent ConfigurationComponent { get; }
    public IStatusComponent StatusComponent { get; }
    public ILogicComponent LogicComponent { get; }
    public ITodoComponent TodoComponent { get; }
    public ITimerComponent TimerComponent { get; }

    private static MaskedCarnivaleSettings Settings => Service.ConfigurationManager.CharacterConfiguration.MaskedCarnivale;
    public GenericSettings GenericSettings => Settings;

    public MaskedCarnivale()
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

            InfoBox.Instance
                .AddTitle(Strings.Module.GrandCompany.TrackedJobs)
                .AddList(Settings.TrackedTasks)
                .Draw();
            
            InfoBox.Instance
                .AddTitle(Strings.Module.MaskedCarnivale.ClickableLinkLabel)
                .AddString(Strings.Module.MaskedCarnivale.ClickableLink)
                .AddConfigCheckbox(Strings.Common.Enabled, Settings.EnableClickableLink)
                .Draw();
            
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
            if (ParentModule.LogicComponent is not ModuleLogicComponent logicModule) return;

            var moduleStatus = logicModule.GetModuleStatus();

            InfoBox.Instance
                .AddTitle(Strings.Status.Label)
                .BeginTable()
                .BeginRow()
                .AddString(Strings.Status.ModuleStatus)
                .AddString(moduleStatus.GetTranslatedString(), moduleStatus.GetStatusColor())
                .EndRow()
                .EndTable()
                .Draw();
            
            if (Settings.TrackedTasks.Any(row => row.Tracked.Value))
            {
                InfoBox.Instance
                    .AddTitle(Strings.Status.ModuleStatus)
                    .BeginTable()
                    .AddRows(Settings.TrackedTasks.Where(row => row.Tracked.Value))
                    .EndTable()
                    .Draw();
            }
            else
            {
                InfoBox.Instance
                    .AddTitle(Strings.Status.ModuleStatus)
                    .AddString(Strings.Module.MaskedCarnivale.NoTasksTracked, Colors.Orange)
                    .Draw();
            }
        }
    }

    private unsafe class ModuleLogicComponent : ILogicComponent
    {
        public IModule ParentModule { get; }
        public DalamudLinkPayload? DalamudLinkPayload { get; }
        public bool LinkPayloadActive => Settings.EnableClickableLink.Value;

        private AgentInterface* AozContentBriefingAgentInterface => Framework.Instance()->GetUiModule()->GetAgentModule()->GetAgentByInternalId(AgentId.AozContentBriefing);
        
        private delegate byte IsWeeklyCompleteDelegate(AgentInterface* agent, byte index);
        [Signature("4C 8B C1 80 FA 03")] private readonly IsWeeklyCompleteDelegate isWeeklyCompleted = null!;
        
        public ModuleLogicComponent(IModule parentModule)
        {
            ParentModule = parentModule;
            
            DalamudLinkPayload = Service.TeleportManager.GetPayload(TeleportLocation.UlDah);
            
            SignatureHelper.Initialise(this);
            
            Service.AddonManager.Get<AOZContentResultAddon>().Setup += OnSetup;
            Service.Framework.Update += OnFrameworkUpdate;
        }

        public void Dispose()
        {
            Service.AddonManager.Get<AOZContentResultAddon>().Setup -= OnSetup;
            Service.Framework.Update -= OnFrameworkUpdate;
        }
        
        private void OnSetup(object? sender, AOZContentResultArgs e)
        {
            switch (e.CompletionType)
            {
                // Novice
                case 0 when e.Successful:
                    SetTaskState(CarnivaleTask.Novice, true);
                    break;
                
                // Moderate 
                case 1 when e.Successful:
                    SetTaskState(CarnivaleTask.Moderate, true);
                    break;
                
                // Advanced
                case 2 when e.Successful:
                    SetTaskState(CarnivaleTask.Advanced, true);
                    break;
                    
                // Other
                case 3:
                    break;
            }
        }
        
        private void OnFrameworkUpdate(Dalamud.Game.Framework framework)
        {
            if (!Settings.Enabled.Value) return;
            if (!AozContentBriefingAgentInterface->IsAgentActive()) return;
            
            foreach (var task in Settings.TrackedTasks)
            {
                var completed = isWeeklyCompleted(AozContentBriefingAgentInterface, (byte) task.Task) != 0;

                if (task.State != completed)
                {
                    task.State = completed;
                    Service.ConfigurationManager.Save();
                }
            }
        }
        
        public string GetStatusMessage() => $"{GetIncompleteCount()} {Strings.Module.MaskedCarnivale.AllowancesRemaining}";
        
        public DateTime GetNextReset() => Time.NextWeeklyReset();

        public void DoReset()
        {
            foreach (var task in Settings.TrackedTasks)
            {
                task.State = false;
            }
        }

        public ModuleStatus GetModuleStatus() => GetIncompleteCount() == 0 ? ModuleStatus.Complete : ModuleStatus.Incomplete;

        private int GetIncompleteCount() => Settings.TrackedTasks.Where(task => task.Tracked.Value && !task.State).Count();

        private void SetTaskState(CarnivaleTask task, bool completedState)
        {
            foreach (var trackedTask in Settings.TrackedTasks)
            {
                if (trackedTask.Task == task)
                {
                    trackedTask.State = completedState;
                    Service.ConfigurationManager.Save();
                }
            }
        }
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

        public string GetShortTaskLabel() => Strings.Module.MaskedCarnivale.Label;

        public string GetLongTaskLabel() => Strings.Module.MaskedCarnivale.Label;
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