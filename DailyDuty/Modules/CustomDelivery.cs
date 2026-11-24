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

public class CustomDelivery : Module<CustomDeliveryData, CustomDeliveryConfig>, IWeeklyResettable, IConfigurable, ITooltip {

    public override string DisplayName => Strings.CustomDelivery_DisplayName;

    public override CustomDeliveryData Data { get; protected set; } = null!;
    public override CustomDeliveryConfig Config { get; protected set; } = null!;

    private TextNode? currentAllowances;

    public override async Task EnableAsync() {
        Data = await CustomDeliveryData.Load();
        Config = await CustomDeliveryConfig.Load();
    }

    public override Task DisableAsync() {
        currentAllowances = null;

        return Task.CompletedTask;
    }

    public override void BuildDataUi(NodeBuilder builder) => builder
        .DataRow(Strings.CustomDelivery_AllowancesRemaining, "CURRENT ALLOWANCES", out currentAllowances);

    public override void UpdateDataUi()
        => currentAllowances?.String = RemainingAllowances.ToString();

    public ReadOnlySeString GetTooltip()
        => string.Format(Strings.Tooltip_AllowancesAboveThreshold, RemainingAllowances - Config.NotificationThreshold);

    public void BuildConfigUi(NodeBuilder builder) => builder
        .TextCentered(Strings.CustomDelivery_ThresholdNote)
        .Row(Strings.CustomDelivery_NotificationThreshold, rowBuilder => rowBuilder
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
        message ?? $"{RemainingAllowances - Config.NotificationThreshold} {Strings.CustomDelivery_StatusMessageIncomplete}"
    );

    protected override void SendResetMessage(ReadOnlySeString? message) => IChatGui.Get().PrintMessage(
        Config.MessageChatChannel,
        DisplayName,
        message ?? Strings.CustomDelivery_StatusMessageReset
    );

    private static unsafe int RemainingAllowances
        => SatisfactionSupplyManager.Instance()->GetRemainingAllowances();
}

public class CustomDeliveryData : ModuleDataFile<CustomDeliveryData> {
    protected override string FileName => "CustomDelivery.data.json";
}

public class CustomDeliveryConfig : ModuleConfigFile<CustomDeliveryConfig> {
    protected override string FileName => "CustomDelivery.config.json";

    public int NotificationThreshold { get; set; }
}
