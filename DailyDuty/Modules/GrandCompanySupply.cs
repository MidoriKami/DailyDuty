using System;
using System.Linq;
using DailyDuty.DataModels;
using DailyDuty.DataStructures;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.UserInterface.Components;
using DailyDuty.Utilities;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.Configuration;
using KamiLib.InfoBoxSystem;
using KamiLib.Interfaces;
using KamiLib.Utilities;

namespace DailyDuty.Modules;

public class GrandCompanySupplySettings : GenericSettings
{
    public TrackedGrandCompanySupplyProvisioning[] TrackedSupply =
    {
        new(8, new Setting<bool>(true), false),
        new(9, new Setting<bool>(true), false),
        new(10, new Setting<bool>(true), false),
        new(11, new Setting<bool>(true), false),
        new(12, new Setting<bool>(true), false),
        new(13, new Setting<bool>(true), false),
        new(14, new Setting<bool>(true), false),
        new(15, new Setting<bool>(true), false),
    };
}

internal class GrandCompanySupply : IModule
{
    public ModuleName Name => ModuleName.GrandCompanySupply;
    public IConfigurationComponent ConfigurationComponent { get; }
    public IStatusComponent StatusComponent { get; }
    public ILogicComponent LogicComponent { get; }
    public ITodoComponent TodoComponent { get; }
    public ITimerComponent TimerComponent { get; }

    private static GrandCompanySupplySettings Settings => Service.ConfigurationManager.CharacterConfiguration.GrandCompanySupply;
    public GenericSettings GenericSettings => Settings;

    public GrandCompanySupply()
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
                .AddTitle(Strings.GrandCompany_Tracked)
                .AddList(Settings.TrackedSupply)
                .Draw();

            InfoBox.Instance.DrawNotificationOptions(this);
        }
    }

    private class ModuleStatusComponent : IStatusComponent
    {
        public IModule ParentModule { get; }

        public ISelectable Selectable => new StatusSelectable(ParentModule, this, ParentModule.LogicComponent.Status);

        public ModuleStatusComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }
        
        public void Draw()
        {
            InfoBox.Instance.DrawGenericStatus(this);

            if (Settings.TrackedSupply.Any(row => row.Tracked))
            {
                InfoBox.Instance
                    .AddTitle(Strings.Status_ModuleData)
                    .BeginTable()
                    .AddDataRows(Settings.TrackedSupply.Where(row => row.Tracked))
                    .EndTable()
                    .Draw();
            }
            else
            {
                InfoBox.Instance
                    .AddTitle(Strings.Status_ModuleData, out var innerWidth)
                    .AddStringCentered(Strings.GrandCompany_NothingTracked, innerWidth, Colors.Orange)
                    .Draw();
            }
            
            InfoBox.Instance
                .AddTitle(Strings.Common_NextReset)
                .BeginTable()
                .BeginRow()
                .AddString(Strings.Common_NextReset)
                .AddString(ModuleLogicComponent.GetNextGrandCompanyReset())
                .EndRow()
                .EndTable()
                .Draw();
            
            InfoBox.Instance.DrawSuppressionOption(this);
        }
    }

    private unsafe class ModuleLogicComponent : ILogicComponent
    {
        public IModule ParentModule { get; }
        public DalamudLinkPayload? DalamudLinkPayload => null;
        public bool LinkPayloadActive => false;

        private AgentInterface* GrandCompanySupplyAgent => Framework.Instance()->GetUiModule()->GetAgentModule()->GetAgentByInternalId(AgentId.GrandCompanySupply);

        public ModuleLogicComponent(IModule parentModule)
        {
            ParentModule = parentModule;

            Service.Framework.Update += OnFrameworkUpdate;
        }
        
        public void Dispose()
        {
            Service.Framework.Update -= OnFrameworkUpdate;
        }

        private void OnFrameworkUpdate(Dalamud.Game.Framework framework)
        {
            if (!Settings.Enabled) return;
            if (GrandCompanySupplyAgent == null) return;
            if (!GrandCompanySupplyAgent->IsAgentActive()) return;

            var dataArray = new GrandCompanyDataArray(new IntPtr(GrandCompanySupplyAgent));

            foreach (var tracked in Settings.TrackedSupply)
            {
                var dataRow = dataArray.GetRowForJob(tracked.ClassJobID);

                var turnInState = !dataRow.IsTurnInAvailable;

                if (turnInState != tracked.State)
                {
                    tracked.State = turnInState;
                    Service.ConfigurationManager.Save();
                }
            }
        }

        public string GetStatusMessage() => $"{GetIncompleteJobs()} {Strings.Common_AllowancesAvailable}";

        public DateTime GetNextReset() => Time.NextGrandCompanyReset();

        public void DoReset()
        {
            foreach (var trackedJobs in Settings.TrackedSupply)
            {
                trackedJobs.State = false;
            }
        }

        public ModuleStatus GetModuleStatus() => GetIncompleteJobs() == 0 ? ModuleStatus.Complete : ModuleStatus.Incomplete;

        private static int GetIncompleteJobs()
        {
            return Settings.TrackedSupply
                .Where(r => r.Tracked)
                .Count(r => r.State == false);
        }
        
        public static string GetNextGrandCompanyReset()
        {
            var span = Time.NextGrandCompanyReset() - DateTime.UtcNow;

            return span.FormatTimespan(Settings.TimerSettings.TimerStyle.Value);
        }
    }

    private class ModuleTodoComponent : ITodoComponent
    {
        public IModule ParentModule { get; }
        public CompletionType CompletionType => CompletionType.Daily;
        public bool HasLongLabel => false;

        public ModuleTodoComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public string GetShortTaskLabel() => Strings.GrandCompany_SupplyLabel;

        public string GetLongTaskLabel() => Strings.GrandCompany_SupplyLabel;
    }

    private class ModuleTimerComponent : ITimerComponent
    {
        public IModule ParentModule { get; }

        public ModuleTimerComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public TimeSpan GetTimerPeriod() => TimeSpan.FromDays(1);

        public DateTime GetNextReset() => Time.NextGrandCompanyReset();
    }
}