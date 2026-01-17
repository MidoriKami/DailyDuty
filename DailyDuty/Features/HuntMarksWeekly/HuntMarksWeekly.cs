using System;
using DailyDuty.Classes;
using DailyDuty.CustomNodes;
using DailyDuty.Enums;
using DailyDuty.Utilities;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Newtonsoft.Json.Linq;

namespace DailyDuty.Features.HuntMarksWeekly;

public unsafe class HuntMarksWeekly : Module<HuntMarksWeeklyConfig, DataBase> {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Hunt Marks Weekly",
        FileName = "HuntMarksWeekly",
        Type = ModuleType.Weekly,
        ChangeLog = [
            new ChangeLogInfo(1, "Initial Re-Implementation"),
        ],
        Tags = [ "Teleport", "Tickets" ],
    };

    public override ConfigNodeBase ConfigNode => new HuntMarksWeeklyConfigNode(this);
    public override DataNodeBase DataNode => new HuntMarksWeeklyDataNode(this);

    protected override HuntMarksWeeklyConfig MigrateConfig(JObject objectData)
        => HuntMarksWeeklyMigration.Migrate(objectData);
    
    protected override StatusMessage GetStatusMessage()
        => $"{GetIncompleteCount()} Hunt Bills Available";

    public override DateTime GetNextResetDateTime()
        => Time.NextWeeklyReset();

    public override TimeSpan GetResetPeriod()
        => TimeSpan.FromDays(7);

    protected override CompletionStatus GetCompletionStatus()
        => GetIncompleteCount() is 0 ? CompletionStatus.Complete : CompletionStatus.Incomplete;

    private int GetIncompleteCount() {
        var incomplete = 0;

        foreach (byte orderType in ModuleConfig.TrackedHuntMarks) {
            if (!MobHunt.Instance()->IsBillComplete(orderType)) {
                incomplete++;
            }
        }

        return incomplete;
    }
}
