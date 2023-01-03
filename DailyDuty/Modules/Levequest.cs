using System;
using DailyDuty.DataModels;
using DailyDuty.DataStructures;
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

public class LevequestSettings : GenericSettings
{
    public Setting<int> NotificationThreshold = new(95);
    public Setting<ComparisonMode> ComparisonMode = new(DataModels.ComparisonMode.EqualTo);
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
                .AddSliderInt(Strings.Common_Allowances, Settings.NotificationThreshold, 0, 100, innerWidth / 4.0f)
                .EndRow()
                .EndTable()
                .Draw();

            InfoBox.Instance.DrawNotificationOptions(this);
        }
    }

    private class ModuleStatusComponent : IStatusComponent
    {
        public IModule ParentModule { get; }

        public ISelectable Selectable =>
            new StatusSelectable(ParentModule, this, ParentModule.LogicComponent.Status);

        public ModuleStatusComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public void Draw()
        {
            if (ParentModule.LogicComponent is not ModuleLogicComponent logicModule) return;

            var moduleStatus = logicModule.GetModuleStatus();

            InfoBox.Instance
                .AddTitle(Strings.Status_Label)
                .BeginTable()
                .BeginRow()
                .AddString(Strings.Status_ModuleStatus)
                .AddString(moduleStatus.GetTranslatedString(), moduleStatus.GetStatusColor())
                .EndRow()
                .BeginRow()
                .AddString(Strings.Common_Allowances)
                .AddString(logicModule.GetRemainingAllowances().ToString())
                .EndRow()
                .BeginRow()
                .AddString(Strings.Common_Accepted)
                .AddString(logicModule.GetAcceptedLeves().ToString())
                .EndRow()
                .EndTable()
                .Draw();

            InfoBox.Instance
                .AddTitle(Strings.Levequest_NextAllowance)
                .BeginTable()
                .BeginRow()
                .AddString(Strings.Levequest_NextAllowance)
                .AddString(logicModule.GetNextLeviquest())
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

        public string GetStatusMessage() => $"{GetRemainingAllowances()} {Strings.Common_AllowancesRemaining}";

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

        public string GetShortTaskLabel() => Strings.Levequest_Label;

        public string GetLongTaskLabel() => Strings.Levequest_Label;
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