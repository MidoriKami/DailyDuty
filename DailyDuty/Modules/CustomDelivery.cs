using System;
using DailyDuty.Configuration.Components;
using DailyDuty.Configuration.Enums;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.UserInterface.Components;
using DailyDuty.UserInterface.Components.InfoBox;
using DailyDuty.Utilities;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Utility.Signatures;

namespace DailyDuty.Modules;

internal class CustomDeliverySettings : GenericSettings
{
    public Setting<int> NotificationThreshold = new(12);
    public Setting<ComparisonMode> ComparisonMode = new(Configuration.Enums.ComparisonMode.EqualTo);
}

internal class CustomDelivery : IModule
{
    public ModuleName Name => ModuleName.CustomDelivery;
    public IConfigurationComponent ConfigurationComponent { get; }
    public IStatusComponent StatusComponent { get; }
    public ILogicComponent LogicComponent { get; }
    public ITodoComponent TodoComponent { get; }
    public ITimerComponent TimerComponent { get; }

    private static CustomDeliverySettings Settings => Service.ConfigurationManager.CharacterConfiguration.CustomDelivery;
    public GenericSettings GenericSettings => Settings;

    public CustomDelivery()
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

        private readonly InfoBox options = new();
        private readonly InfoBox completionConditions = new();
        private readonly InfoBox notificationOptions = new();

        public ModuleConfigurationComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public void Draw()
        {
            options
                .AddTitle(Strings.Configuration.Options)
                .AddConfigCheckbox(Strings.Common.Enabled, Settings.Enabled)
                .Draw();

            completionConditions
                .AddTitle(Strings.Configuration.MarkCompleteWhen)
                .BeginTable(0.40f)

                .BeginRow()
                .AddConfigCombo(Enum.GetValues<ComparisonMode>(), Settings.ComparisonMode, ComparisonModeExtensions.GetTranslatedString)
                .AddSliderInt(Strings.Common.Allowances, Settings.NotificationThreshold, 0, 12, 100.0f)
                .EndRow()

                .EndTable()
                .Draw();

            notificationOptions
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
            var allowances = logicModule.GetRemainingAllowances();

            status
                .AddTitle(Strings.Status.Label)
                .BeginTable()

                .BeginRow()
                .AddString(Strings.Status.ModuleStatus)
                .AddString(moduleStatus.GetTranslatedString(), moduleStatus.GetStatusColor())
                .EndRow()

                .BeginRow()
                .AddString(Strings.Common.Allowances)
                .AddString(allowances.ToString(), moduleStatus.GetStatusColor())
                .EndRow()

                .EndTable()
                .Draw();

            target
                .AddTitle(Strings.Common.Target)
                .BeginTable()

                .BeginRow()
                .AddString(Strings.Common.Mode)
                .AddString(Settings.ComparisonMode.Value.GetTranslatedString())
                .EndRow()

                .BeginRow()
                .AddString(Strings.Common.Target)
                .AddString(Settings.NotificationThreshold.Value.ToString())
                .EndRow()

                .EndTable()
                .Draw();
        }
    }

    private unsafe class ModuleLogicComponent : ILogicComponent
    {
        public IModule ParentModule { get; }
        public DalamudLinkPayload? DalamudLinkPayload => null;

        private delegate int GetCustomDeliveryAllowancesDelegate(byte* array);

        [Signature("0F B6 41 20 4C 8B C1")]
        private readonly GetCustomDeliveryAllowancesDelegate getCustomDeliveryAllowances = null!;

        [Signature("48 8D 0D ?? ?? ?? ?? 41 0F BA EC", ScanType = ScanType.StaticAddress)]
        private readonly byte* staticArrayPointer = null!;

        public ModuleLogicComponent(IModule parentModule)
        {
            ParentModule = parentModule;
            SignatureHelper.Initialise(this);
        }

        public void Dispose()
        {
        }

        public string GetStatusMessage() => $"{GetRemainingAllowances()} {Strings.Module.CustomDelivery.AllowancesRemaining}";

        public DateTime GetNextReset() => Time.NextWeeklyReset();

        public void DoReset()
        {
            // Do nothing
        }

        public ModuleStatus GetModuleStatus()
        {
            switch (Settings.ComparisonMode.Value)
            {
                case ComparisonMode.LessThan when Settings.NotificationThreshold.Value > GetRemainingAllowances():
                case ComparisonMode.EqualTo when Settings.NotificationThreshold.Value == GetRemainingAllowances():
                case ComparisonMode.LessThanOrEqual when Settings.NotificationThreshold.Value >= GetRemainingAllowances():
                    return ModuleStatus.Complete;

                default:
                    return ModuleStatus.Incomplete;
            }
        }

        public int GetRemainingAllowances()
        {
            return 12 - getCustomDeliveryAllowances(staticArrayPointer);
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

        public string GetShortTaskLabel() => Strings.Module.CustomDelivery.Label;

        public string GetLongTaskLabel() => Strings.Module.CustomDelivery.Label;
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