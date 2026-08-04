using System;
using System.Threading.Tasks;
using DailyDuty.Classes;
using DailyDuty.CustomNodes;
using DailyDuty.Enums;
using Dalamud.Game.Chat;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using Lumina.Excel.Sheets;
using Lumina.Extensions;

namespace DailyDuty.Features.TreasureMap;

public class TreasureMap : Module<ConfigBase, TreasureMapData> {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = Strings.TreasureMap_DisplayName,
        FileName = "TreasureMap",
        Type = ModuleType.Special,
        Tags = ["DoH", "DoL", "Exp"],
    };

    public override DataNodeBase DataNode => new TreasureMapDataNode(this);

    protected override StatusMessage GetStatusMessage()
        => Strings.TreasureMap_Gatherable;

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

    protected override Task OnModuleEnable() {
        IChatGui.Get().LogMessage += OnLogMessage;

        return Task.CompletedTask;
    }

    protected override Task OnModuleDisable() {
        IChatGui.Get().LogMessage -= OnLogMessage;

        return Task.CompletedTask;
    }

    private void OnLogMessage(ILogMessage message) {
        if (!ICondition.Get()[ConditionFlag.ExecutingGatheringAction]) return;

        // You obtain <kilo(lnum2,\,)> <ennoun(Item,3,lnum1,lnum2,1)>.
        if (message.LogMessageId is not 1053) return;
        if (!message.TryGetIntParameter(0, out var itemId) || itemId is 0) return;

        var itemInfo = IDataManager.Get().GetExcelSheet<Item>().GetRow((uint) itemId);

        var treasureHunt = IDataManager.Get().GetExcelSheet<TreasureHuntRank>().FirstOrNull(hunt => hunt.ItemName.RowId == itemId);
        if (treasureHunt is null) return;

        IPluginLog.Get().Debug($"Player gathered {itemInfo.Name} corresponding to TreasureHuntRank#{treasureHunt.Value.RowId}");

        ModuleData.LastMapGatheredTime = DateTime.UtcNow;
        ModuleData.NextReset = ModuleData.LastMapGatheredTime + TimeSpan.FromHours(18);
        ModuleConfig.Suppressed = false;

        ModuleData.MarkDirty();
        ModuleConfig.MarkDirty();
    }
}
