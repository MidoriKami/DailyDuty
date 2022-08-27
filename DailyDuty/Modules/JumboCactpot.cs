using System;
using System.Linq;
using DailyDuty.Configuration.Components;
using DailyDuty.Configuration.Enums;
using DailyDuty.Configuration.ModuleSettings;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.System;
using DailyDuty.UserInterface.Components;
using DailyDuty.UserInterface.Components.InfoBox;
using DailyDuty.UserInterface.Windows;
using DailyDuty.Utilities;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace DailyDuty.Modules;

internal class JumboCactpot : IModule
{
    public ModuleName Name => ModuleName.JumboCactpot;
    public IConfigurationComponent ConfigurationComponent { get; }
    public IStatusComponent StatusComponent { get; }
    public ILogicComponent LogicComponent { get; }
    public ITodoComponent TodoComponent { get; }
    public ITimerComponent TimerComponent { get; }

    private static JumboCactpotSettings Settings => Service.ConfigurationManager.CharacterConfiguration.JumboCactpot;
    public GenericSettings GenericSettings => Settings;

    public JumboCactpot()
    {
        ConfigurationComponent = new ModuleConfigurationComponent(this);
        StatusComponent = new ModuleStatusComponent(this);
        LogicComponent = new ModuleLogicComponent(this);
        TodoComponent = new ModuleTodoComponent(this);
        TimerComponent = new ModuleTimerComponent(this);
    }

    public void Dispose()
    {
        LogicComponent.Dispose();
    }

    private class ModuleConfigurationComponent : IConfigurationComponent
    {
        public IModule ParentModule { get; }
        public ISelectable Selectable => new ConfigurationSelectable(ParentModule, this);

        private readonly InfoBox optionsInfoBox = new();
        private readonly InfoBox clickableLink = new();
        private readonly InfoBox notificationOptionsInfoBox = new();

        public ModuleConfigurationComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public void Draw()
        {
            optionsInfoBox
                .AddTitle(Strings.Configuration.Options)
                .AddConfigCheckbox(Strings.Common.Enabled, Settings.Enabled)
                .Draw();

            clickableLink
                .AddTitle(Strings.Module.JumboCactpot.ClickableLinkLabel)
                .AddString(Strings.Module.JumboCactpot.ClickableLink)
                .AddConfigCheckbox(Strings.Common.Enabled, Settings.EnableClickableLink)
                .Draw();

            notificationOptionsInfoBox
                .AddTitle(Strings.Configuration.NotificationOptions)
                .AddConfigCheckbox(Strings.Configuration.OnLogin, Settings.NotifyOnLogin)
                .AddConfigCheckbox(Strings.Configuration.OnZoneChange, Settings.NotifyOnZoneChange)
                .Draw();
        }
    }

    private class ModuleStatusComponent : IStatusComponent
    {
        public IModule ParentModule { get; }

        public ISelectable Selectable => new StatusSelectable(ParentModule, this, ParentModule.LogicComponent.GetModuleStatus);

        private readonly InfoBox status = new();
        private readonly InfoBox nextDrawing = new();

        public ModuleStatusComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public void Draw()
        {
            if (ParentModule.LogicComponent is not ModuleLogicComponent logicModule) return;

            var moduleStatus = logicModule.GetModuleStatus();


            status
                .AddTitle(Strings.Status.Label)
                .BeginTable()

                .AddRow(
                    Strings.Status.ModuleStatus,
                    moduleStatus.GetTranslatedString(),
                    secondColor: moduleStatus.GetStatusColor())

                .AddRow(Strings.Module.JumboCactpot.Tickets,
                    Settings.Tickets.Count == 0 ? Strings.Module.JumboCactpot.NoTickets : logicModule.GetTicketsString()
                )

                .EndTable()
                .Draw();

            nextDrawing
                .AddTitle(Strings.Module.JumboCactpot.NextDrawing)
                .BeginTable()
                .AddRow(
                    Strings.Module.JumboCactpot.NextDrawing,
                    logicModule.GetNextJumboCactpot()
                    )
                .EndTable()
                .Draw();
        }
    }

    private unsafe class ModuleLogicComponent : ILogicComponent
    {
        public IModule ParentModule { get; }
        public DalamudLinkPayload? DalamudLinkPayload { get; } = Service.TeleportManager.GetPayload(TeleportLocation.GoldSaucer);

        private delegate void* AddonReceiveEvent(AgentInterface* addon, void* a2, AtkValue* eventData, int eventDataItemCount, int senderID);

        [Signature("48 89 5C 24 ?? 44 89 4C 24 ?? 4C 89 44 24 ?? 55 56 57 41 54 41 55 41 56 41 57 48 8D 6C 24", DetourName = nameof(LotteryWeekly_ReceiveEvent))]
        private readonly Hook<AddonReceiveEvent>? receiveEventHook = null;

        private delegate void* GoldSaucerUpdateDelegate(void* a1, byte* a2, uint a3, ushort a4, void* a5, int* a6, byte a7);

        [Signature("E8 ?? ?? ?? ?? 80 A7 ?? ?? ?? ?? ?? 48 8D 8F ?? ?? ?? ?? 44 89 AF", DetourName = nameof(GoldSaucerUpdate))]
        private readonly Hook<GoldSaucerUpdateDelegate>? goldSaucerUpdateHook = null;

        private int ticketData = -1;

        public ModuleLogicComponent(IModule parentModule)
        {
            ParentModule = parentModule;

            SignatureHelper.Initialise(this);

            receiveEventHook?.Enable();
            goldSaucerUpdateHook?.Enable();
        }

        public void Dispose()
        {
            receiveEventHook?.Dispose();
            goldSaucerUpdateHook?.Dispose();
        }

        public string GetStatusMessage() => $"{3 - Settings.Tickets.Count} {Strings.Module.JumboCactpot.TicketsAvailable}";

        public DateTime GetNextReset() => Time.NextWeeklyReset();

        public void DoReset() => Settings.Tickets.Clear();

        public ModuleStatus GetModuleStatus() => Settings.Tickets.Count == 3 ? ModuleStatus.Complete : ModuleStatus.Incomplete;

        public string GetTicketsString()
        {
            return string.Join(" ", Settings.Tickets.Select(num => string.Format($"[{num:D4}]")));
        }

        private void* GoldSaucerUpdate(void* a1, byte* a2, uint a3, ushort a4, void* a5, int* a6,  byte a7)
        {
            try
            {
                //1010446 Jumbo Cactpot Broker
                if (Service.TargetManager.Target?.DataId == 1010446)
                {
                    Settings.Tickets.Clear();

                    for(var i = 0; i < 3; ++i)
                    {
                        var ticketValue = a6[i + 2];

                        if (ticketValue != 10000)
                        {
                            if (!Settings.Tickets.Contains(ticketValue))
                            {
                                Settings.Tickets.Add(ticketValue);
                            }
                        }
                    }

                    Service.ConfigurationManager.Save();
                }
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "[Jumbo Cactpot]  Unable to get data from Gold Saucer Update");
            }

            return goldSaucerUpdateHook!.Original(a1, a2, a3, a4, a5, a6, a7);
        }

        private void* LotteryWeekly_ReceiveEvent(AgentInterface* agent, void* a2, AtkValue* eventData, int eventDataItemCount, int senderID)
        {
            try
            {
                var data = eventData->Int;

                switch (senderID)
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
                                Settings.Tickets.Add(ticketData);
                                ticketData = -1;
                                Service.ConfigurationManager.Save();
                                break;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "Unable to retrieve data from Jumbo Cactpot event");
            }

            return receiveEventHook!.Original(agent, a2, eventData, eventDataItemCount, senderID);
        }

        public string GetNextJumboCactpot()
        {
            var span = Time.NextJumboCactpotReset() - DateTime.UtcNow;

            return TimersOverlayWindow.FormatTimespan(span, TimerStyle.Full);
        }
    }

    private class ModuleTodoComponent : ITodoComponent
    {
        public IModule ParentModule { get; }
        public CompletionType CompletionType => CompletionType.Weekly;
        public bool HasLongLabel => false;

        public ModuleTodoComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public string GetShortTaskLabel() => Strings.Module.JumboCactpot.Label;

        public string GetLongTaskLabel() => Strings.Module.JumboCactpot.Label;
    }


    private class ModuleTimerComponent : ITimerComponent
    {
        public IModule ParentModule { get; }

        public ModuleTimerComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public TimeSpan GetTimerPeriod() => TimeSpan.FromDays(7);

        public DateTime GetNextReset() => Time.NextWeeklyReset();
    }
}