using System;
using DailyDuty.DataModels;
using DailyDuty.DataStructures;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.Utilities;
using Dalamud.Utility.Signatures;
using KamiLib.Configuration;
using KamiLib.Drawing;
using KamiLib.Misc;

namespace DailyDuty.Modules;

public class LevequestSettings : GenericSettings
{
    public Setting<int> NotificationThreshold = new(95);
    public Setting<ComparisonMode> ComparisonMode = new(DataModels.ComparisonMode.EqualTo);
}

public unsafe class Levequest : Module
{
    public override ModuleName Name => ModuleName.Levequest;
    public override CompletionType CompletionType => CompletionType.Daily;

    private static LevequestSettings Settings => Service.ConfigurationManager.CharacterConfiguration.Levequest;
    public override GenericSettings GenericSettings => Settings;

    [Signature("88 05 ?? ?? ?? ?? 0F B7 41 06", ScanType = ScanType.StaticAddress)]
    private readonly LevequestStruct* levequestStruct = null;
    
    public Levequest()
    {
        SignatureHelper.Initialise(this);
    }

    public override TimeSpan GetTimerPeriod() => TimeSpan.FromHours(12);
    protected override DateTime GetModuleReset() => Time.NextLeveAllowanceReset();
    public override string GetStatusMessage() => $"{GetRemainingAllowances()} {Strings.Common_AllowancesRemaining}";
    private int GetRemainingAllowances() => levequestStruct->AllowancesRemaining;
    private int GetAcceptedLeves() => levequestStruct->LevesAccepted;

    public override ModuleStatus GetModuleStatus()
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

    private static string GetNextLeviquest()
    {
        var span = Time.NextLeveAllowanceReset() - DateTime.UtcNow;

        return span.FormatTimespan(Settings.TimerSettings.TimerStyle.Value);
    }
    
    protected override void DrawConfiguration()
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

    protected override void DrawStatus()
    {
        InfoBox.Instance.DrawGenericStatus(this);
            
        InfoBox.Instance
            .AddTitle(Strings.Status_ModuleData)
            .BeginTable()
            .BeginRow()
            .AddString(Strings.Common_Allowances)
            .AddString(GetRemainingAllowances().ToString())
            .EndRow()
            .BeginRow()
            .AddString(Strings.Common_Accepted)
            .AddString(GetAcceptedLeves().ToString())
            .EndRow()
            .EndTable()
            .Draw();

        InfoBox.Instance
            .AddTitle(Strings.Levequest_NextAllowance)
            .BeginTable()
            .BeginRow()
            .AddString(Strings.Levequest_NextAllowance)
            .AddString(GetNextLeviquest())
            .EndRow()
            .EndTable()
            .Draw();
            
        InfoBox.Instance.DrawSuppressionOption(this);
    }
}