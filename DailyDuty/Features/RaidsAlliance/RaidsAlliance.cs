using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.Classes;
using DailyDuty.CustomNodes;
using DailyDuty.Enums;
using DailyDuty.Utilities;
using Dalamud.Game.Inventory;
using Dalamud.Game.Inventory.InventoryEventArgTypes;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.Sheets;

namespace DailyDuty.Features.RaidsAlliance;

public unsafe class RaidsAlliance : Module<Config, Data> {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Raids Alliance",
        FileName = "RaidsAlliance",
        Type = ModuleType.Weekly,
        ChangeLog = [
            new ChangeLogInfo(1, "Initial Re-Implementation"),
        ],
        Tags = [ "Tomestones", "Raids", "Exp", "Hardcore" ],
    };

    private List<uint>? validAllianceRaids;

    public override DataNodeBase DataNode => new DataNode(this);
    public override ConfigNodeBase ConfigNode => new ConfigNode(this);

    protected override void OnModuleEnable() {
        Services.GameInventory.ItemAdded += OnItemEvent;
        Services.GameInventory.ItemChanged += OnItemEvent;

        validAllianceRaids = Services.DataManager.LimitedAllianceRaidDuties.Select(cfc => cfc.RowId).ToList();
        
        UpdateTrackedTasks();
    }

    protected override void OnModuleDisable() {
        Services.GameInventory.ItemAdded -= OnItemEvent;
        Services.GameInventory.ItemChanged -= OnItemEvent;

        validAllianceRaids?.Clear();
        validAllianceRaids = null;
    }

    protected override StatusMessage GetStatusMessage() => new() {
        Message = $"{GetIncompleteCount()} Alliance Raid Available",
        PayloadId = PayloadId.OpenDutyFinderRaid,
    };

    public override DateTime GetNextResetDateTime()
        => Time.NextWeeklyReset();

    public override TimeSpan GetResetPeriod()
        => TimeSpan.FromDays(7);

    protected override CompletionStatus GetCompletionStatus()
        => GetIncompleteCount() is 0 ? CompletionStatus.Complete : CompletionStatus.Incomplete;

    public override void Reset() {
        foreach (var raidKey in ModuleData.TaskStatus.Keys) {
            ModuleData.TaskStatus[raidKey] = false;
        }
    }

    private int GetIncompleteCount() {
        var count = 0;

        foreach (var (cfc, raidStatus) in ModuleConfig.TrackedTasks.Where(pair => pair.Value)) {
            if (!ModuleData.TaskStatus.TryGetValue(cfc, out var value)) continue;

            if (raidStatus != value) {
                count++;
            }
        }
        
        return count;
    }

    private void OnItemEvent(GameInventoryEvent type, InventoryEventArgs data) {
    	// If the item event is not for main inventory, we don't care.
    	if (data.Item.ContainerType is not (GameInventoryType.Inventory1 or GameInventoryType.Inventory2 or GameInventoryType.Inventory3 or GameInventoryType.Inventory4)) return;

        var currentDuty = GameMain.Instance()->CurrentContentFinderConditionId;
        
        // If we are not in a tracked zone, return
        if (!ModuleConfig.TrackedTasks.ContainsKey(currentDuty)) return;
 
    	// If we can't get the exd data for this item, return
    	var item = Services.DataManager.GetExcelSheet<Item>().GetRow(data.Item.ItemId);
    	if (item.RowId is 0) return;
    	
    	Services.PluginLog.Debug($"InventoryEvent: {type}: {item.Name}");
 
    	// If the item is a limited type that we care about, mark as completed
    	switch (item.ItemUICategory.RowId) {
    		case 34: // Head
    		case 35: // Body
    		case 36: // Legs
    		case 37: // Hands
    		case 38: // Feet
    		case 61 when item.ItemAction.RowId == 0: // Miscellany with no itemAction
                ModuleData.TaskStatus[currentDuty] = true;
                ModuleData.MarkDirty();
    			break;
    	}
    }

    private void UpdateTrackedTasks() {
        if (validAllianceRaids is null) return;
        
        // Remove any tasks that are now invalid.
        foreach (var key in ModuleConfig.TrackedTasks.Keys.ToList()) {
            if (!validAllianceRaids.Contains(key)) {
                ModuleConfig.TrackedTasks.Remove(key);
            }
        }

        // Add new blank entries for valid tasks.
        foreach (var validRaid in validAllianceRaids) {
            ModuleConfig.TrackedTasks.TryAdd(validRaid, false);
        }
        ModuleConfig.MarkDirty();

        // Forget data for any tasks that are now invalid.
        foreach (var key in ModuleData.TaskStatus.Keys.ToList()) {
            if (!validAllianceRaids.Contains(key)) {
                ModuleData.TaskStatus.Remove(key);
            }
        }
        
        // Add new blank entries for valid tasks.
        foreach (var validRaid in validAllianceRaids) {
            ModuleData.TaskStatus.TryAdd(validRaid, false);
        }
        ModuleData.MarkDirty();
    }
}
