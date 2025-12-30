using System;
using DailyDuty.Classes;
using DailyDuty.Classes.Nodes;
using DailyDuty.Enums;
using DailyDuty.Utilities;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace DailyDuty.Features.TribalQuests;

public unsafe class TribalQuests : Module<Config, DataBase> {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Tribal Quests",
        FileName = "TribalQuests",
        Type = ModuleType.Daily,
        ChangeLog = [
            new ChangeLogInfo(1, "Initial Re-Implementation"),
        ],
        Tags = [ "DoH", "DoL", "Exp" ],
    };

    public override DataNodeBase DataNode => new DataNode(this);
    public override ConfigNodeBase ConfigNode => new ConfigNode(this);

    protected override StatusMessage GetStatusMessage()
        => $"{RemainingAllowances} Quests Remaining";

    public override DateTime GetNextResetDateTime()
        => Time.NextDailyReset();

    public override TimeSpan GetResetPeriod()
        => TimeSpan.FromDays(1);

    protected override CompletionStatus GetCompletionStatus() => ModuleConfig.ComparisonMode switch {
        ComparisonMode.Below when ModuleConfig.NotificationThreshold < RemainingAllowances => CompletionStatus.Complete,
        ComparisonMode.Equal when ModuleConfig.NotificationThreshold == RemainingAllowances => CompletionStatus.Complete,
        ComparisonMode.Above when ModuleConfig.NotificationThreshold > RemainingAllowances => CompletionStatus.Complete,
        _ => CompletionStatus.Incomplete,
    };

    private static int RemainingAllowances => (int) QuestManager.Instance()->GetBeastTribeAllowance();
}
