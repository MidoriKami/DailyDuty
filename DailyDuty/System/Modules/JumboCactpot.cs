using System;
using DailyDuty.Abstracts;
using DailyDuty.Interfaces;
using DailyDuty.Models;
using DailyDuty.Models.Enums;
using DailyDuty.Models.ModuleData;
using DailyDuty.System.Localization;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.Game;
using KamiLib.NativeUi;

namespace DailyDuty.System;

public unsafe class JumboCactpot : Module.SpecialModule, IGoldSaucerMessageReceiver
{
    public override ModuleName ModuleName => ModuleName.JumboCactpot;
    protected override DateTime GetNextReset() => Time.NextJumboCactpotReset();
    
    public override IModuleConfigBase ModuleConfig { get; protected set; } = new JumboCactpotConfig();
    public override IModuleDataBase ModuleData { get; protected set; } = new JumboCactpotData();
    private JumboCactpotConfig Config => ModuleConfig as JumboCactpotConfig ?? new JumboCactpotConfig();
    private JumboCactpotData Data => ModuleData as JumboCactpotData ?? new JumboCactpotData();
    
    public override bool HasClickableLink => true;
    public override PayloadId ClickableLinkPayloadId => PayloadId.GoldSaucerTeleport;

    private Hook<Delegates.AgentReceiveEvent>? onReceiveEventHook;
    private int ticketData = -1;
    
    protected override ModuleStatus GetModuleStatus() => Data.Tickets.Count == 3 ? ModuleStatus.Complete : ModuleStatus.Incomplete;

    public override void Load()
    {
        base.Load();

        onReceiveEventHook ??= Service.Hooker.HookFromAddress<Delegates.AgentReceiveEvent>(new nint(AgentModule.Instance()->GetAgentByInternalId(AgentId.LotteryWeekly)->VTable->ReceiveEvent), OnReceiveEvent);
        onReceiveEventHook?.Enable();
    }

    public override void Unload()
    {
        base.Unload();
        
        onReceiveEventHook?.Disable();
    }

    public override void Dispose()
    {
        onReceiveEventHook?.Dispose();
    }

    public override void Reset()
    {
        Data.Tickets.Clear();
        
        base.Reset();
    }

    protected override StatusMessage GetStatusMessage()
    {
        var message = $"{3 - Data.Tickets.Count} {Strings.TicketsAvailable}";

        return ConditionalStatusMessage.GetMessage(Config.ClickableLink, message, PayloadId.GoldSaucerTeleport);
    }
    
    public void GoldSaucerUpdate(object? sender, GoldSaucerEventArgs data)
    {
        const int jumboCactpotBroker = 1010446;
        if (Service.TargetManager.Target?.DataId != jumboCactpotBroker) return;
        Data.Tickets.Clear();

        for(var i = 0; i < 3; ++i)
        {
            var ticketValue = data.Data[i + 2];

            if (ticketValue != 10000)
            {
                Data.Tickets.Add(ticketValue);
                DataChanged = true;
            }
        }
    }
    
    private nint OnReceiveEvent(AgentInterface* agent, nint rawData, AtkValue* args, uint argCount, ulong sender)
    {
        var result = onReceiveEventHook!.Original(agent, rawData, args, argCount, sender);
        
        Safety.ExecuteSafe(() =>
        {
            var data = args->Int;

            switch (sender)
            {
                // Message is from JumboCactpot
                case 0 when data >= 0:
                    ticketData = data;
                    break;

                // Message is from SelectYesNo
                case 5:
                    switch (data)
                    {
                        case -1:
                        case 1:
                            ticketData = -1;
                            break;

                        case 0 when ticketData >= 0:
                            Data.Tickets.Add(ticketData);
                            ticketData = -1;
                            DataChanged = true;
                            break;
                    }
                    break;
            }
        });

        return result;
    }
}