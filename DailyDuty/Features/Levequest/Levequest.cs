using System;
using DailyDuty.Classes;
using DailyDuty.CustomNodes;
using DailyDuty.Enums;
using DailyDuty.Utilities;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace DailyDuty.Features.Levequest;

public unsafe class Levequest : Module<LevequestConfig, DataBase> {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Levequest",
        FileName = "Levequest",
        Type = ModuleType.Special,
        ChangeLog = [
            new ChangeLogInfo(1, "Initial Re-Implementation"),
        ],
        Tags = [ "DoH", "DoL", "Exp" ],
    };

    public override DataNodeBase DataNode => new LevequestDataNode(this);
    public override ConfigNodeBase ConfigNode => new LevequestConfigNode(this);

    protected override StatusMessage GetStatusMessage()
        => $"{RemainingAllowances - ModuleConfig.NotificationThreshold} Allowances Remaining";

    public override DateTime GetNextResetDateTime()
        => Time.NextLeveAllowanceReset();

    public override TimeSpan GetResetPeriod()
        => TimeSpan.FromHours(12);

    protected override CompletionStatus GetCompletionStatus() => ModuleConfig.ComparisonMode switch {
        ComparisonMode.Above => RemainingAllowances > ModuleConfig.NotificationThreshold ? CompletionStatus.Incomplete : CompletionStatus.Complete,
        ComparisonMode.Below => RemainingAllowances < ModuleConfig.NotificationThreshold ? CompletionStatus.Incomplete : CompletionStatus.Complete,
        ComparisonMode.Equal => RemainingAllowances != ModuleConfig.NotificationThreshold ? CompletionStatus.Incomplete : CompletionStatus.Complete,
        _ => CompletionStatus.Unknown,
    };

    private static int RemainingAllowances => QuestManager.Instance()->NumLeveAllowances;
}
