using DailyDuty.Abstracts;
using DailyDuty.Models;
using DailyDuty.Models.Enums;
using DailyDuty.System.Localization;
using FFXIVClientStructs.FFXIV.Client.Game;
using KamiLib.AutomaticUserInterface;

namespace DailyDuty.System;

public class CustomDeliveryConfig : ModuleConfigBase
{
    [IntConfigOption("NotificationThreshold", "ModuleConfiguration", 1, 0, 12)]
    public int NotificationThreshold = 12;

    [EnumConfigOption("ComparisonMode", "ModuleConfiguration", 1, "ComparisonHelp")]
    public ComparisonMode ComparisonMode = ComparisonMode.LessThan;
}

public class CustomDeliveryData : ModuleDataBase
{
    [IntDisplay("AllowancesRemaining", "ModuleData", 1)]
    public int RemainingAllowances = 12;
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
        TryUpdateData(ref Data.RemainingAllowances, SatisfactionSupplyManager.Instance()->GetRemainingAllowances());
        
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