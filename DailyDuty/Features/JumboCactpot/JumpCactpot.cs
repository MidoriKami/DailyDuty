using System;
using DailyDuty.Classes;
using DailyDuty.Classes.Nodes;
using DailyDuty.Enums;
using DailyDuty.Utilities;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Text.ReadOnly;

namespace DailyDuty.Features.JumboCactpot;

public unsafe class JumpCactpot : Module<ConfigBase, Data> {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Jumbo Cactpot",
        FileName = "JumboCactpot",
        Type = ModuleType.Weekly,
        ChangeLog = [
            new ChangeLogInfo(1, "Initial Re-Implementation"),
        ],
        Tags = [ "MGP" ],
        MessageClickAction = PayloadId.GoldSaucerTeleport,
    };

    public override DataNodeBase DataNode => new DataNode(this);
    private Hook<AgentInterface.Delegates.ReceiveEvent>? onReceiveEventHook;
    private Hook<EventFramework.Delegates.ProcessEventPlay>? frameworkEventHook;
    private int ticketData = -1;

    protected override void OnEnable() {
        onReceiveEventHook = Services.Hooker.HookFromAddress<AgentInterface.Delegates.ReceiveEvent>(AgentModule.Instance()->GetAgentByInternalId(AgentId.LotteryWeekly)->VirtualTable->ReceiveEvent, OnReceiveEvent);
        onReceiveEventHook?.Enable();
        
        frameworkEventHook = Services.Hooker.HookFromAddress<EventFramework.Delegates.ProcessEventPlay>(EventFramework.MemberFunctionPointers.ProcessEventPlay, OnFrameworkEvent);
        frameworkEventHook?.Enable();
    }

    protected override void OnDisable() {
        onReceiveEventHook?.Dispose();
        onReceiveEventHook = null;
        
        frameworkEventHook?.Dispose();
        frameworkEventHook = null;
    }

    protected override ReadOnlySeString GetStatusMessage()
        => $"{3 - ModuleData.Tickets.Count} Tickets Available";

    public override DateTime GetNextResetDateTime()
        => Time.NextJumboCactpotReset();

    public override TimeSpan GetResetPeriod()
        => TimeSpan.FromDays(7);

    public override void Reset() {
        ModuleData.Tickets.Clear();
    }

    protected override CompletionStatus GetCompletionStatus()
        => ModuleData.Tickets.Count is 3 ? CompletionStatus.Complete : CompletionStatus.Incomplete;
    
    private void OnFrameworkEvent(EventFramework* thisPtr, GameObject* gameObject, EventId eventId, short scene, ulong sceneFlags, uint* sceneData, byte sceneDataCount) {
        frameworkEventHook!.Original(thisPtr, gameObject, eventId, scene, sceneFlags, sceneData, sceneDataCount);
        
        if (gameObject->BaseId is not 1010446) return;
        
        // Services.PluginLog.Debug($"[FrameworkEvent] Scene: {scene}, Flags: {sceneFlags}, EventId: {eventId.ContentId}-{eventId.EntryId}-{eventId.EntryId}");
        // Services.PluginLog.Debug($"[FrameworkEvent] DataCount: {sceneDataCount} Data: {string.Join(", ", Enumerable.Range(0, sceneDataCount).Select(index => sceneData[index]))}");
        
        ModuleData.Tickets.Clear();

        for(var i = 0; i < 3; ++i) {
        	var ticketValue = sceneData[i + 2];

        	if (ticketValue != 10000) {
                ModuleData.Tickets.Add((int)ticketValue); 
                ModuleData.MarkDirty();
            }
        }
    }
    
    private AtkValue* OnReceiveEvent(AgentInterface* agent, AtkValue* returnValue, AtkValue* args, uint argCount, ulong sender) {
    	var result = onReceiveEventHook!.Original(agent, returnValue, args, argCount, sender);

        try {
            var data = args->Int;

            switch (sender) {
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
        catch (Exception e) {
            Services.PluginLog.Error(e, "Exception processing JumboCactpot Event");
        }

        return result;
    }
}
