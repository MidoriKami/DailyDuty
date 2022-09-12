using System;
using DailyDuty.Addons;
using DailyDuty.Configuration.Components;
using DailyDuty.Configuration.Enums;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.System;
using DailyDuty.UserInterface.Components;
using DailyDuty.UserInterface.Components.InfoBox;
using DailyDuty.UserInterface.Windows;
using DailyDuty.Utilities;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Utility.Signatures;

namespace DailyDuty.Modules;

public class FashionReportSettings : GenericSettings
{
    public int AllowancesRemaining = 4;
    public int HighestWeeklyScore;
    public Setting<FashionReportMode> Mode = new(FashionReportMode.Single);
    public Setting<bool> EnableClickableLink = new(false);
}

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
                .SameLine(110.0f)
                .AddConfigRadio(Strings.Module.FashionReport.Mode80Plus, Settings.Mode, FashionReportMode.Plus80, Strings.Module.FashionReport.Mode80PlusHelp)
                .SameLine(220.0f)
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
                .BeginRow()
                .AddString(Strings.Status.ModuleStatus)
                .AddString(moduleStatus.GetTranslatedString(), moduleStatus.GetStatusColor())
                .EndRow()
                .BeginRow()
                .AddString(Strings.Module.FashionReport.AllowancesAvailable)
                .AddString(Settings.AllowancesRemaining.ToString())
                .EndRow()
                .BeginRow()
                .AddString(Strings.Module.FashionReport.HighestScore)
                .AddString(Settings.HighestWeeklyScore.ToString())
                .EndRow()
                .EndTable()
                .Draw();

            reportAvailable
                .AddTitle(Strings.Module.FashionReport.ReportOpen)
                .BeginTable()
                .BeginRow()
                .AddString(Strings.Module.FashionReport.ReportOpen)
                .AddString(logicModule.FashionReportAvailable() ? Strings.Module.FashionReport.AvailableNow : logicModule.GetNextFashionReport(),
                    logicModule.FashionReportAvailable() ? Colors.Green : Colors.Orange)
                .EndRow()
                .EndTable()
                .Draw();
        }
    }

    private unsafe class ModuleLogicComponent : ILogicComponent
    {
        public IModule ParentModule { get; }
        public DalamudLinkPayload? DalamudLinkPayload { get; } = Service.TeleportManager.GetPayload(TeleportLocation.GoldSaucer);

        public ModuleLogicComponent(IModule parentModule)
        {
            ParentModule = parentModule;

            SignatureHelper.Initialise(this);

            Service.AddonManager.Get<GoldSaucerAddon>().OnGoldSaucerUpdate += OnGoldSaucerUpdate;
        }

        public void Dispose()
        {
            Service.AddonManager.Get<GoldSaucerAddon>().OnGoldSaucerUpdate -= OnGoldSaucerUpdate;
        }

        private void OnGoldSaucerUpdate(object? sender, GoldSaucerEventArgs e)
        {
            if (Service.TargetManager.Target?.DataId != 1025176) return;

            var allowances = Settings.AllowancesRemaining;
            var score = Settings.HighestWeeklyScore;

            switch (e.EventID)
            {
                case 5:     // When speaking to Masked Rose, gets update information
                    allowances = e.Data[1];
                    score = e.Data[0];
                    break;

                case 3:     // During turn in, gets new score
                    score = e.Data[0];
                    break;
                    
                case 1:     // During turn in, gets new allowances
                    allowances = e.Data[0];
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

        public TimeSpan GetTimerPeriod() => TimeSpan.FromDays(4);

        public DateTime GetNextReset() => Time.NextFashionReportReset();
    }
}