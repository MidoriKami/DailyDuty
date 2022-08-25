using System;
using DailyDuty.Configuration.Components;
using DailyDuty.Configuration.Enums;
using DailyDuty.Configuration.ModuleSettings;
using DailyDuty.Interfaces;
using DailyDuty.System;
using DailyDuty.System.Localization;
using DailyDuty.UserInterface.Components;
using DailyDuty.UserInterface.Components.InfoBox;
using DailyDuty.UserInterface.Windows;
using DailyDuty.Utilities;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud.Utility.Signatures;

namespace DailyDuty.Modules;

internal class FashionReport : IModule
{
    public ModuleName Name => ModuleName.FashionReport;
    public IConfigurationComponent ConfigurationComponent { get; }
    public IStatusComponent StatusComponent { get; }
    public ILogicComponent LogicComponent { get; }
    public ITodoComponent TodoComponent { get; }
    public ITimerComponent TimerComponent { get; }

    private static FashionReportSettings Settings => Service.ConfigurationManager.CharacterConfiguration.FashionReport;
    public GenericSettings GenericSettings => Settings;

    public FashionReport()
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
        private readonly InfoBox modeSelect = new();
        private readonly InfoBox clickableLink = new();
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

            modeSelect
                .AddTitle(Strings.Module.FashionReport.CompletionCondition)
                .AddConfigRadio(Strings.Module.FashionReport.ModeSingle, Settings.Mode, FashionReportMode.Single, Strings.Module.FashionReport.ModeSingleHelp)
                .AddConfigRadio(Strings.Module.FashionReport.Mode80Plus, Settings.Mode, FashionReportMode.Plus80, Strings.Module.FashionReport.Mode80PlusHelp)
                .AddConfigRadio(Strings.Module.FashionReport.ModeAll, Settings.Mode, FashionReportMode.All, Strings.Module.FashionReport.ModeAllHelp)
                .Draw();

            clickableLink
                .AddTitle(Strings.Module.FashionReport.ClickableLinkLabel)
                .AddString(Strings.Module.FashionReport.ClickableLink)
                .AddConfigCheckbox(Strings.Common.Enabled, Settings.EnableClickableLink)
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

        public ISelectable Selectable => new StatusSelectable(ParentModule, this, ParentModule.LogicComponent.GetModuleStatus);

        private readonly InfoBox status = new();
        private readonly InfoBox reportAvailable = new();

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

                .AddRow(
                    Strings.Module.FashionReport.AllowancesAvailable,
                    Settings.AllowancesRemaining.ToString()
                    )

                .AddRow(
                    Strings.Module.FashionReport.HighestScore,
                    Settings.HighestWeeklyScore.ToString()
                    )

                .EndTable()
                .Draw();

            reportAvailable
                .AddTitle(Strings.Module.FashionReport.ReportOpen)
                .BeginTable()

                .AddRow(
                    Strings.Module.FashionReport.ReportOpen,
                    logicModule.FashionReportAvailable() ? Strings.Module.FashionReport.AvailableNow : logicModule.GetNextFashionReport(),
                    secondColor: logicModule.FashionReportAvailable() ? Colors.Green : Colors.Orange)

                .EndTable()
                .Draw();
        }
    }

    private unsafe class ModuleLogicComponent : ILogicComponent
    {
        public IModule ParentModule { get; }
        public DalamudLinkPayload? DalamudLinkPayload { get; } = Service.TeleportManager.GetPayload(TeleportLocation.DomanEnclave);

        private delegate void* GoldSaucerUpdateDelegate(void* a1, byte* a2, uint a3, ushort a4, void* a5, int* a6, byte a7);

        [Signature("E8 ?? ?? ?? ?? 80 A7 ?? ?? ?? ?? ?? 48 8D 8F ?? ?? ?? ?? 44 89 AF", DetourName = nameof(GoldSaucerUpdate))]
        private readonly Hook<GoldSaucerUpdateDelegate>? goldSaucerUpdateHook = null;

        public ModuleLogicComponent(IModule parentModule)
        {
            ParentModule = parentModule;

            SignatureHelper.Initialise(this);

            goldSaucerUpdateHook?.Enable();
        }

        public void Dispose()
        {
            goldSaucerUpdateHook?.Dispose();
        }

        private void* GoldSaucerUpdate(void* a1, byte* a2, uint a3, ushort a4, void* a5, int* a6, byte a7)
        {
            try
            {
                if (Service.TargetManager.Target?.DataId == 1025176)
                {
                    int allowances = Settings.AllowancesRemaining;
                    int score = Settings.HighestWeeklyScore;

                    switch (a7)
                    {
                        // When speaking to Masked Rose, gets update information
                        case 5:
                            allowances = a6[1];
                            score = a6[0];
                            break;

                        // During turn in, gets new score
                        case 3:
                            score = a6[0];
                            break;

                        // During turn in, gets new allowances
                        case 1:
                            allowances = a6[0];
                            break;
                    }

                    if (Settings.AllowancesRemaining != allowances)
                    {
                        Settings.AllowancesRemaining = allowances;
                        Service.ConfigurationManager.Save();
                    }

                    if (Settings.HighestWeeklyScore != score)
                    {
                        Settings.HighestWeeklyScore = score;
                        Service.ConfigurationManager.Save();
                    }
                }
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "[Fashion Report] Unable to get data from Gold Saucer Update");
            }

            return goldSaucerUpdateHook!.Original(a1, a2, a3, a4, a5, a6, a7);
        }

        public string GetStatusMessage()
        {
            switch(Settings.Mode.Value)
            {
                case FashionReportMode.All:
                case FashionReportMode.Single when Settings.AllowancesRemaining == 4:
                    return $"{Settings.AllowancesRemaining} {Strings.Module.FashionReport.AllowancesAvailable}";

                case FashionReportMode.Plus80 when Settings.HighestWeeklyScore <= 80:
                    return $"{Settings.HighestWeeklyScore} {Strings.Module.FashionReport.HighestScore}";

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public DateTime GetNextReset() => Time.NextWeeklyReset();

        public void DoReset()
        {
            Settings.AllowancesRemaining = 4;
            Settings.HighestWeeklyScore = 0;
        }

        public ModuleStatus GetModuleStatus()
        {
            if (FashionReportAvailable() == false) return ModuleStatus.Unavailable;

            // Zero is always "Complete"
            // Four is always "Incomplete"
            if (Settings.AllowancesRemaining == 0) return ModuleStatus.Complete;
            if (Settings.AllowancesRemaining == 4) return ModuleStatus.Incomplete;

            // If this line is reached, then we have between 1 and 3 remaining allowances (inclusive)
            switch (Settings.Mode.Value)
            {
                case FashionReportMode.Single:
                case FashionReportMode.All when Settings.AllowancesRemaining == 0:
                case FashionReportMode.Plus80 when Settings.HighestWeeklyScore >= 80:
                    return ModuleStatus.Complete;

                default:
                    return ModuleStatus.Incomplete;
            }
        }

        public bool FashionReportAvailable()
        {
            var reportOpen = Time.NextFashionReportReset();
            var reportClosed = Time.NextWeeklyReset();

            var now = DateTime.UtcNow;

            return now > reportOpen && now < reportClosed;
        }

        public string GetNextFashionReport()
        {
            var span = Time.NextFashionReportReset() - DateTime.UtcNow;

            return TimersOverlayWindow.FormatTimespan(span, TimerStyle.Full);
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

        public string GetShortTaskLabel() => Strings.Module.FashionReport.Label;

        public string GetLongTaskLabel() => Strings.Module.FashionReport.Label;
    }


    private class ModuleTimerComponent : ITimerComponent
    {
        public IModule ParentModule { get; }

        public ModuleTimerComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public DateTime GetNextReset()
        {
            var now = DateTime.UtcNow;

            var fashionReportOpen = Time.NextFashionReportReset();
            var fashionReportClose = Time.NextWeeklyReset();

            if (now > fashionReportOpen && now < fashionReportClose)
            {
                return Time.NextWeeklyReset().AddDays(3);
            }
            else
            {
                return Time.NextFashionReportReset();
            }
        }

        public TimeSpan GetTimerPeriod() => TimeSpan.FromDays(7);
    }
}