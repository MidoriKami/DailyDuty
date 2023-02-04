using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.Addons;
using DailyDuty.DataModels;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.Utilities;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using KamiLib.Configuration;
using KamiLib.Drawing;
using KamiLib.Misc;
using KamiLib.Teleporter;

namespace DailyDuty.Modules;

public class JumboCactpotSettings : GenericSettings
{
    public List<int> Tickets = new();
    public Setting<bool> EnableClickableLink = new(false);
}

public unsafe class JumboCactpot : AbstractModule
{
    public override ModuleName Name => ModuleName.JumboCactpot;
    public override CompletionType CompletionType => CompletionType.Weekly;

    private static JumboCactpotSettings Settings => Service.ConfigurationManager.CharacterConfiguration.JumboCactpot;
    public override GenericSettings GenericSettings => Settings;
    public override DalamudLinkPayload DalamudLinkPayload => TeleportManager.Instance.GetPayload(TeleportLocation.GoldSaucer); 
    public override bool LinkPayloadActive => Settings.EnableClickableLink;

    private int ticketData = -1;
    
    public JumboCactpot()
    {
        AgentGoldSaucer.Instance.GoldSaucerUpdate += OnGoldSaucerUpdate;
        AddonLotteryWeekly.Instance.ReceiveEvent += OnReceiveEvent;
    }

    public override void Dispose()
    {
        AgentGoldSaucer.Instance.GoldSaucerUpdate -= OnGoldSaucerUpdate;
        AddonLotteryWeekly.Instance.ReceiveEvent -= OnReceiveEvent;
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
    
    public override string GetStatusMessage() => $"{3 - Settings.Tickets.Count} {Strings.JumboCactpot_TicketsAvailable}";
    protected override DateTime GetModuleReset() => Time.NextJumboCactpotReset();
    public override void DoReset() => Settings.Tickets.Clear();
    public override ModuleStatus GetModuleStatus() => Settings.Tickets.Count == 3 ? ModuleStatus.Complete : ModuleStatus.Incomplete;
    private static string GetTicketsString() => string.Join(" ", Settings.Tickets.Select(num => string.Format($"[{num:D4}]")));

    private static string GetNextJumboCactpot()
    {
        var span = Time.NextJumboCactpotReset() - DateTime.UtcNow;

        return span.FormatTimespan(Settings.TimerSettings.TimerStyle.Value);
    }
    
    protected override void DrawConfiguration()
    {
        InfoBox.Instance.DrawGenericSettings(this);

        InfoBox.Instance
            .AddTitle(Strings.Common_ClickableLink)
            .AddString(Strings.GoldSaucer_ClickableLink)
            .AddConfigCheckbox(Strings.Common_Enabled, Settings.EnableClickableLink)
            .Draw();

        InfoBox.Instance.DrawNotificationOptions(this);
    }

    protected override void DrawStatus()
    {
        InfoBox.Instance.DrawGenericStatus(this);
            
        InfoBox.Instance
            .AddTitle(Strings.Status_ModuleData)
            .BeginTable()
            .BeginRow()
            .AddString(Strings.JumboCactpot_Tickets)
            .AddString(Settings.Tickets.Count == 0 ? Strings.JumboCactpot_NoTickets : GetTicketsString())
            .EndRow()
            .EndTable()
            .Draw();

        InfoBox.Instance
            .AddTitle(Strings.JumboCactpot_NextDrawing)
            .BeginTable()
            .BeginRow()
            .AddString(Strings.JumboCactpot_NextDrawing)
            .AddString(GetNextJumboCactpot())
            .EndRow()
            .EndTable()
            .Draw();
            
        InfoBox.Instance.DrawSuppressionOption(this);
    }
}