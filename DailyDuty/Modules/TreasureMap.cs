using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DailyDuty.Classes;
using DailyDuty.Models;
using DailyDuty.Modules.BaseModules;
using Dalamud.Game.ClientState.Conditions;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.Sheets;

namespace DailyDuty.Modules;

public class TreasureMapConfig : ModuleConfig;

public class TreasureMapData : ModuleData {
	public DateTime LastMapGatheredTime = DateTime.MinValue;
	public bool MapAvailable = true;
	
	protected override void DrawModuleData() {
		DrawDataTable([
			("Last Map Gathered", LastMapGatheredTime.ToLocalTime().ToString(CultureInfo.CurrentCulture)),
			("Map Available", MapAvailable.ToString()),
		]);
	}
}

public unsafe class TreasureMap : BaseModules.Modules.Special<TreasureMapData, TreasureMapConfig> {
	public override ModuleName ModuleName => ModuleName.TreasureMap;

	public override DateTime GetNextReset() => DateTime.MaxValue;

	private List<TreasureHuntRank> treasureMaps = [];
	private List<uint> inventoryMaps = [];
	private bool gatheringStarted;

	public override void Load() {
		base.Load();

		treasureMaps = Service.DataManager.GetExcelSheet<TreasureHuntRank>()
			.Where(map => map.ItemName.RowId is not 0)
			.ToList();
	}

	public override void Reset() {
		Data.MapAvailable = true;
        
		base.Reset();
	}

	public override void Update() {
		if (Service.Condition[ConditionFlag.ExecutingGatheringAction] && !gatheringStarted) {
			gatheringStarted = true;
			OnGatheringStart();
		} 
		else if (!Service.Condition[ConditionFlag.ExecutingGatheringAction] && gatheringStarted) {
			gatheringStarted = false;
			OnGatheringStop();
		}
        
		base.Update();
	}

	private void OnGatheringStart() {
		inventoryMaps.Clear();
		inventoryMaps = GetInventoryTreasureMaps();
	}

	private void OnGatheringStop() {
		var newInventoryMaps = GetInventoryTreasureMaps();

		if (newInventoryMaps.Count > inventoryMaps.Count) {
			Data.MapAvailable = false;
			Data.LastMapGatheredTime = DateTime.UtcNow;
			Data.NextReset = Data.LastMapGatheredTime + TimeSpan.FromHours(18);
			Config.Suppressed = false;
			DataChanged = true;
			ConfigChanged = true;
		}
	}

	private List<uint> GetInventoryTreasureMaps() {
		var mapsInInventory =
			from map in treasureMaps
			where InventoryManager.Instance()->GetInventoryItemCount(map.ItemName.RowId) > 0
			select map.ItemName.RowId;
        
		return mapsInInventory.ToList();
	}

	protected override ModuleStatus GetModuleStatus() 
		=> Data.MapAvailable ? ModuleStatus.Incomplete : ModuleStatus.Complete;

	protected override StatusMessage GetStatusMessage()
		=> "Map Available";
}