using System;
using DailyDuty.Addons;
using DailyDuty.DataModels;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.UserInterface.Components;
using DailyDuty.Utilities;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Interface;
using Dalamud.Utility.Signatures;
using KamiLib.Configuration;
using KamiLib.InfoBoxSystem;
using KamiLib.Interfaces;
using KamiLib.Utilities;

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

        public ModuleConfigurationComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public void Draw()
        {
            InfoBox.Instance.DrawGenericSettings(this);

            InfoBox.Instance
                .AddTitle(Strings.FashionReport_CompletionCondition)
                .AddConfigRadio(Strings.Common_Single, Settings.Mode, FashionReportMode.Single, Strings.FashionReport_SingleMode_Info)
                .SameLine(110.0f * ImGuiHelpers.GlobalScale)
                .AddConfigRadio(Strings.FashionReport_Mode80Plus, Settings.Mode, FashionReportMode.Plus80, Strings.FashionReport_80Mode_Info)
                .SameLine(220.0f * ImGuiHelpers.GlobalScale)
                .AddConfigRadio(Strings.Common_All, Settings.Mode, FashionReportMode.All, Strings.FashionReport_AllMode_Info)
                .Draw();

            InfoBox.Instance
                .AddTitle(Strings.Common_ClickableLink)
                .AddString(Strings.GoldSaucer_ClickableLink)
                .AddConfigCheckbox(Strings.Common_Enabled, Settings.EnableClickableLink)
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

            InfoBox.Instance
                .AddTitle(Strings.Status_Label)
                .BeginTable()
                .BeginRow()
                .AddString(Strings.Status_ModuleStatus)
                .AddString(moduleStatus.GetTranslatedString(), moduleStatus.GetStatusColor())
                .EndRow()
                .BeginRow()
                .AddString(Strings.Common_AllowancesAvailable)
                .AddString(Settings.AllowancesRemaining.ToString())
                .EndRow()
                .BeginRow()
                .AddString(Strings.FashionReport_HighestScore)
                .AddString(Settings.HighestWeeklyScore.ToString())
                .EndRow()
                .EndTable()
                .Draw();

            InfoBox.Instance
                .AddTitle(Strings.FashionReport_ReportOpen)
                .BeginTable()
                .BeginRow()
                .AddString(Strings.FashionReport_ReportOpen)
                .AddString(logicModule.FashionReportAvailable() ? Strings.Common_AvailableNow : ModuleLogicComponent.GetNextFashionReport(),
                    logicModule.FashionReportAvailable() ? Colors.Green : Colors.Orange)
                .EndRow()
                .EndTable()
                .Draw();
            
            InfoBox.Instance.DrawSuppressionOption(this);
        }
    }

    private unsafe class ModuleLogicComponent : ILogicComponent
    {
        public IModule ParentModule { get; }
        public DalamudLinkPayload? DalamudLinkPayload { get; }
        public bool LinkPayloadActive => Settings.EnableClickableLink;

        public ModuleLogicComponent(IModule parentModule)
        {
            ParentModule = parentModule;

            DalamudLinkPayload = TeleportManager.Instance.GetPayload(TeleportLocation.GoldSaucer);
            
            SignatureHelper.Initialise(this);

            GoldSaucerAddon.Instance.GoldSaucerUpdate += GoldSaucerUpdate;
        }

        public void Dispose()
        {
            GoldSaucerAddon.Instance.GoldSaucerUpdate -= GoldSaucerUpdate;
        }

        private void GoldSaucerUpdate(object? sender, GoldSaucerEventArgs e)
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
                    return $"{Settings.AllowancesRemaining} {Strings.Common_AllowancesAvailable}";

                case FashionReportMode.Plus80 when Settings.HighestWeeklyScore <= 80:
                    return $"{Settings.HighestWeeklyScore} {Strings.FashionReport_HighestScore}";

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

        public static string GetNextFashionReport()
        {
            var span = Time.NextFashionReportReset() - DateTime.UtcNow;

            return span.FormatTimespan(Settings.TimerSettings.TimerStyle.Value);
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

        public string GetShortTaskLabel() => Strings.FashionReport_Label;

        public string GetLongTaskLabel() => Strings.FashionReport_Label;
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