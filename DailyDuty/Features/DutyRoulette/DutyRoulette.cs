using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.Classes;
using DailyDuty.CustomNodes;
using DailyDuty.Enums;
using DailyDuty.Utilities;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.Sheets;
using Newtonsoft.Json.Linq;
using InstanceContent = FFXIVClientStructs.FFXIV.Client.Game.UI.InstanceContent;

namespace DailyDuty.Features.DutyRoulette;

public unsafe class DutyRoulette : Module<DutyRouletteConfig, DataBase> {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Duty Roulette",
        FileName = "DutyRoulette",
        Type = ModuleType.Daily,
        ChangeLog = [
            new ChangeLogInfo(1, "Initial Re-Implementation"),
        ],
        Tags = [ "Exp", "Gil" ],
    };

    private DutyRouletteDutyFinderController? rouletteController;
    public override DataNodeBase DataNode => new DutyRouletteDataNode(this);
    public override ConfigNodeBase ConfigNode => new DutyRouletteConfigNode(this);

    protected override DutyRouletteConfig MigrateConfig(JObject objectData)
        => DutyRouletteMigration.Migrate(objectData);

    protected override void OnModuleEnable() {
        rouletteController = new DutyRouletteDutyFinderController(this);
    }

    protected override void OnModuleDisable() {
        rouletteController?.Dispose();
        rouletteController = null;
    }

    protected override StatusMessage GetStatusMessage() => new() {
        Message = $"{GetIncompleteCount()} Duty Roulette(s) incomplete",
        PayloadId = PayloadId.OpenDutyFinderRoulette,
    };

    public override DateTime GetNextResetDateTime() 
        => Time.NextDailyReset();

    public override TimeSpan GetResetPeriod()
        => TimeSpan.FromDays(1);

    protected override CompletionStatus GetCompletionStatus() {
        if (ModuleConfig.CompleteWhenCapped && GetLimitedTomestonesCount() == GetLimitedTomestonesLimit()) {
            return  CompletionStatus.Complete;
        }
        
        return GetIncompleteCount() is 0 ? CompletionStatus.Complete : CompletionStatus.Incomplete;
    }

    protected override TodoTooltip GetTooltip() => new() {
        TooltipText = string.Join("\n", GetIncompleteTasks().Select(task => task.Name)),
        ClickAction = PayloadId.OpenDutyFinderRoulette,
    };

    private IEnumerable<ContentRoulette> GetIncompleteTasks()
        => ModuleConfig.TrackedRoulettes
            .Select(id => Services.DataManager.GetExcelSheet<ContentRoulette>().GetRow(id))
            .Where(row => !InstanceContent.Instance()->IsRouletteComplete((byte)row.RowId));

    private int GetIncompleteCount()
        => GetIncompleteTasks().Count();

    private static int GetLimitedTomestonesCount() => InventoryManager.Instance()->GetWeeklyAcquiredTomestoneCount();
    private static int GetLimitedTomestonesLimit() => InventoryManager.GetLimitedTomestoneWeeklyLimit();
}
