using System;
using DailyDuty.Classes;
using DailyDuty.CustomNodes;
using DailyDuty.Enums;
using DailyDuty.Utilities;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace DailyDuty.Features.CustomDelivery;

public unsafe class CustomDelivery : Module<CustomDeliveryConfig, DataBase> {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Custom Delivery",
        FileName = "CustomDelivery",
        Type = ModuleType.Weekly,
        ChangeLog = [
            new ChangeLogInfo(1, "Initial Re-Implementation"),
        ],
        Tags = [ "DoH", "DoL", "Exp" ],
    };

    public override DataNodeBase DataNode => new CustomDeliveryDataNode(this);
    public override ConfigNodeBase ConfigNode => new CustomDeliveryConfigNode(this);
    
    protected override StatusMessage GetStatusMessage()
        => $"{RemainingAllowances - ModuleConfig.NotificationThreshold} Custom Deliveries Available";

    public override DateTime GetNextResetDateTime()
        => Time.NextWeeklyReset();

    public override TimeSpan GetResetPeriod()
        => TimeSpan.FromDays(7);

    protected override CompletionStatus GetCompletionStatus() => ModuleConfig.ComparisonMode switch {
        ComparisonMode.Below when ModuleConfig.NotificationThreshold < RemainingAllowances => CompletionStatus.Complete,
        ComparisonMode.Equal when ModuleConfig.NotificationThreshold == RemainingAllowances => CompletionStatus.Complete,
        ComparisonMode.Above when ModuleConfig.NotificationThreshold > RemainingAllowances => CompletionStatus.Complete,
        _ => CompletionStatus.Incomplete,
    };
    
    private static int RemainingAllowances => SatisfactionSupplyManager.Instance()->GetRemainingAllowances();
}
