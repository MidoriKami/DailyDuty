using DailyDuty.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DailyDuty.Classes;
using DailyDuty.CustomNodes;
using DailyDuty.Enums;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.Sheets;
using Newtonsoft.Json.Linq;
using InstanceContent = FFXIVClientStructs.FFXIV.Client.Game.UI.InstanceContent;

namespace DailyDuty.Features.DutyRoulette;

public class DutyRoulette : Module<DutyRouletteConfig, DataBase> {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = Strings.DutyRoulette_DisplayName,
        FileName = "DutyRoulette",
        Type = ModuleType.Daily,
        Tags = ["Exp", "Gil"],
    };

    private DutyRouletteDutyFinderController? rouletteController;
    public override DataNodeBase DataNode => new DutyRouletteDataNode(this);
    public override ConfigNodeBase ConfigNode => new DutyRouletteConfigNode(this);

    protected override DutyRouletteConfig MigrateConfig(JObject objectData)
        => DutyRouletteMigration.Migrate(objectData);

    protected override async Task OnModuleEnable() {
        await Services.Framework.RunSafely(() => {
            rouletteController = new DutyRouletteDutyFinderController(this);
        });
    }

    protected override async Task OnModuleDisable() {
        await Services.Framework.RunSafely(() => {
            rouletteController?.Dispose();
        });

        rouletteController = null;
    }

    protected override StatusMessage GetStatusMessage() => new() {
        Message = $"{GetIncompleteCount()} {Strings.StatusMessages_RouletteIncomplete}",
        PayloadId = PayloadId.OpenDutyFinderRoulette,
    };

    public override DateTime GetNextResetDateTime()
        => Time.NextDailyReset();

    public override TimeSpan GetResetPeriod()
        => TimeSpan.FromDays(1);

    protected override CompletionStatus GetCompletionStatus() {
        if (ModuleConfig.CompleteWhenCapped && GetLimitedTomestonesCount() == GetLimitedTomestonesLimit()) {
            return CompletionStatus.Complete;
        }

        return GetIncompleteCount() is 0 ? CompletionStatus.Complete : CompletionStatus.Incomplete;
    }

    protected override TodoTooltip GetTooltip() => new() {
        TooltipText = string.Join("\n", GetIncompleteTasks().Select(task => task.Name)),
        ClickAction = PayloadId.OpenDutyFinderRoulette,
    };

    private unsafe IEnumerable<ContentRoulette> GetIncompleteTasks()
        => ModuleConfig.TrackedRoulettes
            .Select(id => Services.DataManager.GetExcelSheet<ContentRoulette>().GetRow(id))
            .Where(row => !InstanceContent.Instance()->IsRouletteComplete((byte)row.RowId));

    private int GetIncompleteCount()
        => GetIncompleteTasks().Count();

    private static unsafe int GetLimitedTomestonesCount() => InventoryManager.Instance()->GetWeeklyAcquiredTomestoneCount();
    private static int GetLimitedTomestonesLimit() => InventoryManager.GetLimitedTomestoneWeeklyLimit();
}
