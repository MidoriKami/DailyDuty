using System.Threading.Tasks;
using DailyDuty.Data;
using DailyDuty.Enums;
using DailyDuty.Interfaces;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using KamiToolKit.Controls.Builders;
using KamiToolKit.UI.Nodes;
using Lumina.Text.ReadOnly;

namespace DailyDuty.Modules;

public class BeastTribes : Module<BeastTribesData, BeastTribesConfig>, IDailyResettable, IConfigurable, ITooltip {

    public override string DisplayName => Strings.BeastTribes_DisplayName;

    public override BeastTribesData Data { get; protected set; } = null!;
    public override BeastTribesConfig Config { get; protected set; } = null!;

    private TextNode? allowancesNode;

    public override async Task EnableAsync() {
        Data = await BeastTribesData.Load();
        Config = await BeastTribesConfig.Load();
    }

    public override Task DisableAsync() {
        allowancesNode = null;

        return Task.CompletedTask;
    }

    public override void BuildDataUi(NodeBuilder builder) => builder
        .DataRow(Strings.BeastTribes_Available, "0", out allowancesNode);

    public override void UpdateDataUi()
        => allowancesNode?.String = RemainingAllowances.ToString();

    public ReadOnlySeString GetTooltip()
        => string.Format(Strings.Tooltip_AllowancesAboveThreshold, RemainingAllowances - Config.NotificationThreshold);

    public void BuildConfigUi(NodeBuilder builder) => builder
        .TextCentered(Strings.BeastTribes_ThresholdNote)
        .Row(Strings.BeastTribes_NotificationThreshold, rowBuilder => rowBuilder
            .IntSlider(Config.NotificationThreshold, 0, 12, newValue => {
                Config.NotificationThreshold = newValue;
                Config.Save();
            }));

    protected override Task ResetAsync()
        => Task.CompletedTask;

    protected override CompletionStatus GetCompletionStatus()
        => RemainingAllowances > Config.NotificationThreshold ? CompletionStatus.Incomplete : CompletionStatus.Complete;

    protected override void SendStatusMessage(ReadOnlySeString? message) => IChatGui.Get().PrintMessage(
        Config.MessageChatChannel,
        DisplayName,
        message ?? $"{RemainingAllowances - Config.NotificationThreshold} {Strings.BeastTribes_StatusMessageIncomplete}"
    );

    protected override void SendResetMessage(ReadOnlySeString? message) => IChatGui.Get().PrintMessage(
        Config.MessageChatChannel,
        DisplayName,
        message ?? Strings.BeastTribes_StatusMessageReset
    );

    private static unsafe uint RemainingAllowances
        => QuestManager.Instance()->GetBeastTribeAllowance();
}

public class BeastTribesData : ModuleDataFile<BeastTribesData> {
    protected override string FileName => "BeastTribes.data.json";
}

public class BeastTribesConfig : ModuleConfigFile<BeastTribesConfig> {
    protected override string FileName => "BeastTribes.config.json";

    public int NotificationThreshold { get; set; }
}
