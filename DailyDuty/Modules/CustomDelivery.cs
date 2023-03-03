using System;
using DailyDuty.DataModels;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.Utilities;
using FFXIVClientStructs.FFXIV.Client.Game;
using KamiLib.Configuration;
using KamiLib.Drawing;

namespace DailyDuty.Modules;

public class CustomDeliverySettings : GenericSettings
{
    public Setting<int> NotificationThreshold = new(12);
    public Setting<ComparisonMode> ComparisonMode = new(DataModels.ComparisonMode.LessThan);
}

public unsafe class CustomDelivery : Module
{
    public override ModuleName Name => ModuleName.CustomDelivery;
    public override CompletionType CompletionType => CompletionType.Weekly;
    
    private static CustomDeliverySettings Settings => Service.ConfigurationManager.CharacterConfiguration.CustomDelivery;
    public override GenericSettings GenericSettings => Settings;
    
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
    
    private int GetRemainingAllowances() => SatisfactionSupplyManager.Instance()->GetRemainingAllowances();
    
    public override string GetStatusMessage() => $"{GetRemainingAllowances()} {Strings.Common_AllowancesRemaining}";

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
            .AddTitle(Strings.Config_Label)
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