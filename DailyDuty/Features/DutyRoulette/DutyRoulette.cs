using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.Classes;
using DailyDuty.Classes.Nodes;
using DailyDuty.Enums;
using DailyDuty.Utilities;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
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
        MessageClickAction = PayloadId.OpenDutyFinderRoulette, 
    };

    private DutyFinderRouletteController? rouletteController;
    public override DataNodeBase DataNode => new DataNode(this);
    public override ConfigNodeBase ConfigNode => new ConfigNode(this);

    protected override void OnEnable() {
        rouletteController = new DutyFinderRouletteController(this);
    }

    protected override void OnDisable() {
        rouletteController?.Dispose();
        rouletteController = null;
    }

    protected override ReadOnlySeString GetStatusMessage() 
        => $"{GetIncompleteCount()} Duty Roulette(s) incomplete";

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

    public override ReadOnlySeString? GetTooltip()
        => string.Join("\n", GetIncompleteTasks().Select(task => task.Name));

    private IEnumerable<ContentRoulette> GetIncompleteTasks()
        => ModuleConfig.TrackedRoulettes
            .Select(id => Services.DataManager.GetExcelSheet<ContentRoulette>().GetRow(id))
            .Where(row => !InstanceContent.Instance()->IsRouletteComplete((byte)row.RowId));

    private int GetIncompleteCount()
        => GetIncompleteTasks().Count();

    private static int GetLimitedTomestonesCount() => InventoryManager.Instance()->GetWeeklyAcquiredTomestoneCount();
    private static int GetLimitedTomestonesLimit() => InventoryManager.GetLimitedTomestoneWeeklyLimit();
}
