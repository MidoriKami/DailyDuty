using System;
using DailyDuty.DataModels;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.UserInterface.Components;
using DailyDuty.Utilities;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Utility.Signatures;
using KamiLib.Configuration;
using KamiLib.InfoBoxSystem;
using KamiLib.Interfaces;
using KamiLib.Utilities;

namespace DailyDuty.Modules;

internal class CustomDeliverySettings : GenericSettings
{
    public Setting<int> NotificationThreshold = new(12);
    public Setting<ComparisonMode> ComparisonMode = new(DataModels.ComparisonMode.LessThan);
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

        public ModuleConfigurationComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public void Draw()
        {
            InfoBox.Instance.DrawGenericSettings(this);

            InfoBox.Instance
                .AddTitle(Strings.Config_MarkCompleteWhen, out var innerWidth)
                .BeginTable()
                .BeginRow()
                .AddConfigCombo(Enum.GetValues<ComparisonMode>(), Settings.ComparisonMode, ComparisonModeExtensions.GetTranslatedString)
                .AddSliderInt(Strings.Common_Allowances, Settings.NotificationThreshold, 0, 12, innerWidth / 4.0f)
                .EndRow()
                .EndTable()
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
            if (ParentModule.LogicComponent is not ModuleLogicComponent logicModule) return;

            var moduleStatus = logicModule.GetModuleStatus();
            var allowances = logicModule.GetRemainingAllowances();

            InfoBox.Instance
                .AddTitle(Strings.Status_Label)
                .BeginTable()
                .BeginRow()
                .AddString(Strings.Status_ModuleStatus)
                .AddString(moduleStatus.GetTranslatedString(), moduleStatus.GetStatusColor())
                .EndRow()
                .BeginRow()
                .AddString(Strings.Common_Allowances)
                .AddString(allowances.ToString(), moduleStatus.GetStatusColor())
                .EndRow()
                .EndTable()
                .Draw();

            InfoBox.Instance
                .AddTitle(Strings.Common_Target)
                .BeginTable()
                .BeginRow()
                .AddString(Strings.Common_Mode)
                .AddString(Settings.ComparisonMode.Value.GetTranslatedString())
                .EndRow()
                .BeginRow()
                .AddString(Strings.Common_Target)
                .AddString(Settings.NotificationThreshold.Value.ToString())
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

        public string GetStatusMessage() => $"{GetRemainingAllowances()} {Strings.Common_AllowancesRemaining}";

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

        public string GetShortTaskLabel() => Strings.CustomDelivery_Label;

        public string GetLongTaskLabel() => Strings.CustomDelivery_Label;
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