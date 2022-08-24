using System;
using DailyDuty.Configuration.Components;
using DailyDuty.Configuration.Enums;
using DailyDuty.Configuration.ModuleSettings;
using DailyDuty.Interfaces;
using DailyDuty.Modules.Enums;
using DailyDuty.System.Localization;
using DailyDuty.UserInterface.Components;
using DailyDuty.UserInterface.Components.InfoBox;
using DailyDuty.Utilities;
using Dalamud.Game;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Utility.Signatures;

namespace DailyDuty.Modules;

internal class DomanEnclave : IModule
{
    public ModuleName Name => ModuleName.DomanEnclave;
    public IConfigurationComponent ConfigurationComponent { get; }
    public IStatusComponent StatusComponent { get; }
    public ILogicComponent LogicComponent { get; }
    public ITodoComponent TodoComponent { get; }
    public ITimerComponent TimerComponent { get; }

    private static DomanEnclaveSettings Settings => Service.ConfigurationManager.CharacterConfiguration.DomanEnclave;
    public GenericSettings GenericSettings => Settings;

    public DomanEnclave()
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
        private readonly InfoBox clickableLink = new();
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

            clickableLink
                .AddTitle(Strings.Module.DomanEnclave.ClickableLinkLabel)
                .AddString(Strings.Module.DomanEnclave.ClickableLink)
                .AddConfigCheckbox(Strings.Module.DomanEnclave.ClickableLinkLabel, Settings.EnableClickableLink)
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

        public ISelectable Selectable =>
            new StatusSelectable(ParentModule, this, ParentModule.LogicComponent.GetModuleStatus);

        private readonly InfoBox statusInfoBox = new();
        private readonly InfoBox warningInfoBox = new();

        public ModuleStatusComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public void Draw()
        {
            if (ParentModule.LogicComponent is not ModuleLogicComponent logicModule) return;

            var moduleStatus = logicModule.GetModuleStatus();

            statusInfoBox
                .AddTitle(Strings.Status.Label)
                .BeginTable()

                .AddRow(
                    Strings.Status.ModuleStatus,
                    moduleStatus.GetLocalizedString(),
                    secondColor: moduleStatus.GetStatusColor())

                .AddRow(
                    Strings.Module.DomanEnclave.BudgetRemaining,
                    logicModule.GetRemainingBudget().ToString(),
                    secondColor: logicModule.GetRemainingBudget() == 0 ? Colors.Green : Colors.Orange
                    )

                .AddRow(
                    Strings.Module.DomanEnclave.CurrentAllowance,
                    logicModule.GetWeeklyAllowance().ToString()
                )

                .EndTable()
                .Draw();

            if (moduleStatus == ModuleStatus.Unknown)
            {
                warningInfoBox
                    .AddTitle(Strings.Module.DomanEnclave.UnknownStatusLabel)
                    .AddString(Strings.Module.DomanEnclave.UnknownStatus, Colors.Orange)
                    .Draw();
            }
        }
    }

    private unsafe class ModuleLogicComponent : ILogicComponent
    {
        public IModule ParentModule { get; }
        public DalamudLinkPayload? DalamudLinkPayload { get; } = Service.TeleportManager.GetPayload(TeleportLocation.DomanEnclave);

        private delegate IntPtr GetPointerDelegate();

        [Signature("E8 ?? ?? ?? ?? 48 85 C0 74 09 0F B6 B8")]
        private readonly GetPointerDelegate getBasePointer = null!;

        public ModuleLogicComponent(IModule parentModule)
        {
            ParentModule = parentModule;
            SignatureHelper.Initialise(this);
            Service.Framework.Update += FrameworkOnUpdate;
        }

        public void Dispose()
        {
            Service.Framework.Update -= FrameworkOnUpdate;
        }
        private void FrameworkOnUpdate(Framework framework)
        {
            if (!DataAvailable()) return;

            UpdateWeeklyAllowance();
            UpdateDonatedThisWeek();
        }

        public string GetStatusMessage()
        {
            if (GetModuleStatus() == ModuleStatus.Unknown) return Strings.Module.DomanEnclave.UnknownStatus;

            return $"{GetRemainingBudget()} {Strings.Module.DomanEnclave.GilRemaining}";
        }

        public DateTime GetNextReset() => Time.NextWeeklyReset();

        public void DoReset() => Settings.DonatedThisWeek = 0;

        public ModuleStatus GetModuleStatus()
        {
            if (!ModuleInitialized()) return ModuleStatus.Unknown;

            return GetRemainingBudget() == 0 ? ModuleStatus.Complete : ModuleStatus.Incomplete;
        }

        private void UpdateWeeklyAllowance()
        {
            var allowance = GetWeeklyAllowance();

            if (Settings.WeeklyAllowance != allowance)
            {
                Settings.WeeklyAllowance = allowance;
                Service.ConfigurationManager.Save();
            }
        }
        private void UpdateDonatedThisWeek()
        {
            var donatedThisWeek = GetDonatedThisWeek();

            if (Settings.DonatedThisWeek != donatedThisWeek)
            {
                Settings.DonatedThisWeek = donatedThisWeek;
                Service.ConfigurationManager.Save();
            }
        }

        private ushort GetDonatedThisWeek()
        {
            var baseAddress = getBasePointer();
            var donatedThisWeek = *((ushort*) baseAddress + 80);

            return donatedThisWeek;
        }

        public ushort GetWeeklyAllowance()
        {
            var baseAddress = getBasePointer();
            var adjustedAddress = baseAddress + 166;

            var allowance = *(ushort*) adjustedAddress;

            return allowance;
        }

        public int GetRemainingBudget() => Settings.WeeklyAllowance - Settings.DonatedThisWeek;
        private bool DataAvailable() => GetWeeklyAllowance() != 0;
        public bool ModuleInitialized() => Settings.WeeklyAllowance != 0;
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

        public string GetShortTaskLabel() => Strings.Module.DomanEnclave.Label;

        public string GetLongTaskLabel()  => Strings.Module.DomanEnclave.Label;
    }


    private class ModuleTimerComponent : ITimerComponent
    {
        public IModule ParentModule { get; }

        public ModuleTimerComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public TimeSpan GetTimerPeriod() => TimeSpan.FromDays(7);
    }
}