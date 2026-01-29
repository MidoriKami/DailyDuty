using System;
using System.Linq;
using DailyDuty.Classes;
using DailyDuty.CustomNodes;
using DailyDuty.Enums;
using DailyDuty.Utilities;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using Newtonsoft.Json.Linq;

namespace DailyDuty.Features.HuntMarksDaily;

public unsafe class HuntMarksDaily : Module<HuntMarksDailyConfig, DataBase> {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Hunt Marks Daily",
        FileName = "HuntMarksDaily",
        Type = ModuleType.Daily,
        ChangeLog = [
            new ChangeLogInfo(1, "Initial Re-Implementation"),
        ],
        Tags = [ "Teleport", "Tickets" ],
    };

    public override ConfigNodeBase ConfigNode => new HuntMarksDailyConfigNode(this);
    public override DataNodeBase DataNode => new HuntMarksDailyDataNode(this);

    protected override HuntMarksDailyConfig MigrateConfig(JObject objectData)
        => HuntMarksDailyMigration.Migrate(objectData);

    protected override StatusMessage GetStatusMessage()
        => $"{GetIncompleteCount()} Hunt Bills Available";

    public override DateTime GetNextResetDateTime()
        => Time.NextDailyReset();

    public override TimeSpan GetResetPeriod()
        => TimeSpan.FromDays(1);

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
    
    protected override TodoTooltip GetTooltip() => new() {
        TooltipText = GetMissingObjectives(),
        ClickAction = PayloadId.OpenChallengeLog,
    };

    private ReadOnlySeString GetMissingObjectives() {
        var result = string.Empty;
        
        foreach (var warningId in ModuleConfig.TrackedHuntMarks.Where(huntBill => !MobHunt.Instance()->IsBillComplete((byte) huntBill))) {
            if (!Services.DataManager.GetExcelSheet<MobHuntOrderType>().TryGetRow(warningId, out var contentNote)) continue;
            
            result += contentNote.EventItem.ValueNullable?.Name.ToString() + "\n";
        }

        result = result.Trim('\n');
        
        return result;
    }
}
