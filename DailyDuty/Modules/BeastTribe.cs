using DailyDuty.Interfaces;
using DailyDuty.Modules.Enums;
using DailyDuty.System.Localization;
using DailyDuty.UserInterface.Components;
using DailyDuty.UserInterface.Components.InfoBox;
using System;
using DailyDuty.Configuration.Components;
using DailyDuty.Configuration.Enums;
using Dalamud.Utility.Signatures;
using DailyDuty.Configuration.ModuleSettings;

namespace DailyDuty.Modules;

internal class BeastTribe : IModule
{
    public ModuleName Name => ModuleName.BeastTribe;
    public IConfigurationComponent ConfigurationComponent { get; }
    public IStatusComponent StatusComponent { get; }
    public ILogicComponent LogicComponent { get; }
    public ITodoComponent TodoComponent { get; }

    private static BeastTribeSettings Settings => Service.ConfigurationManager.CharacterConfiguration.BeastTribe;
    public GenericSettings GenericSettings => Settings;

    public BeastTribe()
    {
        ConfigurationComponent = new ModuleConfigurationComponent(this);
        StatusComponent = new ModuleStatusComponent(this);
        LogicComponent = new ModuleLogicComponent(this);
        TodoComponent = new ModuleTodoComponent(this);
    }

    private class ModuleConfigurationComponent : IConfigurationComponent
    {
        public IModule ParentModule { get; }
        public ISelectable Selectable => new ConfigurationSelectable(ParentModule, this);

        private readonly InfoBox optionsInfoBox = new();
        private readonly InfoBox completionConditionsInfoBox = new();
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

            completionConditionsInfoBox
                .AddTitle(Strings.Configuration.MarkCompleteWhen)
                .BeginTable(0.40f)
                .AddActions(
                    Actions.GetConfigComboAction(Enum.GetValues<ComparisonMode>(), Settings.ComparisonMode, ComparisonModeExtensions.GetLocalizedString),
                    Actions.GetSliderInt(Strings.Common.Allowances, Settings.NotificationThreshold, 0, 12))
                .EndTable()
                .Draw();

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

        public ISelectable Selectable => new StatusSelectable(ParentModule, this, ParentModule.LogicComponent.GetModuleStatus);

        private readonly InfoBox statusInfoBox = new();
        private readonly InfoBox targetInfoBox = new();

        public ModuleStatusComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public void Draw()
        {
            if (ParentModule.LogicComponent is not ModuleLogicComponent logicModule) return;

            var moduleStatus = logicModule.GetModuleStatus();
            var allowances = logicModule.GetRemainingAllowances();

            statusInfoBox
                .AddTitle(Strings.Status.Label)
                .BeginTable()

                .AddRow(
                    Strings.Status.ModuleStatus, 
                    moduleStatus.GetLocalizedString(), 
                    secondColor: moduleStatus.GetStatusColor())

                .AddRow(
                    Strings.Common.Allowances, 
                    allowances.ToString(), 
                    secondColor: moduleStatus.GetStatusColor())

                .EndTable()
                .Draw();

            targetInfoBox
                .AddTitle(Strings.Common.Target)
                .BeginTable()

                .AddRow(
                    Strings.Common.Mode,
                    Settings.ComparisonMode.Value.GetLocalizedString())

                .AddRow(
                    Strings.Common.Target,
                    Settings.NotificationThreshold.Value.ToString())

                .EndTable()
                .Draw();
        }
    }

    private class ModuleLogicComponent : ILogicComponent
    {
        public IModule ParentModule { get; }

        private delegate int GetBeastTribeAllowancesDelegate(IntPtr agent);
        private delegate IntPtr GetBeastTribeAgentDelegate();

        [Signature("45 33 C9 48 81 C1 ?? ?? ?? ?? 45 8D 51 02")]
        private readonly GetBeastTribeAllowancesDelegate getBeastTribeAllowance = null!;

        [Signature("E8 ?? ?? ?? ?? 0F B7 DB")]
        private readonly GetBeastTribeAgentDelegate getBeastTribeBasePointer = null!;

        public ModuleLogicComponent(IModule parentModule)
        {
            ParentModule = parentModule;
            SignatureHelper.Initialise(this);
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
            return getBeastTribeAllowance(getBeastTribeBasePointer());
        }
    }

    private class ModuleTodoComponent : ITodoComponent
    {
        public IModule ParentModule { get; }
        public Setting<bool> Enabled => Settings.TodoTaskEnabled;
        public CompletionType CompletionType => CompletionType.Daily;

        public ModuleTodoComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public string GetShortTaskLabel() => Strings.Module.BeastTribe.Label;

        public string GetLongTaskLabel() => Strings.Module.BeastTribe.Label;
    }
}