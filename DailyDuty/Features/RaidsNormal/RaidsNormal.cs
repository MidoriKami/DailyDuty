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
using Newtonsoft.Json.Linq;

namespace DailyDuty.Features.RaidsNormal;

public unsafe class RaidsNormal : Module<Config, Data> {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Raids Normal",
        FileName = "RaidsNormal",
        Type = ModuleType.Weekly,
        ChangeLog = [
            new ChangeLogInfo(1, "Initial Re-Implementation"),
        ],
        Tags = [ "Tomestones", "Raids", "Exp", "Hardcore" ],
    };

    private List<uint>? validNormalRaids;

    public override DataNodeBase DataNode => new DataNode(this);
    public override ConfigNodeBase ConfigNode => new ConfigNode(this);

    protected override Config MigrateConfig(JObject objectData)
        => Migration.Migrate(objectData);
    
    protected override void OnModuleEnable() {
        Services.GameInventory.ItemAdded += OnItemEvent;
        Services.GameInventory.ItemChanged += OnItemEvent;

        validNormalRaids = Services.DataManager.LimitedNormalRaidDuties.Select(cfc => cfc.RowId).ToList();
        
        UpdateTrackedTasks();
    }

    protected override void OnModuleDisable() {
        Services.GameInventory.ItemAdded -= OnItemEvent;
        Services.GameInventory.ItemChanged -= OnItemEvent;

        validNormalRaids?.Clear();
        validNormalRaids = null;
    }

    protected override StatusMessage GetStatusMessage() => new() {
        Message = $"{GetIncompleteCount()} Limited Normal Raids Available",
        PayloadId = PayloadId.OpenDutyFinderRaid,
    };

    protected override TodoTooltip GetTooltip() => new() {
        TooltipText = string.Join("\n", GetIncompleteTasks()),
        ClickAction = PayloadId.OpenDutyFinderRaid,
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
        if (validNormalRaids is null) return;
        
        // Remove any tasks that are now invalid.
        foreach (var key in ModuleConfig.TrackedTasks.Keys.ToList()) {
            if (!validNormalRaids.Contains(key)) {
                ModuleConfig.TrackedTasks.Remove(key);
            }
        }

        // Add new blank entries for valid tasks.
        foreach (var validRaid in validNormalRaids) {
            ModuleConfig.TrackedTasks.TryAdd(validRaid, false);
        }
        ModuleConfig.MarkDirty();

        // Forget data for any tasks that are now invalid.
        foreach (var key in ModuleData.TaskStatus.Keys.ToList()) {
            if (!validNormalRaids.Contains(key)) {
                ModuleData.TaskStatus.Remove(key);
            }
        }
        
        // Add new blank entries for valid tasks.
        foreach (var validRaid in validNormalRaids) {
            ModuleData.TaskStatus.TryAdd(validRaid, false);
        }
        ModuleData.MarkDirty();
    }
    
    private IEnumerable<string> GetIncompleteTasks()
        => ModuleConfig.TrackedTasks
            .Where(task => task.Value)
            .Where(task => !ModuleData.TaskStatus[task.Key])
            .Select(task => Services.DataManager.GetExcelSheet<ContentFinderCondition>().GetRow(task.Key).Name.ToString());
}
