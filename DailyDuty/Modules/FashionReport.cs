using System;
using System.Threading.Tasks;
using DailyDuty.Data;
using DailyDuty.UI;
using DailyDuty.Enums;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using KamiToolKit.Controls.Builders;
using KamiToolKit.UI.Nodes;
using KamiToolKit.UI.Nodes.Layout;
using Lumina.Text.ReadOnly;

namespace DailyDuty.Modules;

public class FashionReport : Module<FashionReportData, FashionReportConfig>, ISpecialResettable, IClickableTooltip, INpcInteractListener, IConfigurable {

    public override string DisplayName => Strings.FashionReport_DisplayName;

    public override FashionReportData Data { get; protected set; } = null!;
    public override FashionReportConfig Config { get; protected set; } = null!;

    private TextNode? scoreText;
    private TextNode? attemptsText;

    private VerticalListNode? warningPage;
    private VerticalListNode? dataPage;

    private ChatLinkPayload? goldSaucerTeleportPayload;

    public override async Task EnableAsync() {
        Data = await FashionReportData.Load();
        Config = await FashionReportConfig.Load();

        goldSaucerTeleportPayload = ChatLinkPayload.TeleportToGoldSaucer();
    }

    public override Task DisableAsync() {
        goldSaucerTeleportPayload?.Dispose();
        goldSaucerTeleportPayload = null;

        scoreText = null;
        attemptsText = null;

        warningPage = null;
        dataPage = null;

        return Task.CompletedTask;
    }

    public override void BuildDataUi(NodeBuilder builder) => builder
        .FittedContent(inner => inner
            .VerticalList(list => list
                .TextWarningFilled(Strings.FashionReport_NotAvailable), out warningPage)
            .VerticalList(list => list
                .DataRow(Strings.FashionReport_HighestScore, "SCORE", out scoreText)
                .DataRow(Strings.MiniCactpot_AttemptsAvailable, "ATTEMPTS", out attemptsText), out dataPage));

    public override void UpdateDataUi() {
        dataPage?.IsVisible = IsFashionReportAvailable;
        warningPage?.IsVisible = !IsFashionReportAvailable;

        scoreText?.String = Data.HighestScore > 0 ? Data.HighestScore.ToString() : Strings.FashionReport_ScoreNotUpdated;
        attemptsText?.String = Data.AttemptsRemaining.ToString();
    }

    protected override TimeSpan GetSpecialResetPeriod()
        => TimeSpan.FromDays(7);

    protected override DateTime GetSpecialNextResetTime()
        => Time.NextFashionReportReset();

    public ReadOnlySeString GetTooltip() {
        if (Data.AttemptsRemaining is 4) {
            return Strings.FashionReport_NotAttempted;
        }

        return Config.CompletionMode switch {
            FashionReportMode.Plus80 => $"Score: {Data.HighestScore}/80, {Data.AttemptsRemaining} Left",
            _ => $"Score: {Data.HighestScore}, {Data.AttemptsRemaining} Left",
        };
    }

    public void OnTooltipClicked()
        => goldSaucerTeleportPayload?.ClickAction();

    public void BuildConfigUi(NodeBuilder builder) => builder
        .Row(Strings.FashionReport_CompletionMode, rowBuilder => rowBuilder
            .Combo(Config.CompletionMode, mode => {
                Config.CompletionMode = mode;
                Config.Save();
            }));

    protected override async Task ResetAsync() {
        Data.HighestScore = 0;
        Data.AttemptsRemaining = 4;
        await Data.Save();
    }

    protected override CompletionStatus GetCompletionStatus() {
        if (!IsFashionReportAvailable) return CompletionStatus.Complete;

        return Config.CompletionMode switch {
            FashionReportMode.Single when Data.AttemptsRemaining < 4 => CompletionStatus.Complete,
            FashionReportMode.All when Data.AttemptsRemaining is 0 => CompletionStatus.Complete,
            FashionReportMode.Plus80 when Data.HighestScore >= 80 => CompletionStatus.Complete,
            _ => CompletionStatus.Incomplete,
        };
    }

    protected override DateTime? GetWindowEndTimeOverride()
        => IsFashionReportAvailable ? Time.NextWeeklyReset() : null;

    public unsafe void OnNpcInteract(EventFramework* thisPtr, GameObject* gameObject, EventId eventId, short scene, ulong sceneFlags, uint* sceneData, byte sceneDataCount) {
        if (gameObject->BaseId is not 1025176) return;

        switch (scene) {
            case 1:
                Data.AttemptsRemaining = (int)sceneData[1];
                Data.HighestScore = (int)sceneData[0];
                Data.Save();
                break;

            case 2:
                Data.HighestScore = Math.Max((int)sceneData[0], Data.HighestScore);
                Data.Save();
                break;

            case 5:
                Data.AttemptsRemaining = (int)sceneData[0];
                Data.Save();
                break;
        }
    }

    private ReadOnlySeString StatusMessage => Config.CompletionMode switch {
        FashionReportMode.Plus80 => $"{Strings.FashionReport_ScoreTarget}: {Data.HighestScore}/80",
        _ => $"{Data.AttemptsRemaining} {Strings.FashionReport_AttemptsRemaining}",
    };

    protected override void SendStatusMessage(ReadOnlySeString? message) => IChatGui.Get().PrintMessage(
        Config.MessageChatChannel,
        goldSaucerTeleportPayload,
        DisplayName,
        message ?? StatusMessage
    );

    protected override void SendResetMessage(ReadOnlySeString? message) => IChatGui.Get().PrintMessage(
        Config.MessageChatChannel,
        DisplayName,
        message ?? "Module has been reset."
    );

    private static bool IsFashionReportAvailable
        => DateTime.UtcNow > Time.NextWeeklyReset().AddDays(-4) && DateTime.UtcNow < Time.NextWeeklyReset();
}

public class FashionReportData : ModuleDataFile<FashionReportData> {
    protected override string FileName => "FashionReport.data.json";

    public int HighestScore { get; set; }
    public int AttemptsRemaining { get; set; } = 4;
}

public class FashionReportConfig : ModuleConfigFile<FashionReportConfig> {
    protected override string FileName => "FashionReport.config.json";

    public FashionReportMode CompletionMode { get; set; } = FashionReportMode.Single;
}
