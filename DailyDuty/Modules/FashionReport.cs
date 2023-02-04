using System;
using DailyDuty.Addons;
using DailyDuty.DataModels;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.Utilities;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Interface;
using KamiLib.Configuration;
using KamiLib.Drawing;
using KamiLib.Misc;
using KamiLib.Teleporter;

namespace DailyDuty.Modules;

public class FashionReportSettings : GenericSettings
{
    public int AllowancesRemaining = 4;
    public int HighestWeeklyScore;
    public Setting<FashionReportMode> Mode = new(FashionReportMode.Single);
    public Setting<bool> EnableClickableLink = new(false);
}

public unsafe class FashionReport : AbstractModule
{
    public override ModuleName Name => ModuleName.FashionReport;
    public override CompletionType CompletionType => CompletionType.Weekly;

    private static FashionReportSettings Settings => Service.ConfigurationManager.CharacterConfiguration.FashionReport;
    public override GenericSettings GenericSettings => Settings;
    public override DalamudLinkPayload DalamudLinkPayload => TeleportManager.Instance.GetPayload(TeleportLocation.GoldSaucer);
    public override bool LinkPayloadActive => Settings.EnableClickableLink;
    
    public FashionReport()
    {
        AgentGoldSaucer.Instance.GoldSaucerUpdate += GoldSaucerUpdate;
    }

    public override void Dispose()
    {
        AgentGoldSaucer.Instance.GoldSaucerUpdate -= GoldSaucerUpdate;
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

    public override TimeSpan GetTimerPeriod() => TimeSpan.FromDays(4);
    protected override DateTime GetTimerReset() => Time.NextFashionReportReset();
    public override string GetStatusMessage()
    {
        switch(Settings.Mode.Value)
        {
            case FashionReportMode.All:
            case FashionReportMode.Single when Settings.AllowancesRemaining == 4:
                return $"{Settings.AllowancesRemaining} {Strings.FashionReport_AllowancesAvailable}";

            case FashionReportMode.Plus80 when Settings.HighestWeeklyScore <= 80:
                return $"{Settings.HighestWeeklyScore} {Strings.FashionReport_HighestScore}";

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public override void DoReset()
    {
        Settings.AllowancesRemaining = 4;
        Settings.HighestWeeklyScore = 0;
    }

    public override ModuleStatus GetModuleStatus()
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

    private static bool FashionReportAvailable()
    {
        var reportOpen = Time.NextFashionReportReset();
        var reportClosed = Time.NextWeeklyReset();

        var now = DateTime.UtcNow;

        return now > reportOpen && now < reportClosed;
    }

    private static string GetNextFashionReport()
    {
        var span = Time.NextFashionReportReset() - DateTime.UtcNow;

        return span.FormatTimespan(Settings.TimerSettings.TimerStyle.Value);
    }

    protected override void DrawConfiguration()
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

    protected override void DrawStatus()
    {
        InfoBox.Instance.DrawGenericStatus(this);

        InfoBox.Instance
            .AddTitle(Strings.Status_ModuleData)
            .BeginTable()
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
            .AddString(FashionReportAvailable() ? Strings.Common_AvailableNow : GetNextFashionReport(),
                FashionReportAvailable() ? Colors.Green : Colors.Orange)
            .EndRow()
            .EndTable()
            .Draw();
            
        InfoBox.Instance.DrawSuppressionOption(this);
    }
}