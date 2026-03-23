using System;
using DailyDuty.Classes;
using DailyDuty.CustomNodes;
using DailyDuty.Enums;
using DailyDuty.Utilities;
using Dalamud.Game.Agent;
using Dalamud.Game.Agent.AgentArgTypes;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.Object;

namespace DailyDuty.Features.JumboCactpot;

public unsafe class JumboCactpot : Module<ConfigBase, JumboCactpotData> {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Jumbo Cactpot",
        FileName = "JumboCactpot",
        Type = ModuleType.Weekly,
        ChangeLog = [
            new ChangeLogInfo(1, "Initial Re-Implementation"),
        ],
        Tags = [ "MGP" ],
    };

    public override DataNodeBase DataNode => new JumboCactpotDataNode(this);
    private int ticketData = -1;

    protected override void OnModuleEnable() {
        Services.AgentLifecycle.RegisterListener(AgentEvent.PreReceiveEvent, AgentId.LotteryWeekly, OnLotteryEvent);
    }

    protected override void OnModuleDisable() {
        Services.AgentLifecycle.UnregisterListener(OnLotteryEvent);
    }

    protected override StatusMessage GetStatusMessage() => new() {
        Message = $"{3 - ModuleData.Tickets.Count} Tickets Available",
        PayloadId = PayloadId.GoldSaucerTeleport,
    };

    public override DateTime GetNextResetDateTime()
        => Time.NextJumboCactpotReset();

    public override TimeSpan GetResetPeriod()
        => TimeSpan.FromDays(7);

    public override void Reset() {
        ModuleData.Tickets.Clear();
    }

    protected override CompletionStatus GetCompletionStatus()
        => ModuleData.Tickets.Count is 3 ? CompletionStatus.Complete : CompletionStatus.Incomplete;

    public override void OnNpcInteract(EventFramework* thisPtr, GameObject* gameObject, EventId eventId, short scene, ulong sceneFlags, uint* sceneData, byte sceneDataCount) {
        if (gameObject->BaseId is not 1010446) return;
        
        ModuleData.Tickets.Clear();

        for(var i = 0; i < 3; ++i) {
            var ticketValue = sceneData[i + 2];

            if (ticketValue != 10000) {
                ModuleData.Tickets.Add((int)ticketValue); 
                ModuleData.MarkDirty();
            }
        }
    }
    
    private void OnLotteryEvent(AgentEvent type, AgentArgs args) {
        if (args is not AgentReceiveEventArgs receiveArgs) return;

        var data = receiveArgs.AtkValueSpan[0].Int;

        switch (receiveArgs.EventKind) {
            // Message is from JumboCactpot
            case 0 when data >= 0:
                ticketData = data;
                break;

            // Message is from SelectYesNo
            case 5:
                switch (data) {
                    case -1:
                    case 1:
                        ticketData = -1;
                        break;

                    case 0 when ticketData >= 0:
                        ModuleData.Tickets.Add(ticketData);
                        ticketData = -1;
                        ModuleData.MarkDirty();
                        break;
                }
                break;
        }
    }
}
