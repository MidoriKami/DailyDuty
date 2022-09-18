using System;
using DailyDuty.Configuration.Components;
using DailyDuty.DataStructures;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.UserInterface.Components;
using DailyDuty.UserInterface.Components.InfoBox;
using DailyDuty.UserInterface.Windows;
using DailyDuty.Utilities;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Utility.Signatures;

namespace DailyDuty.Modules;

public class LevequestSettings : GenericSettings
{
    public Setting<int> NotificationThreshold = new(95);
    public Setting<ComparisonMode> ComparisonMode = new(Configuration.Components.ComparisonMode.EqualTo);
}

internal class Levequest : IModule
{
    public ModuleName Name => ModuleName.Levequest;
    public IConfigurationComponent ConfigurationComponent { get; }
    public IStatusComponent StatusComponent { get; }
    public ILogicComponent LogicComponent { get; }
    public ITodoComponent TodoComponent { get; }
    public ITimerComponent TimerComponent { get; }

    private static LevequestSettings Settings => Service.ConfigurationManager.CharacterConfiguration.Levequest;
    public GenericSettings GenericSettings => Settings;

    public Levequest()
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
                .AddSliderInt(Strings.Common.Allowances, Settings.NotificationThreshold, 0, 100, 100.0f)
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
        private readonly InfoBox nextAllowances = new();

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
                .BeginRow()
                .AddString(Strings.Status.ModuleStatus)
                .AddString(moduleStatus.GetTranslatedString(), moduleStatus.GetStatusColor())
                .EndRow()
                .BeginRow()
                .AddString(Strings.Common.Allowances)
                .AddString(logicModule.GetRemainingAllowances().ToString())
                .EndRow()
                .BeginRow()
                .AddString(Strings.Module.Levequest.Accepted)
                .AddString(logicModule.GetAcceptedLeves().ToString())
                .EndRow()
                .EndTable()
                .Draw();

            nextAllowances
                .AddTitle(Strings.Module.Levequest.NextAllowance)
                .BeginTable()
                .BeginRow()
                .AddString(Strings.Module.Levequest.NextAllowance)
                .AddString(logicModule.GetNextLeviquest())
                .EndRow()
                .EndTable()
                .Draw();
        }
    }

    private unsafe class ModuleLogicComponent : ILogicComponent
    {
        public IModule ParentModule { get; }
        public DalamudLinkPayload? DalamudLinkPayload => null;

        [Signature("88 05 ?? ?? ?? ?? 0F B7 41 06", ScanType = ScanType.StaticAddress)]
        private readonly LevequestStruct* levequestStruct = null;

        public ModuleLogicComponent(IModule parentModule)
        {
            ParentModule = parentModule;

            SignatureHelper.Initialise(this);
        }

        public void Dispose()
        {

        }

        public string GetStatusMessage() => $"{GetRemainingAllowances()} {Strings.Module.Levequest.AllowancesRemaining}";

        public DateTime GetNextReset() => Time.NextLeveAllowanceReset();

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

        public int GetRemainingAllowances() => levequestStruct->AllowancesRemaining;

        public int GetAcceptedLeves() => levequestStruct->LevesAccepted;

        public string GetNextLeviquest()
        {
            var span = Time.NextLeveAllowanceReset() - DateTime.UtcNow;

            return TimersOverlayWindow.FormatTimespan(span, TimerStyle.Full);
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

        public string GetShortTaskLabel() => Strings.Module.Levequest.Label;

        public string GetLongTaskLabel() => Strings.Module.Levequest.Label;
    }


    private class ModuleTimerComponent : ITimerComponent
    {
        public IModule ParentModule { get; }

        public ModuleTimerComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public TimeSpan GetTimerPeriod() => TimeSpan.FromHours(12);

        public DateTime GetNextReset() => Time.NextLeveAllowanceReset();
    }
}