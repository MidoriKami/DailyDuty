using System;
using System.Globalization;
using System.Linq;
using DailyDuty.Classes;
using DailyDuty.Localization;
using DailyDuty.Models;
using DailyDuty.Modules.BaseModules;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Inventory.InventoryEventArgTypes;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.Modules;

public class TreasureMapConfig : ModuleConfig;

public class TreasureMapData : ModuleData {
	public DateTime LastMapGatheredTime = DateTime.MinValue;
	public bool MapAvailable = true;
	
	protected override void DrawModuleData() {
		DrawDataTable([
			(Strings.LastMapGathered, LastMapGatheredTime.ToLocalTime().ToString(CultureInfo.CurrentCulture)),
			(Strings.MapAvailable, MapAvailable.ToString()),
		]);
	}
}

public class TreasureMap : Modules.Special<TreasureMapData, TreasureMapConfig> {
	public override ModuleName ModuleName => ModuleName.TreasureMap;

	public override DateTime GetNextReset() => DateTime.MaxValue;

	public override TimeSpan GetModulePeriod() => TimeSpan.FromHours(18);

	public TreasureMap()
		=> Service.GameInventory.ItemAddedExplicit += OnItemAdded;

	public override void Dispose()
		=> Service.GameInventory.ItemAddedExplicit -= OnItemAdded;

	public override void Reset() {
		Data.MapAvailable = true;
        
		base.Reset();
	}

	private void OnItemAdded(InventoryItemAddedArgs data) {
		if (!Service.Condition[ConditionFlag.Gathering42]) return;
		
		if (Service.DataManager.GetExcelSheet<TreasureHuntRank>()!.Any(treasureHunt => treasureHunt.ItemName.Row == data.Item.ItemId)) {
			Data.MapAvailable = false;
			Data.LastMapGatheredTime = DateTime.UtcNow;
			Data.NextReset = Data.LastMapGatheredTime + TimeSpan.FromHours(18);
			Config.Suppressed = false;
			DataChanged = true;
			ConfigChanged = true;
		}
	}

	protected override ModuleStatus GetModuleStatus() 
		=> Data.MapAvailable ? ModuleStatus.Incomplete : ModuleStatus.Complete;

	protected override StatusMessage GetStatusMessage() => new() {
		Message = Strings.MapAvailable,
	};
}