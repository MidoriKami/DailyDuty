using DailyDuty.Interfaces;
using DailyDuty.UserInterface.Components;
using System;
using DailyDuty.DataModels;
using DailyDuty.Localization;
using DailyDuty.Utilities;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using KamiLib.Configuration;
using KamiLib.InfoBoxSystem;
using KamiLib.Interfaces;
using KamiLib.Utilities;

namespace DailyDuty.Modules;

public class BeastTribeSettings : GenericSettings
{
    public Setting<int> NotificationThreshold = new(12);
    public Setting<ComparisonMode> ComparisonMode = new(DataModels.ComparisonMode.EqualTo);
}

internal class TribalQuests : IModule
{
    public ModuleName Name => ModuleName.BeastTribe;
    public IConfigurationComponent ConfigurationComponent { get; }
    public IStatusComponent StatusComponent { get; }
    public ILogicComponent LogicComponent { get; }
    public ITodoComponent TodoComponent { get; }
    public ITimerComponent TimerComponent { get; }

    private static BeastTribeSettings Settings => Service.ConfigurationManager.CharacterConfiguration.BeastTribe;
    public GenericSettings GenericSettings => Settings;

    public TribalQuests()
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
            var allowances = ModuleLogicComponent.GetRemainingAllowances();

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

    private class ModuleLogicComponent : ILogicComponent
    {
        public IModule ParentModule { get; }
        public DalamudLinkPayload? DalamudLinkPayload => null;
        public bool LinkPayloadActive => false;

        public ModuleLogicComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public void Dispose()
        {
        }

        public string GetStatusMessage() => $"{GetRemainingAllowances()} {Strings.Common_AllowancesRemaining}";

        public DateTime GetNextReset() => Time.NextDailyReset();

        public void DoReset()
        {
            // Do Nothing
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

        public static int GetRemainingAllowances() => (int)PlayerState.GetBeastTribeAllowance();
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

        public string GetShortTaskLabel() => Strings.TribalQuests_Label;

        public string GetLongTaskLabel() => Strings.TribalQuests_Label;
    }

    private class ModuleTimerComponent : ITimerComponent
    {
        public IModule ParentModule { get; }

        public ModuleTimerComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public TimeSpan GetTimerPeriod() => TimeSpan.FromDays(1);
        public DateTime GetNextReset() => Time.NextDailyReset();
    }
}