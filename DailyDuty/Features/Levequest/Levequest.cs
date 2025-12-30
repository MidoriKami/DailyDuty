using System;
using DailyDuty.Classes;
using DailyDuty.Classes.Nodes;
using DailyDuty.Enums;
using DailyDuty.Utilities;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace DailyDuty.Features.Levequest;

public unsafe class Levequest : Module<Config, DataBase> {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Levequest",
        FileName = "Levequest",
        Type = ModuleType.Special,
        ChangeLog = [
            new ChangeLogInfo(1, "Initial Re-Implementation"),
        ],
        Tags = [ "DoH", "DoL", "Exp" ],
    };

    public override DataNodeBase DataNode => new DataNode(this);
    public override ConfigNodeBase ConfigNode => new ConfigNode(this);

    protected override StatusMessage GetStatusMessage()
        => $"{RemainingAllowances - ModuleConfig.NotificationThreshold} Allowances Remaining";

    public override DateTime GetNextResetDateTime()
        => Time.NextLeveAllowanceReset();

    public override TimeSpan GetResetPeriod()
        => TimeSpan.FromHours(12);

    protected override CompletionStatus GetCompletionStatus() => ModuleConfig.ComparisonMode switch {
        ComparisonMode.Below when ModuleConfig.NotificationThreshold < RemainingAllowances => CompletionStatus.Complete,
        ComparisonMode.Equal when ModuleConfig.NotificationThreshold == RemainingAllowances => CompletionStatus.Complete,
        ComparisonMode.Above when ModuleConfig.NotificationThreshold > RemainingAllowances => CompletionStatus.Complete,
        _ => CompletionStatus.Incomplete,
    };

    private static int RemainingAllowances => QuestManager.Instance()->NumLeveAllowances;
}
