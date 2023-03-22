using System;
using System.Linq;
using DailyDuty.Abstracts;
using DailyDuty.Interfaces;
using DailyDuty.Models;
using DailyDuty.Models.Attributes;
using DailyDuty.Models.Enums;
using DailyDuty.System.Localization;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
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
    [DataDisplay("LastMapGathered")]
    public DateTime LastMapGatheredTime = DateTime.MinValue;
    
    [DataDisplay("MapAvailable")]
    public bool MapAvailable = true;
}

public class TreasureMap : Module.SpecialModule, IChatMessageReceiver
{
    public override ModuleName ModuleName => ModuleName.TreasureMap;

    public override ModuleConfigBase ModuleConfig { get; protected set; } = new TreasureMapConfig();
    public override ModuleDataBase ModuleData { get; protected set; } = new TreasureMapData();
    private TreasureMapData Data => ModuleData as TreasureMapData ?? new TreasureMapData();

    public override TimeSpan GetResetPeriod() => TimeSpan.FromHours(18);
    protected override DateTime GetNextReset() => DateTime.MaxValue;

    public override void Reset()
    {
        Data.MapAvailable = true;
        
        base.Reset();
    }
    
    public void OnChatMessage(XivChatType type, uint senderID, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        if ((int)type != 2115 || !Condition.IsGathering()) return;
        if (message.Payloads.FirstOrDefault(p => p is ItemPayload) is not ItemPayload item) return;
        if (!LuminaCache<TreasureHuntRank>.Instance.Any(map => map.ItemName.Row == item.ItemId)) return;

        Data.MapAvailable = false;
        Data.LastMapGatheredTime = DateTime.UtcNow;
        Data.NextReset = Data.LastMapGatheredTime + TimeSpan.FromHours(18);
        DataChanged = true;
    }
    
    protected override ModuleStatus GetModuleStatus() => Data.MapAvailable ? ModuleStatus.Incomplete : ModuleStatus.Complete;

    protected override StatusMessage GetStatusMessage() => new()
    {
        Message = Strings.MapAvailable,
    };
}