using System;
using DailyDuty.Configuration.Components;
using DailyDuty.Configuration.Enums;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.UserInterface.Components;
using DailyDuty.UserInterface.Components.InfoBox;
using DailyDuty.Utilities;
using Dalamud.Game.Text.SeStringHandling.Payloads;

namespace DailyDuty.Modules.Template;

internal class TemplateModule : IModule
{
    public ModuleName Name => ModuleName.TemplateModule;
    public IConfigurationComponent ConfigurationComponent { get; }
    public IStatusComponent StatusComponent { get; }
    public ILogicComponent LogicComponent { get; }
    public ITodoComponent TodoComponent { get; }
    public ITimerComponent TimerComponent { get; }

    private static TemplateModuleSettings Settings => Service.ConfigurationManager.CharacterConfiguration.TemplateModule;
    public GenericSettings GenericSettings => Settings;

    public TemplateModule()
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

        private readonly InfoBox optionsInfoBox = new();
        private readonly InfoBox notificationOptionsInfoBox = new();

        public ModuleConfigurationComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public void Draw()
        {
            optionsInfoBox
                .AddTitle(Strings.Configuration.Options)
                .AddConfigCheckbox(Strings.Common.Enabled, Settings.Enabled)
                .Draw();

            // Todo: add options

            notificationOptionsInfoBox
                .AddTitle(Strings.Configuration.NotificationOptions)
                .AddConfigCheckbox(Strings.Configuration.OnLogin, Settings.NotifyOnLogin)
                .AddConfigCheckbox(Strings.Configuration.OnZoneChange, Settings.NotifyOnZoneChange)
                .Draw();
        }
    }

    private class ModuleStatusComponent : IStatusComponent
    {
        public IModule ParentModule { get; }

        public ISelectable Selectable =>
            new StatusSelectable(ParentModule, this, ParentModule.LogicComponent.GetModuleStatus);

        private readonly InfoBox status = new();
        private readonly InfoBox target = new();

        public ModuleStatusComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public void Draw()
        {
            if (ParentModule.LogicComponent is not ModuleLogicComponent logicModule) return;

            var moduleStatus = logicModule.GetModuleStatus();


            status
                .AddTitle(Strings.Status.Label)
                .BeginTable()

                .AddRow(
                    Strings.Status.ModuleStatus,
                    moduleStatus.GetLocalizedString(),
                    secondColor: moduleStatus.GetStatusColor())


                .EndTable()
                .Draw();

            target
                .AddTitle(Strings.Common.Target)
                .BeginTable()

                .EndTable()
                .Draw();
        }
    }

    private class ModuleLogicComponent : ILogicComponent
    {
        public IModule ParentModule { get; }
        public DalamudLinkPayload? DalamudLinkPayload { get; }

        public ModuleLogicComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public void Dispose()
        {

        }

        public string GetStatusMessage()
        {
            // Todo: Set this
            return "StringNotSet";
        }

        // Todo: Set this
        public DateTime GetNextReset() => Time.NextDailyReset();

        public void DoReset()
        {
            // Todo: Set this
        }

        public ModuleStatus GetModuleStatus()
        {
            // Todo: Set this
            return ModuleStatus.Unknown;
        }
    }

    private class ModuleTodoComponent : ITodoComponent
    {
        public IModule ParentModule { get; }

        // Todo: Set completion type
        public CompletionType CompletionType => CompletionType.CompletionType;
        public bool HasLongLabel => HasLongLabel;

        public ModuleTodoComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        // Todo: Set this
        public string GetShortTaskLabel() => Strings.Module.TemplateModule.Label;

        public string GetLongTaskLabel()
        {
            // Todo: Set this
            return "StringNotSet";
        }
    }


    private class ModuleTimerComponent : ITimerComponent
    {
        public IModule ParentModule { get; }

        public ModuleTimerComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        // Todo: Confirm this
        public TimeSpan GetTimerPeriod() => TimeSpan.FromDays(1);

        // Todo: Confirm this
        public DateTime GetNextReset() => Time.NextDailyReset();
    }
}
