using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.Addons;
using DailyDuty.DataModels;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.UserInterface.Components;
using DailyDuty.Utilities;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using KamiLib.Configuration;
using KamiLib.InfoBoxSystem;
using KamiLib.Interfaces;
using KamiLib.Utilities;

namespace DailyDuty.Modules;

public class JumboCactpotSettings : GenericSettings
{
    public List<int> Tickets = new();
    public Setting<bool> EnableClickableLink = new(false);
}

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

        public ModuleConfigurationComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public void Draw()
        {
            InfoBox.Instance.DrawGenericSettings(this);

            InfoBox.Instance
                .AddTitle(Strings.Common_ClickableLink)
                .AddString(Strings.GoldSaucer_ClickableLink)
                .AddConfigCheckbox(Strings.Common_Enabled, Settings.EnableClickableLink)
                .Draw();

            InfoBox.Instance.DrawNotificationOptions(this);
        }
    }

    private class ModuleStatusComponent : IStatusComponent
    {
        public IModule ParentModule { get; }

        public ISelectable Selectable => new StatusSelectable(ParentModule, this, ParentModule.LogicComponent.Status);

        public ModuleStatusComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public void Draw()
        {
            if (ParentModule.LogicComponent is not ModuleLogicComponent logicModule) return;

            var moduleStatus = logicModule.GetModuleStatus();

            InfoBox.Instance
                .AddTitle(Strings.Status_Label)
                .BeginTable()
                .BeginRow()
                .AddString(Strings.Status_ModuleStatus)
                .AddString(moduleStatus.GetTranslatedString(), moduleStatus.GetStatusColor())
                .EndRow()
                .BeginRow()
                .AddString(Strings.JumboCactpot_Tickets)
                .AddString(Settings.Tickets.Count == 0 ? Strings.JumboCactpot_NoTickets : ModuleLogicComponent.GetTicketsString())
                .EndRow()
                .EndTable()
                .Draw();

            InfoBox.Instance
                .AddTitle(Strings.JumboCactpot_NextDrawing)
                .BeginTable()
                .BeginRow()
                .AddString(Strings.JumboCactpot_NextDrawing)
                .AddString(ModuleLogicComponent.GetNextJumboCactpot())
                .EndRow()
                .EndTable()
                .Draw();
            
            InfoBox.Instance.DrawSuppressionOption(this);
        }
    }

    private unsafe class ModuleLogicComponent : ILogicComponent
    {
        public IModule ParentModule { get; }
        public DalamudLinkPayload? DalamudLinkPayload { get; } 
        public bool LinkPayloadActive => Settings.EnableClickableLink;

        private int ticketData = -1;

        public ModuleLogicComponent(IModule parentModule)
        {
            ParentModule = parentModule;

            DalamudLinkPayload = TeleportManager.Instance.GetPayload(TeleportLocation.GoldSaucer);

            GoldSaucerAddon.Instance.GoldSaucerUpdate += OnGoldSaucerUpdate;
            LotteryWeeklyAddon.Instance.ReceiveEvent += OnReceiveEvent;
        }

        public void Dispose()
        {
            GoldSaucerAddon.Instance.GoldSaucerUpdate -= OnGoldSaucerUpdate;
            LotteryWeeklyAddon.Instance.ReceiveEvent -= OnReceiveEvent;
        }

        public string GetStatusMessage() => $"{3 - Settings.Tickets.Count} {Strings.JumboCactpot_TicketsAvailable}";

        public DateTime GetNextReset() => Time.NextJumboCactpotReset();

        public void DoReset() => Settings.Tickets.Clear();

        public ModuleStatus GetModuleStatus() => Settings.Tickets.Count == 3 ? ModuleStatus.Complete : ModuleStatus.Incomplete;

        public static string GetTicketsString() => string.Join(" ", Settings.Tickets.Select(num => string.Format($"[{num:D4}]")));

        private void OnReceiveEvent(object? sender, ReceiveEventArgs e)
        {
            var data = e.EventArgs->Int;

            switch (e.SenderID)
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

        private void OnGoldSaucerUpdate(object? sender, GoldSaucerEventArgs e)
        {
            //1010446 Jumbo Cactpot Broker
            if (Service.TargetManager.Target?.DataId != 1010446) return;
            Settings.Tickets.Clear();

            for(var i = 0; i < 3; ++i)
            {
                var ticketValue = e.Data[i + 2];

                if (ticketValue != 10000)
                {
                    Settings.Tickets.Add(ticketValue);
                }
            }

            Service.ConfigurationManager.Save();
        }

        public static string GetNextJumboCactpot()
        {
            var span = Time.NextJumboCactpotReset() - DateTime.UtcNow;

            return span.FormatTimespan(Settings.TimerSettings.TimerStyle.Value);
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

        public string GetShortTaskLabel() => Strings.JumboCactpot_Label;

        public string GetLongTaskLabel() => Strings.JumboCactpot_Label;
    }


    private class ModuleTimerComponent : ITimerComponent
    {
        public IModule ParentModule { get; }

        public ModuleTimerComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public TimeSpan GetTimerPeriod() => TimeSpan.FromDays(7);

        public DateTime GetNextReset() => Time.NextJumboCactpotReset();
    }
}