using System;
using DailyDuty.Classes;
using DailyDuty.Classes.Nodes;
using DailyDuty.Enums;
using DailyDuty.Utilities;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Text.ReadOnly;

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

    public override DataNodeBase GetDataNode() => new DataNode(this);

    public override ConfigNodeBase GetConfigNode() => new ConfigNode(this);

    protected override void OnEnable() { }

    protected override void OnDisable() { }

    protected override ReadOnlySeString GetStatusMessage()
        => $"{RemainingAllowances} Custom Delivery allowances remaining";

    public override DateTime GetNextResetDateTime()
        => Time.NextWeeklyReset();

    public override void Reset() { }

    protected override CompletionStatus GetCompletionStatus() => ModuleConfig.ComparisonMode switch {
        ComparisonMode.Below when ModuleConfig.NotificationThreshold > RemainingAllowances => CompletionStatus.Complete,
        ComparisonMode.Equal when ModuleConfig.NotificationThreshold == RemainingAllowances => CompletionStatus.Complete,
        ComparisonMode.Above when ModuleConfig.NotificationThreshold < RemainingAllowances => CompletionStatus.Complete,
        _ => CompletionStatus.Incomplete,
    };
    
    private static int RemainingAllowances =>  SatisfactionSupplyManager.Instance()->GetRemainingAllowances();
}
