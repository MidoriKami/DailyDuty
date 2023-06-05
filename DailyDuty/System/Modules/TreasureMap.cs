using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.Abstracts;
using DailyDuty.Models;
using DailyDuty.Models.Enums;
using DailyDuty.System.Localization;
using Dalamud.Game.ClientState.Conditions;
using FFXIVClientStructs.FFXIV.Client.Game;
using KamiLib.AutomaticUserInterface;
using KamiLib.Caching;
using Lumina.Excel.GeneratedSheets;
using Condition = KamiLib.GameState.Condition;

namespace DailyDuty.System;

public class TreasureMapConfig : ModuleConfigBase
{
    // No Config Options
}

public class TreasureMapData : ModuleDataBase
{
    [DrawCategory("ModuleData", 1)]
    [DateTimeDisplay("LastMapGathered")]
    public DateTime LastMapGatheredTime = DateTime.MinValue;
    
    [DrawCategory("ModuleData", 1)]
    [BoolDisplay("MapAvailable")]
    public bool MapAvailable = true;
}

public unsafe class TreasureMap : Module.SpecialModule
{
    public override ModuleName ModuleName => ModuleName.TreasureMap;

    public override ModuleConfigBase ModuleConfig { get; protected set; } = new TreasureMapConfig();
    public override ModuleDataBase ModuleData { get; protected set; } = new TreasureMapData();
    private TreasureMapData Data => ModuleData as TreasureMapData ?? new TreasureMapData();
    private TreasureMapConfig Config => ModuleConfig as TreasureMapConfig ?? new TreasureMapConfig();

    protected override DateTime GetNextReset() => DateTime.MaxValue;

    private List<TreasureHuntRank> treasureMaps = new();
    private List<uint> inventoryMaps = new();
    private bool gatheringStarted;

    public override void Load()
    {
        base.Load();

        treasureMaps = LuminaCache<TreasureHuntRank>.Instance
            .Where(map => map.ItemName.Row is not 0)
            .ToList();
    }

    public override void Reset()
    {
        Data.MapAvailable = true;
        
        base.Reset();
    }

    public override void Update()
    {
        if (Condition.CheckFlag(ConditionFlag.Gathering42) && !gatheringStarted)
        {
            gatheringStarted = true;
            OnGatheringStart();
        } 
        else if (!Condition.CheckFlag(ConditionFlag.Gathering42) && gatheringStarted)
        {
            gatheringStarted = false;
            OnGatheringStop();
        }
        
        base.Update();
    }

    private void OnGatheringStart()
    {
        inventoryMaps.Clear();
        inventoryMaps = GetInventoryTreasureMaps();
    }

    private void OnGatheringStop()
    {
        var newInventoryMaps = GetInventoryTreasureMaps();

        if (newInventoryMaps.Count > inventoryMaps.Count)
        {
            Data.MapAvailable = false;
            Data.LastMapGatheredTime = DateTime.UtcNow;
            Data.NextReset = Data.LastMapGatheredTime + TimeSpan.FromHours(18);
            Config.Suppressed = false;
            DataChanged = true;
            ConfigChanged = true;
        }
    }

    private List<uint> GetInventoryTreasureMaps()
    {
        var mapsInInventory =
            from map in treasureMaps
            where InventoryManager.Instance()->GetInventoryItemCount(map.ItemName.Row) > 0
            select map.ItemName.Row;
        
        return mapsInInventory.ToList();
    }
    
    protected override ModuleStatus GetModuleStatus() => Data.MapAvailable ? ModuleStatus.Incomplete : ModuleStatus.Complete;

    protected override StatusMessage GetStatusMessage() => new()
    {
        Message = Strings.MapAvailable,
    };
}