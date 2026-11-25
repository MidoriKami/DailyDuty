using System.Collections.Generic;
using System.Threading.Tasks;
using DailyDuty.Data;
using DailyDuty.UI;
using DailyDuty.Enums;
using DailyDuty.Interfaces;
using Dalamud.Game.Agent;
using Dalamud.Game.Agent.AgentArgTypes;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using KamiToolKit.Controls.Builders;
using KamiToolKit.UI.Nodes;
using Lumina.Text.ReadOnly;
using AgentId = Dalamud.Game.Agent.AgentId;

namespace DailyDuty.Modules;

public class JumboCactpot : Module<JumboCactpotData, JumboCactpotConfig>, IWeeklyResettable, INpcInteractListener, IClickableTooltip {

    public override string DisplayName => Strings.JumboCactpot_DisplayName;

    public override JumboCactpotData Data { get; protected set; } = null!;
    public override JumboCactpotConfig Config { get; protected set; } = null!;

    private int ticketData = -1;

    private ChatLinkPayload? goldSaucerTeleportPayload;

    private TextNode? ticketNode1;
    private TextNode? ticketNode2;
    private TextNode? ticketNode3;

    public override async Task EnableAsync() {
        Data = await JumboCactpotData.Load();
        Config = await JumboCactpotConfig.Load();

        goldSaucerTeleportPayload = ChatLinkPayload.TeleportToGoldSaucer();

        IAgentLifecycle.Get().RegisterListener(AgentEvent.PreReceiveEvent, AgentId.LotteryWeekly, LotteryWeeklyReceiveEvent);
    }

    public override Task DisableAsync() {
        IAgentLifecycle.Get().UnregisterListener(LotteryWeeklyReceiveEvent);

        ticketNode1 = null;
        ticketNode2 = null;
        ticketNode3 = null;

        goldSaucerTeleportPayload?.Dispose();
        goldSaucerTeleportPayload = null;

        return Task.CompletedTask;
    }

    public override void BuildDataUi(NodeBuilder builder) => builder
        .DataRow($"{Strings.JumboCactpot_Ticket} #1", Strings.JumboCactpot_TicketNotUpdated, out ticketNode1)
        .DataRow($"{Strings.JumboCactpot_Ticket} #2", Strings.JumboCactpot_TicketNotUpdated, out ticketNode2)
        .DataRow($"{Strings.JumboCactpot_Ticket} #3", Strings.JumboCactpot_TicketNotUpdated, out ticketNode3);

    public override void UpdateDataUi() {
        ticketNode1?.String = Data.Tickets.Count > 0 ? Data.Tickets[0].ToString() : Strings.JumboCactpot_TicketNotClaimed;
        ticketNode2?.String = Data.Tickets.Count > 1 ? Data.Tickets[1].ToString() : Strings.JumboCactpot_TicketNotClaimed;
        ticketNode3?.String = Data.Tickets.Count > 2 ? Data.Tickets[2].ToString() : Strings.JumboCactpot_TicketNotClaimed;
    }

    public ReadOnlySeString GetTooltip()
        => $"{3 - Data.Tickets.Count} {Strings.JumboCactpot_StatusMessageRemaining}";

    public void OnTooltipClicked()
        => goldSaucerTeleportPayload?.ClickAction();

    protected override async Task ResetAsync() {
        Data.Tickets.Clear();
        await Data.Save();
    }

    protected override CompletionStatus GetCompletionStatus()
        => Data.Tickets.Count is 3 ? CompletionStatus.Complete : CompletionStatus.Incomplete;

    protected override void SendStatusMessage(ReadOnlySeString? message) => IChatGui.Get().PrintMessage(
        Config.MessageChatChannel,
        goldSaucerTeleportPayload,
        DisplayName,
        message ?? $"{3 - Data.Tickets.Count} {Strings.JumboCactpot_StatusMessageRemaining}"
    );

    protected override void SendResetMessage(ReadOnlySeString? message) => IChatGui.Get().PrintMessage(
        Config.MessageChatChannel,
        goldSaucerTeleportPayload,
        DisplayName,
        message ?? Strings.JumboCactpot_StatusMessageReset
    );

    unsafe void INpcInteractListener.OnNpcInteract(EventFramework* thisPtr, GameObject* gameObject, EventId eventId, short scene, ulong sceneFlags, uint* sceneData, byte sceneDataCount) {
        if (gameObject->BaseId is not 1010446) return;

        Data.Tickets.Clear();

        for (var i = 0; i < 3; i++) {
            var ticketValue = sceneData[i + 2];
            if (ticketValue != 10000) {
                Data.Tickets.Add((int)ticketValue);
            }
        }

        Data.Save();
    }

    private void LotteryWeeklyReceiveEvent(AgentEvent type, AgentArgs args) {
        if (args is not AgentReceiveEventArgs eventArgs) return;
        if (!eventArgs.AtkValueSpan[0].TryGet(out int data)) return;

        var sender = eventArgs.EventKind;

        switch (sender) {
            case 0 when data >= 0:
                ticketData = data;
                break;

            case 5:
                switch (data) {
                    case -1:
                    case 1:
                        ticketData = -1;
                        break;

                    case 0 when ticketData >= 0:
                        Data.Tickets.Add(ticketData);
                        ticketData = -1;
                        Data.Save();
                        break;
                }
                break;
        }
    }
}

public class JumboCactpotData : ModuleDataFile<JumboCactpotData> {
    protected override string FileName => "JumboCactpot.data.json";

    public List<int> Tickets { get; set; } = [];
}

public class JumboCactpotConfig : ModuleConfigFile<JumboCactpotConfig> {
    protected override string FileName => "JumboCactpot.config.json";
}
