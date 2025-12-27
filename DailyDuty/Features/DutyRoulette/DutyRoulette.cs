using System;
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

    private DutyFinderRouleteController? rouletteController;
    
    public override DataNodeBase GetDataNode() 
        => new DataNode(this);

    public override ConfigNodeBase GetConfigNode() 
        => new ConfigNode(this);

    protected override void OnEnable() {
        rouletteController = new DutyFinderRouleteController(this);
    }

    protected override void OnDisable() {
        rouletteController?.Dispose();
        rouletteController = null;
    }

    protected override ReadOnlySeString GetStatusMessage() 
        => $"{GetIncompleteCount()} Duty Roulettes incomplete";

    public override DateTime GetNextResetDateTime() 
        => Time.NextDailyReset();

    public override void Reset() { }

    protected override CompletionStatus GetCompletionStatus() {
        if (ModuleConfig.CompleteWhenCapped && GetLimitedTomestonesCount() == GetLimitedTomestonesLimit()) {
            return  CompletionStatus.Complete;
        }
        
        return GetIncompleteCount() is 0 ? CompletionStatus.Complete : CompletionStatus.Incomplete;
    }

    private int GetIncompleteCount()
        => ModuleConfig.TrackedRoulettes.Select(id => Services.DataManager.GetExcelSheet<ContentRoulette>().GetRow(id))
            .Count(row => !InstanceContent.Instance()->IsRouletteComplete((byte)row.RowId));

    private static int GetLimitedTomestonesCount() => InventoryManager.Instance()->GetWeeklyAcquiredTomestoneCount();
    private static int GetLimitedTomestonesLimit() => InventoryManager.GetLimitedTomestoneWeeklyLimit();
}
