using System.Threading.Tasks;
using DailyDuty.Classes;
using DailyDuty.Enums;
using DailyDuty.Interfaces;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using KamiToolKit.Components.Builders;
using KamiToolKit.Nodes;
using Lumina.Text.ReadOnly;

namespace DailyDuty.Modules;

public class CustomDelivery : Module<CustomDeliveryData, CustomDeliveryConfig>, IWeeklyResettable, IConfigurable {

    public override string DisplayName => Strings.CustomDelivery_DisplayName;

    public override CustomDeliveryData Data { get; protected set; } = null!;
    public override CustomDeliveryConfig Config { get; protected set; } = null!;

    private TextNode? currentAllowances;

    public override async Task EnableAsync() {
        Data = await CustomDeliveryData.Load();
        Config = await CustomDeliveryConfig.Load();
    }

    public override Task DisableAsync()
        => Task.CompletedTask;

    public override void BuildDataUi(NodeBuilder builder) => builder
        .DataRow(Strings.CustomDelivery_AllowancesRemaining, "CURRENT ALLOWANCES", out currentAllowances);

    public override void UpdateDataUi()
        => currentAllowances?.String = RemainingAllowances.ToString();

    public void BuildConfigUi(NodeBuilder builder) => builder
        .Row(Strings.CustomDelivery_AllowanceAmount, rowBuilder => rowBuilder
            .IntSlider(Config.NotificationThreshold, 0, 12, newValue => {
                Config.NotificationThreshold = newValue;
                Config.Save();
            }))
        .Row(Strings.CustomDelivery_WarningTrigger, rowBuilder => rowBuilder
            .Combo(Config.ComparisonMode, newValue => {
                Config.ComparisonMode = newValue;
                Config.Save();
            }));

    protected override Task ResetAsync()
        => Task.CompletedTask;

    protected override CompletionStatus GetCompletionStatus() => Config.ComparisonMode switch {
        ComparisonMode.Above => RemainingAllowances > Config.NotificationThreshold ? CompletionStatus.Incomplete : CompletionStatus.Complete,
        ComparisonMode.Below => RemainingAllowances < Config.NotificationThreshold ? CompletionStatus.Incomplete : CompletionStatus.Complete,
        ComparisonMode.Equal => RemainingAllowances != Config.NotificationThreshold ? CompletionStatus.Incomplete : CompletionStatus.Complete,
        _ => CompletionStatus.Unknown,
    };

    protected override void SendStatusMessage(ReadOnlySeString? message) => IChatGui.Get().PrintMessage(
        Config.MessageChatChannel,
        DisplayName,
        message ?? $"{RemainingAllowances - Config.NotificationThreshold} {Strings.StatusMessages_CustomDeliveryIncomplete}"
    );

    protected override void SendResetMessage(ReadOnlySeString? message) => IChatGui.Get().PrintMessage(
        Config.MessageChatChannel,
        DisplayName,
        message ?? "Module has been reset."
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
    public ComparisonMode ComparisonMode { get; set; } = ComparisonMode.Above;
}
