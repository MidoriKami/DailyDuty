using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Data.Enums;
using DailyDuty.Data.ModuleData.JumboCactpot;
using DailyDuty.Data.SettingsObjects;
using DailyDuty.Data.SettingsObjects.WeeklySettings;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using DailyDuty.Utilities.Helpers.JumboCactpot;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Interface.Components;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;

namespace DailyDuty.Modules.Weekly;

internal unsafe class JumboCactpot : 
    IConfigurable, 
    IUpdateable,
    IResettable,
    IZoneChangeThrottledNotification,
    ILoginNotification,
    ICompletable
{
    private bool purchaseTicketExchangeStarted;
    private bool collectRewardExchangeStarted;

    private JumboCactpotSettings Settings => Service.Configuration.Current().JumboCactpot;
    public CompletionType Type => CompletionType.Weekly;
    public string HeaderText => "Jumbo Cactpot";
    public GenericSettings GenericSettings => Settings;

    private readonly DalamudLinkPayload goldSaucerTeleport;

    public JumboCactpot()
    {
        goldSaucerTeleport = Service.TeleportManager.GetPayload(TeleportPayloads.GoldSaucerTeleport);
    }

    public DateTime NextReset
    {
        get => Settings.NextReset;
        set => Settings.NextReset = value;
    }

    public bool IsCompleted()
    {
        var ticketsAvailable = GetAvailableTickets();
        var rewardsAvailable = GetAvailableRewards();

        return ticketsAvailable == 0 && rewardsAvailable == 0;
    }

    public void SendNotification()
    {
        if (Settings.Enabled && Condition.IsBoundByDuty() == false)
        {
            Notification();
        }
    }

    public void NotificationOptions()
    {
        Draw.OnLoginReminderCheckbox(Settings, HeaderText);

        Draw.OnTerritoryChangeCheckbox(Settings, HeaderText);
    }

    public void EditModeOptions()
    {
        ImGui.Text("Add ticket to tracking");

        if (ImGui.Button($"Add Ticket##{HeaderText}"))
        {
            if (GetAvailableTickets() > 0)
            {
                AddNewTicket();
                Service.Configuration.Save();
            }
        }
    }

    public void DisplayData()
    {
        if (GetAvailableRewards() > 0)
        {
            Draw.NumericDisplay("Rewards Available", GetAvailableRewards());
        } 
        else if (GetAvailableTickets() > 0)
        {
            Draw.NumericDisplay("Tickets Available", GetAvailableTickets(), Colors.Red);
        }
        else
        {
            Draw.NumericDisplay("Tickets Pending", GetTicketsWaiting(), Colors.Green);
        }

        var timespan = Settings.NextReset - DateTime.UtcNow;
        Draw.TimeSpanDisplay("Next Ticket Drawing", timespan);
    }

    public void Update()
    {
        UpdatePlayerRegion();

        PurchaseTicket();

        CollectReward();
    }
    
    public void Dispose()
    {

    }

    DateTime IResettable.GetNextReset()
    {
        return GetNextReset();
    }

    void IResettable.ResetThis(CharacterSettings settings)
    {
        PurgeExpiredTickets();
    }

    //
    //  Implementation
    //
    private DateTime GetNextReset()
    {
        return DatacenterLookup.GetDrawingTime(Settings.PlayerRegion);
    }

    private int GetAvailableTickets()
    {
        var now = DateTime.UtcNow;

        return 3 - Settings.CollectedTickets.Count(t => now > t.CollectedDate && now < t.DrawingAvailableTime);
    }

    private int GetAvailableRewards()
    {
        var now = DateTime.UtcNow;

        return Settings.CollectedTickets.Count(t => now > t.DrawingAvailableTime && now < t.ExpirationDate);
    }

    private int GetTicketsWaiting()
    {
        var now = DateTime.UtcNow;

        return Settings.CollectedTickets.Count(t => now < t.DrawingAvailableTime);
    }

    private void PurchaseTicket()
    {
        // If the window is open
        if (GetPurchaseTicketWindow() != null)
        {
            purchaseTicketExchangeStarted = true;
        }

        // If the window was previously open
        else if(purchaseTicketExchangeStarted == true)
        {
            purchaseTicketExchangeStarted = false;

            AddNewTicket();

            Service.Configuration.Save();
        }
    }

    private void AddNewTicket()
    {
        Settings.CollectedTickets.Add(new TicketData
        {
            DrawingAvailableTime = GetNextReset(),
            ExpirationDate = GetNextReset().AddDays(7),
            CollectedDate = DateTime.UtcNow
        });
    }

    private void CollectReward()
    {
        // If the payout info window and the MGP Reward Window are open
        if (GetCollectRewardWindow() != null && GetRewardPopupWindow() != null)
        {
            collectRewardExchangeStarted = true;
        }

        // If either of them close, check states
        else if (collectRewardExchangeStarted == true)
        {
            collectRewardExchangeStarted = false;

            var now = DateTime.UtcNow;
            var thisWeeksTickets = Settings.CollectedTickets
                .Where(t => now > t.DrawingAvailableTime)
                .ToList();

            if (thisWeeksTickets.Count > 0)
            {
                Settings.CollectedTickets.Remove(thisWeeksTickets.First());

                Service.Configuration.Save();
            }
        }
    }

    private void PurgeExpiredTickets()
    {
        Settings.CollectedTickets.RemoveAll(t => DateTime.UtcNow > t.ExpirationDate);
    }

    private void UpdatePlayerRegion()
    {
        if (Settings.PlayerRegion != 0) return;

        var region = DatacenterLookup.TryGetPlayerDatacenter();
        if (region != null)
        {
            Settings.PlayerRegion = region.Value;
        }
    }
    private AtkUnitBase* GetPurchaseTicketWindow()
    {
        return (AtkUnitBase*) Service.GameGui.GetAddonByName("LotteryWeeklyInput", 1);
    }

    private AtkUnitBase* GetCollectRewardWindow()
    {
        return (AtkUnitBase*) Service.GameGui.GetAddonByName("LotteryWeeklyRewardList", 1);
    }

    private AtkUnitBase* GetRewardPopupWindow()
    {
        return (AtkUnitBase*) Service.GameGui.GetAddonByName("GoldSaucerReward", 1);
    }

    private void Notification()
    {
        if (GetAvailableTickets() > 0)
        {
            Chat.Print(HeaderText, $"{GetAvailableTickets()} Tickets Available", goldSaucerTeleport);
        }

        if (GetAvailableRewards() > 0)
        {
            Chat.Print(HeaderText, $"{GetAvailableRewards()} Rewards Available", goldSaucerTeleport);
        }
    }
}