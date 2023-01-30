using DailyDuty.Interfaces;
using System;
using DailyDuty.DataModels;
using DailyDuty.Localization;
using DailyDuty.Utilities;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using KamiLib.Configuration;
using KamiLib.Drawing;

namespace DailyDuty.Modules;

public class BeastTribeSettings : GenericSettings
{
    public Setting<int> NotificationThreshold = new(12);
    public Setting<ComparisonMode> ComparisonMode = new(DataModels.ComparisonMode.EqualTo);
}

public class TribalQuests : AbstractModule
{
    public override ModuleName Name => ModuleName.BeastTribe;
    public override CompletionType CompletionType => CompletionType.Daily;

    private static BeastTribeSettings Settings => Service.ConfigurationManager.CharacterConfiguration.BeastTribe;
    public override GenericSettings GenericSettings => Settings;

    private static int GetRemainingAllowances() => (int)PlayerState.GetBeastTribeAllowance();
    public override string GetStatusMessage() => $"{GetRemainingAllowances()} {Strings.Common_AllowancesRemaining}";

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

    protected override void DrawConfiguration()
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

    protected override void DrawStatus()
    {

        var moduleStatus = GetModuleStatus();
        var allowances = GetRemainingAllowances();

        InfoBox.Instance.DrawGenericStatus(this);
            
        InfoBox.Instance
            .AddTitle(Strings.Status_ModuleData)
            .BeginTable()
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