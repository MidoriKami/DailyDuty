using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.Classes;
using DailyDuty.Classes.Nodes;
using DailyDuty.Enums;
using Dalamud.Game.ClientState.Conditions;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.Sheets;

namespace DailyDuty.Features.TreasureMap;

public unsafe class TreasureMap : Module<ConfigBase, Data> {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Treasure Map",
        FileName = "TreasureMap",
        Type = ModuleType.Special,
        ChangeLog = [
            new ChangeLogInfo(1, "Initial Re-Implementation"),
        ],
        Tags = [ "DoH", "DoL", "Exp" ],
    };

    public override DataNodeBase DataNode => new DataNode(this);
    private List<uint> inventoryMaps = [];
    private bool gatheringStarted;

    protected override StatusMessage GetStatusMessage()
        => "Map Available";

    public override DateTime GetNextResetDateTime() {
        if (ModuleData.LastMapGatheredTime == DateTime.MinValue) return DateTime.MaxValue;
        if (DateTime.UtcNow > ModuleData.LastMapGatheredTime + TimeSpan.FromHours(18)) return DateTime.MaxValue;

        return ModuleData.LastMapGatheredTime + TimeSpan.FromHours(18);
    }

    public override void Reset()
        => ModuleData.NextReset = DateTime.MaxValue;

    public override TimeSpan GetResetPeriod()
        => TimeSpan.FromHours(18);

    protected override CompletionStatus GetCompletionStatus()
        => ModuleData.NextReset == DateTime.MaxValue ? CompletionStatus.Incomplete : CompletionStatus.Complete;

    protected override void Update() {
        base.Update();
        
        if (Services.Condition[ConditionFlag.ExecutingGatheringAction] && !gatheringStarted) {
            gatheringStarted = true;
            OnGatheringStart();
        } 
        else if (!Services.Condition[ConditionFlag.ExecutingGatheringAction] && gatheringStarted) {
            gatheringStarted = false;
            OnGatheringStop();
        }
    }
        
    private void OnGatheringStart() {
        inventoryMaps.Clear();
        inventoryMaps = GetInventoryTreasureMaps().ToList();
    }

    private void OnGatheringStop() {
    	var newInventoryMaps = GetInventoryTreasureMaps().ToList();

    	if (newInventoryMaps.Count > inventoryMaps.Count) {
            ModuleData.LastMapGatheredTime = DateTime.UtcNow;
            ModuleData.NextReset = ModuleData.LastMapGatheredTime + TimeSpan.FromHours(18);
            ModuleConfig.Suppressed = false;

            ModuleData.MarkDirty();
            ModuleConfig.MarkDirty();
    	}
    }

    private static IEnumerable<uint> GetInventoryTreasureMaps() {
        foreach (var treasureMap in Services.DataManager.GetExcelSheet<TreasureHuntRank>().Where(map => map.ItemName.RowId is not 0)) {
            if (IsItemInInventory(treasureMap.ItemName.RowId)) {
                yield return treasureMap.ItemName.RowId;
            }
        }
    }
    
    private static bool IsItemInInventory(uint itemId)
        => InventoryManager.Instance()->GetInventoryItemCount(itemId) > 0;
}
