using System.Threading.Tasks;
using DailyDuty.Classes;
using DailyDuty.Enums;
using DailyDuty.Interfaces;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Components.Builders;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using Lumina.Text.ReadOnly;

namespace DailyDuty.Modules;

public class DomanEnclave : Module<DomanEnclaveData, DomanEnclaveConfig>, IWeeklyResettable, IFrameworkUpdatable, IClickableTooltip {

    public override string DisplayName => Strings.DomanEnclave_DisplayName;

    public override DomanEnclaveData Data { get; protected set; } = null!;
    public override DomanEnclaveConfig Config { get; protected set; } = null!;

    private TextNode? allowanceText;
    private TextNode? donatedText;
    private TextNode? remainingText;

    private VerticalListNode? dataPage;
    private VerticalListNode? warningPage;

    private ChatLinkPayload? domanEnclaveTeleportPayload;

    public override async Task EnableAsync() {
        Data = await DomanEnclaveData.Load();
        Config = await DomanEnclaveConfig.Load();

        domanEnclaveTeleportPayload = ChatLinkPayloads.TeleportToDomanEnclave();
    }

    public override Task DisableAsync() {
        domanEnclaveTeleportPayload?.Dispose();
        domanEnclaveTeleportPayload = null;

        allowanceText = null;
        donatedText = null;
        remainingText = null;

        dataPage = null;
        warningPage = null;

        return Task.CompletedTask;
    }

    public override void BuildDataUi(NodeBuilder builder) => builder
        .FittedContent(inner => inner
            .VerticalList(list => list
                .Text(Strings.DomanEnclave_UnavailableStatus, textNode => {
                    textNode.AlignmentType = AlignmentType.Center;
                    textNode.LayoutFill = LayoutFill.Both;
                    textNode.FontSize = 14;
                    textNode.TextColor = ColorHelper.GetColor(32);
                }), out warningPage)
            .VerticalList(list => list
                .DataRow(Strings.DomanEnclave_CurrentMaxAllowance, "CURRENT_ALLOWANCE", out allowanceText)
                .DataRow(Strings.DomanEnclave_DonatedThisWeek, "DONATED", out donatedText)
                .DataRow(Strings.DomanEnclave_AllowanceRemaining, "REMAINING", out remainingText), out dataPage));

    public override void UpdateDataUi() {
        allowanceText?.String = Data.WeeklyAllowance.ToString();
        donatedText?.String = Data.DonatedThisWeek.ToString();
        remainingText?.String = Data.RemainingAllowance.ToString();

        dataPage?.IsVisible = Data.WeeklyAllowance is not 0;
        warningPage?.IsVisible = Data.WeeklyAllowance is 0;
    }

    public unsafe void Update() {
        var manager = DomanEnclaveManager.Instance();
        if (manager is null) return;

        var allowance = manager->State.Allowance;
        var donated = manager->State.Donated;

        if (allowance is 0) return;

        if (Data.WeeklyAllowance != allowance) {
            Data.WeeklyAllowance = allowance;
            Data.Save();
        }

        if (Data.DonatedThisWeek != donated) {
            Data.DonatedThisWeek = donated;
            Data.Save();
        }
    }

    public ReadOnlySeString GetTooltip() {
        if (Data.WeeklyAllowance is 0) return Strings.DomanEnclave_UnknownStatus;

        return $"{Data.RemainingAllowance:N0} {Strings.DomanEnclave_AllowanceRemaining}";
    }

    public void OnTooltipClicked()
        => domanEnclaveTeleportPayload?.ClickAction();

    protected override async Task ResetAsync() {
        Data.DonatedThisWeek = 0;
        await Data.Save();
    }

    protected override CompletionStatus GetCompletionStatus() => Data switch {
        { WeeklyAllowance: 0 } => CompletionStatus.Unknown,
        { WeeklyAllowance: not 0, RemainingAllowance: 0 } => CompletionStatus.Complete,
        { WeeklyAllowance: not 0, RemainingAllowance: not 0 } => CompletionStatus.Incomplete,
        _ => CompletionStatus.Unknown,
    };

    protected override void SendStatusMessage(ReadOnlySeString? message) => IChatGui.Get().PrintMessage(
        Config.MessageChatChannel,
        domanEnclaveTeleportPayload,
        DisplayName,
        message ?? $"{Data.RemainingAllowance:N0} {Strings.DomanEnclave_AllowanceRemaining}"
    );

    protected override void SendResetMessage(ReadOnlySeString? message) => IChatGui.Get().PrintMessage(
        Config.MessageChatChannel,
        domanEnclaveTeleportPayload,
        DisplayName,
        message ?? "Module has been reset."
    );
}

public class DomanEnclaveData : ModuleDataFile<DomanEnclaveData> {
    protected override string FileName => "DomanEnclave.data.json";

    public int WeeklyAllowance { get; set; }
    public int DonatedThisWeek { get; set; }

    public int RemainingAllowance => WeeklyAllowance - DonatedThisWeek;
}

public class DomanEnclaveConfig : ModuleConfigFile<DomanEnclaveConfig> {
    protected override string FileName => "DomanEnclave.config.json";
}
