using System;
using System.Linq;
using DailyDuty.Abstracts;
using DailyDuty.Models;
using DailyDuty.Models.Attributes;
using DailyDuty.Models.Enums;
using FFXIVClientStructs.FFXIV.Client.Game;
using KamiLib.Caching;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.System;

public class TreasureMapConfig : ModuleConfigBase
{
    
}

public class TreasureMapData : ModuleDataBase
{
    [DataDisplay("LastMapGathered")]
    public DateTime LastMapGatheredTime = DateTime.MinValue;
    
    [DataDisplay("MapAvailable")]
    public bool MapAvailable;
}

public unsafe class TreasureMap : Module.SpecialModule
{
    public override ModuleName ModuleName => ModuleName.TreasureMap;

    public override ModuleConfigBase ModuleConfig { get; protected set; } = new TreasureMapConfig();
    public override ModuleDataBase ModuleData { get; protected set; } = new TreasureMapData();
    private TreasureMapData Data => ModuleData as TreasureMapData ?? new TreasureMapData();

    public override TimeSpan GetResetPeriod() => TimeSpan.FromHours(18);
    protected override DateTime GetNextReset() => Data.LastMapGatheredTime + TimeSpan.FromHours(18);

    public override void Reset()
    {
        Data.MapAvailable = true;
        
        base.Reset();
    }

    public override void Update()
    { 
        if (Data.MapAvailable)
        {
            foreach (var item in LuminaCache<TreasureHuntRank>.Instance.Where(map => map.ItemName.Row is not 0))
            {
                if (InventoryManager.Instance()->GetInventoryItemCount(item.ItemName.Row) > 0)
                {
                    Data.MapAvailable = false;
                    Data.LastMapGatheredTime = DateTime.UtcNow;
                    DataChanged = true;
                }
            }
        }
        
        base.Update();
    }

    protected override ModuleStatus GetModuleStatus() => Data.MapAvailable ? ModuleStatus.Incomplete : ModuleStatus.Complete;

    protected override StatusMessage GetStatusMessage() => new()
    {
        Message = "Map Available",
    };
}