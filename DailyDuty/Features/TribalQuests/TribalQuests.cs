using DailyDuty.Utilities;
using System;
using DailyDuty.Classes;
using DailyDuty.CustomNodes;
using DailyDuty.Enums;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace DailyDuty.Features.TribalQuests;

public unsafe class TribalQuests : Module<TribalQuestsConfig, DataBase> {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = Strings.TribalQuests_DisplayName,
        FileName = "TribalQuests",
        Type = ModuleType.Daily,
        Tags = ["DoH", "DoL", "Exp"],
    };

    public override DataNodeBase DataNode => new TribalQuestsDataNode(this);
    public override ConfigNodeBase ConfigNode => new TribalQuestsConfigNode(this);

    protected override StatusMessage GetStatusMessage()
        => $"{RemainingAllowances} {Strings.StatusMessages_QuestIncomplete}";

    public override DateTime GetNextResetDateTime()
        => Time.NextDailyReset();

    public override TimeSpan GetResetPeriod()
        => TimeSpan.FromDays(1);

    protected override CompletionStatus GetCompletionStatus() => ModuleConfig.ComparisonMode switch {
        ComparisonMode.Above => RemainingAllowances > ModuleConfig.NotificationThreshold ? CompletionStatus.Incomplete : CompletionStatus.Complete,
        ComparisonMode.Below => RemainingAllowances < ModuleConfig.NotificationThreshold ? CompletionStatus.Incomplete : CompletionStatus.Complete,
        ComparisonMode.Equal => RemainingAllowances != ModuleConfig.NotificationThreshold ? CompletionStatus.Incomplete : CompletionStatus.Complete,
        _ => CompletionStatus.Unknown,
    };

    private static int RemainingAllowances => (int)QuestManager.Instance()->GetBeastTribeAllowance();
}
