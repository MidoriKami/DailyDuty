using DailyDuty.Abstracts;
using DailyDuty.Models;
using DailyDuty.Models.Enums;
using DailyDuty.Models.ModuleData;
using DailyDuty.System.Localization;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace DailyDuty.System;

public unsafe class CustomDelivery : Module.WeeklyModule
{
    public override ModuleName ModuleName => ModuleName.CustomDelivery;

    public override IModuleDataBase ModuleData { get; protected set; } = new CustomDeliveryData();
    public override IModuleConfigBase ModuleConfig { get; protected set; } = new CustomDeliveryConfig();
    
    private CustomDeliveryConfig Config => ModuleConfig as CustomDeliveryConfig ?? new CustomDeliveryConfig();
    private CustomDeliveryData Data => ModuleData as CustomDeliveryData ?? new CustomDeliveryData();

    public override void Update()
    {
        Data.RemainingAllowances = TryUpdateData(Data.RemainingAllowances, SatisfactionSupplyManager.Instance()->GetRemainingAllowances());
        
        base.Update();
    }

    public override void Reset()
    {
        Data.RemainingAllowances = 12;
        
        base.Reset();
    }

    protected override ModuleStatus GetModuleStatus() => Config.ComparisonMode switch
    {
        ComparisonMode.LessThan when Config.NotificationThreshold > Data.RemainingAllowances => ModuleStatus.Complete,
        ComparisonMode.EqualTo when Config.NotificationThreshold == Data.RemainingAllowances => ModuleStatus.Complete,
        ComparisonMode.LessThanOrEqual when Config.NotificationThreshold >= Data.RemainingAllowances => ModuleStatus.Complete,
        _ => ModuleStatus.Incomplete
    };

    protected override StatusMessage GetStatusMessage() => new()
    {
        Message = $"{Data.RemainingAllowances} {Strings.AllowancesRemaining}",
    };
}