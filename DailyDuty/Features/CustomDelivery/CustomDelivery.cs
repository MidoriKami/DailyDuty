using DailyDuty.Utilities;
using System;
using DailyDuty.Classes;
using DailyDuty.CustomNodes;
using DailyDuty.Enums;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace DailyDuty.Features.CustomDelivery;

public unsafe class CustomDelivery : Module<CustomDeliveryConfig, DataBase> {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = Strings.CustomDelivery_DisplayName,
        FileName = "CustomDelivery",
        Type = ModuleType.Weekly,
        Tags = ["DoH", "DoL", "Exp"],
    };

    public override DataNodeBase DataNode => new CustomDeliveryDataNode(this);
    public override ConfigNodeBase ConfigNode => new CustomDeliveryConfigNode(this);

    protected override StatusMessage GetStatusMessage()
        => $"{RemainingAllowances - ModuleConfig.NotificationThreshold} {Strings.StatusMessages_CustomDeliveryIncomplete}";

    public override DateTime GetNextResetDateTime()
        => Time.NextWeeklyReset();

    public override TimeSpan GetResetPeriod()
        => TimeSpan.FromDays(7);

    protected override CompletionStatus GetCompletionStatus() => ModuleConfig.ComparisonMode switch {
        ComparisonMode.Above => RemainingAllowances > ModuleConfig.NotificationThreshold ? CompletionStatus.Incomplete : CompletionStatus.Complete,
        ComparisonMode.Below => RemainingAllowances < ModuleConfig.NotificationThreshold ? CompletionStatus.Incomplete : CompletionStatus.Complete,
        ComparisonMode.Equal => RemainingAllowances != ModuleConfig.NotificationThreshold ? CompletionStatus.Incomplete : CompletionStatus.Complete,
        _ => CompletionStatus.Unknown,
    };

    private static int RemainingAllowances => SatisfactionSupplyManager.Instance()->GetRemainingAllowances();
}
