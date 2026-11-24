using System;
using System.Linq;
using System.Threading.Tasks;
using DailyDuty.Data;
using DailyDuty.Enums;
using DailyDuty.Interfaces;
using Lumina.Excel.Sheets;
using Dalamud.Game.Chat;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using KamiToolKit.Controls.Builders;
using KamiToolKit.UI.Nodes;
using Lumina.Text.ReadOnly;

namespace DailyDuty.Modules;

public class TreasureMap : Module<TreasureMapData, TreasureMapConfig>, ISpecialResettable {

    public override string DisplayName => Strings.TreasureMap_DisplayName;

    public override TreasureMapData Data { get; protected set; } = null!;
    public override TreasureMapConfig Config { get; protected set; } = null!;

    private TextNode? lastGatheredNode;

    public override async Task EnableAsync() {
        Data = await TreasureMapData.Load();
        Config = await TreasureMapConfig.Load();

        IChatGui.Get().LogMessage += OnLogMessage;
    }

    public override Task DisableAsync() {
        IChatGui.Get().LogMessage -= OnLogMessage;

        lastGatheredNode = null;

        return Task.CompletedTask;
    }

    public override void BuildDataUi(NodeBuilder builder) => builder
        .DataRow(Strings.TreasureMap_LastGathered, Strings.TreasureMap_AttemptsNotUpdated, out lastGatheredNode);

    public override void UpdateDataUi() {
        if (Data.LastMapGatheredTime == DateTime.MinValue) {
            lastGatheredNode?.String = Strings.TreasureMap_NeverGathered;
            return;
        }

        var dateTime = Data.LastMapGatheredTime;
        var shortDate = dateTime.ToLocalTime().ToShortDateString();
        var shortTime = dateTime.ToLocalTime().ToShortTimeString();

        lastGatheredNode?.String = $"{shortDate} {shortTime}";
    }

    protected override async Task ResetAsync() {
        Data.LastMapGatheredTime = DateTime.MinValue;
        await Data.Save();
    }

    protected override CompletionStatus GetCompletionStatus()
        => Data.LastMapGatheredTime == DateTime.MinValue ? CompletionStatus.Incomplete : CompletionStatus.Complete;

    protected override void SendStatusMessage(ReadOnlySeString? message) => IChatGui.Get().PrintMessage(
        Config.MessageChatChannel,
        DisplayName,
        message ?? Strings.TreasureMap_Gatherable
    );

    protected override void SendResetMessage(ReadOnlySeString? message) => IChatGui.Get().PrintMessage(
        Config.MessageChatChannel,
        DisplayName,
        message ?? Strings.TreasureMap_StatusMessageReset
    );

    protected override TimeSpan GetSpecialResetPeriod()
        => TimeSpan.FromHours(18);

    protected override DateTime GetSpecialNextResetTime() {
        if (Data.LastMapGatheredTime == DateTime.MinValue) return DateTime.MaxValue;

        return Data.LastMapGatheredTime + TimeSpan.FromHours(18);
    }

    private void OnLogMessage(ILogMessage message) {
        if (!ICondition.Get()[ConditionFlag.ExecutingGatheringAction]) return;

        // You obtain <kilo(lnum2,\,)> <ennoun(Item,3,lnum1,lnum2,1)>.
        if (message.LogMessageId is not 1053) return;
        if (!message.TryGetIntParameter(0, out var itemId) || itemId is 0) return;

        var itemInfo = IDataManager.Get().GetExcelSheet<Item>().GetRow((uint) itemId);

        var treasureHunt = IDataManager.Get().GetExcelSheet<TreasureHuntRank>()
            .FirstOrDefault(hunt => hunt.ItemName.RowId == itemId);
        if (treasureHunt.RowId == 0) return;

        IPluginLog.Get().Debug($"Player gathered {itemInfo.Name} corresponding to TreasureHuntRank#{treasureHunt.RowId}");

        Data.LastMapGatheredTime = DateTime.UtcNow;
        Config.Suppressed = false;

        Data.Save();
        Config.Save();
    }
}

public class TreasureMapData : ModuleDataFile<TreasureMapData> {
    protected override string FileName => "TreasureMap.data.json";

    public DateTime LastMapGatheredTime = DateTime.MinValue;
}

public class TreasureMapConfig : ModuleConfigFile<TreasureMapConfig> {
    protected override string FileName => "TreasureMap.config.json";
}
