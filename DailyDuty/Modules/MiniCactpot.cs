using System.Threading.Tasks;
using DailyDuty.Classes;
using DailyDuty.Enums;
using DailyDuty.Interfaces;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using KamiToolKit.Controls.Builders;
using KamiToolKit.UI.Nodes;
using Lumina.Text.ReadOnly;

namespace DailyDuty.Modules;

public class MiniCactpot : Module<MiniCactpotData, MiniCactpotConfig>, IDailyResettable, INpcInteractListener {

    public override string DisplayName => Strings.MiniCactpot_DisplayName;

    public override MiniCactpotData Data { get; protected set; } = null!;
    public override MiniCactpotConfig Config { get; protected set; } = null!;

    private ChatLinkPayload? goldSaucerTeleportPayload;

    private TextNode? attemptsNode;

    public override async Task EnableAsync() {
        Data = await MiniCactpotData.Load();
        Config = await MiniCactpotConfig.Load();

        goldSaucerTeleportPayload = ChatLinkPayload.TeleportToGoldSaucer();

        IAddonLifecycle.Get().RegisterListener(AddonEvent.PreSetup, "LotteryDaily", LotteryDailyPreSetup);
    }

    public override Task DisableAsync() {
        IAddonLifecycle.Get().UnregisterListener(LotteryDailyPreSetup);

        attemptsNode = null;

        goldSaucerTeleportPayload?.Dispose();
        goldSaucerTeleportPayload = null;

        return Task.CompletedTask;
    }

    public override void BuildDataUi(NodeBuilder builder) => builder
        .DataRow(Strings.MiniCactpot_AttemptsAvailable, Strings.MiniCactpot_AttemptsNotUpdated, out attemptsNode);

    public override void UpdateDataUi() {
        attemptsNode?.String = Data.AllowancesRemaining.ToString();
    }

    protected override async Task ResetAsync() {
        Data.AllowancesRemaining = 3;
        await Data.Save();
    }

    protected override CompletionStatus GetCompletionStatus()
        => Data.AllowancesRemaining is 0 ? CompletionStatus.Complete : CompletionStatus.Incomplete;

    protected override void SendStatusMessage(ReadOnlySeString? message) => IChatGui.Get().PrintMessage(
        Config.MessageChatChannel,
        goldSaucerTeleportPayload,
        DisplayName,
        message ?? $"{Data.AllowancesRemaining} {Strings.StatusMessages_TicketRemaining}"
    );

    protected override void SendResetMessage(ReadOnlySeString? message) => IChatGui.Get().PrintMessage(
        Config.MessageChatChannel,
        goldSaucerTeleportPayload,
        DisplayName,
        message ?? "Module has been reset."
    );

    private void LotteryDailyPreSetup(AddonEvent eventType, AddonArgs addonInfo) {
        Data.AllowancesRemaining -= 1;
        Data.Save();
    }

    unsafe void INpcInteractListener.OnNpcInteract(EventFramework* thisPtr, GameObject* gameObject, EventId eventId, short scene, ulong sceneFlags, uint* sceneData, byte sceneDataCount) {
        if (gameObject->BaseId is not 1010445) return;

        if (sceneDataCount is 5) {
            Data.AllowancesRemaining = (int)sceneData[4];
        }
        else {
            Data.AllowancesRemaining = 0;
        }

        Data.Save();
    }
}

public class MiniCactpotData : ModuleDataFile<MiniCactpotData> {
    protected override string FileName => "MiniCactpot.data.json";

    public int AllowancesRemaining = 3;
}

public class MiniCactpotConfig : ModuleConfigFile<MiniCactpotConfig> {
    protected override string FileName => "MiniCactpot.config.json";
}
