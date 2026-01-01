using System;
using DailyDuty.Classes;
using DailyDuty.CustomNodes;
using DailyDuty.Enums;
using DailyDuty.Utilities;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.Object;

namespace DailyDuty.Features.FashionReport;

public unsafe class FashionReport : Module<Config, Data> {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Fashion Report",
        FileName = "FashionReport",
        Type = ModuleType.Special,
        ChangeLog = [
            new ChangeLogInfo(1, "Initial Re-Implementation"),
        ],
        Tags = [ "Gold Saucer", "Gold", "Saucer", "MGP" ],
    };

    public override DataNodeBase DataNode => new DataNode(this);
    public override ConfigNodeBase ConfigNode => new ConfigNode(this);

    protected override StatusMessage GetStatusMessage() => new() {
        Message = ModuleConfig.CompletionMode switch {
            FashionReportMode.All => $"{ModuleData.AllowancesRemaining} Allowances Available",
            FashionReportMode.Single when ModuleData.AllowancesRemaining is 4 => $"{ModuleData.AllowancesRemaining} Allowances Available",
            FashionReportMode.Plus80 when ModuleData.HighestWeeklyScore <= 80 => $"{ModuleData.HighestWeeklyScore} Highest Score",
            _ => string.Empty,
        },
        PayloadId = PayloadId.GoldSaucerTeleport,
    };

    public override DateTime GetNextResetDateTime() 
        => Time.NextFashionReportReset();

    public override TimeSpan GetResetPeriod()
        => TimeSpan.FromDays(7);

    public override void Reset() {
        ModuleData.AllowancesRemaining = 4;
        ModuleData.HighestWeeklyScore = 0;
    }

    protected override CompletionStatus GetCompletionStatus() => ModuleConfig.CompletionMode switch {
        _ when !IsFashionReportAvailable => CompletionStatus.Unavailable,
        FashionReportMode.Single when ModuleData.AllowancesRemaining < 4 => CompletionStatus.Complete,
        FashionReportMode.All when ModuleData.AllowancesRemaining is 0 => CompletionStatus.Complete,
        FashionReportMode.Plus80 when ModuleData.HighestWeeklyScore >= 80 => CompletionStatus.Complete,
        _ => CompletionStatus.Incomplete,
    };

    public override void OnNpcInteract(EventFramework* thisPtr, GameObject* gameObject, EventId eventId, short scene, ulong sceneFlags, uint* sceneData, byte sceneDataCount) {
        if (gameObject->BaseId is not 1025176) return;

        switch (scene) {
            case 1:
                ModuleData.AllowancesRemaining = (int) sceneData[1];
                ModuleData.HighestWeeklyScore = (int) sceneData[0];
                ModuleData.MarkDirty();
                break;
            
            case 2:
                ModuleData.HighestWeeklyScore = Math.Max((int) sceneData[0], ModuleData.HighestWeeklyScore);
                ModuleData.MarkDirty();
                break;
            
            case 5:
                ModuleData.AllowancesRemaining = (int) sceneData[0];
                ModuleData.MarkDirty();
                break;
        }
    }

    private static bool IsFashionReportAvailable 
        => DateTime.UtcNow > Time.NextWeeklyReset().AddDays(-4) && DateTime.UtcNow < Time.NextWeeklyReset();
}
