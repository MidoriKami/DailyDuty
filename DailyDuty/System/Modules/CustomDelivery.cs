using DailyDuty.Abstracts;
using DailyDuty.Models;
using DailyDuty.Models.Attributes;
using DailyDuty.Models.Enums;
using DailyDuty.System.Localization;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace DailyDuty.System;

public class CustomDeliveryConfig : ModuleConfigBase
{
    [ConfigOption("NotificationThreshold", 0, 100)]
    public int NotificationThreshold = 12;

    [ConfigOption("ComparisonMode", "ComparisonHelp")]
    public ComparisonMode ComparisonMode = ComparisonMode.LessThan;
}

public class CustomDeliveryData : ModuleDataBase
{
    [DataDisplay("RemainingAllowances")]
    public int RemainingAllowances;
}

public unsafe class CustomDelivery : Module.WeeklyModule
{
    public override ModuleName ModuleName => ModuleName.CustomDelivery;

    public override ModuleDataBase ModuleData { get; protected set; } = new CustomDeliveryData();
    public override ModuleConfigBase ModuleConfig { get; protected set; } = new CustomDeliveryConfig();
    
    private CustomDeliveryConfig Config => ModuleConfig as CustomDeliveryConfig ?? new CustomDeliveryConfig();
    private CustomDeliveryData Data => ModuleData as CustomDeliveryData ?? new CustomDeliveryData();

    public override void Update()
    {
        var taskState = GetRemainingAllowances();

        if (Data.RemainingAllowances != taskState)
        {
            Data.RemainingAllowances = taskState;
            SaveData();
        }
    }

    public override void Reset()
    {
        Data.RemainingAllowances = 0;
        
        base.Reset();
    }

    protected override ModuleStatus GetModuleStatus()
    {
        switch (Config.ComparisonMode)
        {
            case ComparisonMode.LessThan when Config.NotificationThreshold > GetRemainingAllowances():
            case ComparisonMode.EqualTo when Config.NotificationThreshold == GetRemainingAllowances():
            case ComparisonMode.LessThanOrEqual when Config.NotificationThreshold >= GetRemainingAllowances():
                return ModuleStatus.Complete;

            default:
                return ModuleStatus.Incomplete;
        }
    }

    protected override StatusMessage GetStatusMessage() => new()
    {
        Message = $"{GetRemainingAllowances()} {Strings.AllowancesRemaining}",
    };

    private int GetRemainingAllowances() => SatisfactionSupplyManager.Instance()->GetRemainingAllowances();
}