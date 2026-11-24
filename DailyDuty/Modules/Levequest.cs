using System;
using System.Threading.Tasks;
using DailyDuty.Data;
using DailyDuty.Enums;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using KamiToolKit.Controls.Builders;
using KamiToolKit.UI.Nodes;
using Lumina.Text.ReadOnly;

namespace DailyDuty.Modules;

public class Levequest : Module<LevequestData, LevequestConfig>, ISpecialResettable, IConfigurable {

    public override string DisplayName => Strings.Levequest_DisplayName;

    public override LevequestData Data { get; protected set; } = null!;
    public override LevequestConfig Config { get; protected set; } = null!;

    private TextNode? allowancesNode;
    private TextNode? acceptedNode;

    public override async Task EnableAsync() {
        Data = await LevequestData.Load();
        Config = await LevequestConfig.Load();
    }

    public override Task DisableAsync() {
        allowancesNode = null;
        acceptedNode = null;

        return Task.CompletedTask;
    }

    public override void BuildDataUi(NodeBuilder builder) => builder
        .DataRow(Strings.Levequest_Available, "0", out allowancesNode)
        .DataRow(Strings.Levequest_Accepted, "0", out acceptedNode);

    public override void UpdateDataUi() {
        unsafe {
            allowancesNode?.String = QuestManager.Instance()->NumLeveAllowances.ToString();
            acceptedNode?.String = QuestManager.Instance()->NumAcceptedLeveQuests.ToString();
        }
    }

    public void BuildConfigUi(NodeBuilder builder) => builder
        .TextCentered(Strings.Levequest_ThresholdNote)
        .Row(Strings.Levequest_NotificationThreshold, rowBuilder => rowBuilder
            .IntSlider(Config.NotificationThreshold, 0, 100, newValue => {
                Config.NotificationThreshold = newValue;
                Config.Save();
            }));

    protected override Task ResetAsync()
        => Task.CompletedTask;

    protected override CompletionStatus GetCompletionStatus()
        => RemainingAllowances > Config.NotificationThreshold ? CompletionStatus.Incomplete : CompletionStatus.Complete;

    protected override DateTime GetSpecialNextResetTime()
        => Time.NextLeveAllowanceReset();

    protected override TimeSpan GetSpecialResetPeriod()
        => TimeSpan.FromHours(12);

    private static unsafe int RemainingAllowances
        => QuestManager.Instance()->NumLeveAllowances;

    protected override void SendStatusMessage(ReadOnlySeString? message) => IChatGui.Get().PrintMessage(
        Config.MessageChatChannel,
        DisplayName,
        message ?? $"{RemainingAllowances - Config.NotificationThreshold} {Strings.Levequest_StatusMessageIncomplete}"
    );

    protected override void SendResetMessage(ReadOnlySeString? message) => IChatGui.Get().PrintMessage(
        Config.MessageChatChannel,
        DisplayName,
        message ?? Strings.Levequest_StatusMessageReset
    );
}

public class LevequestData : ModuleDataFile<LevequestData> {
    protected override string FileName => "Levequest.data.json";
}

public class LevequestConfig : ModuleConfigFile<LevequestConfig> {
    protected override string FileName => "Levequest.config.json";

    public int NotificationThreshold { get; set; } = 95;
}
