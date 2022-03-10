using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.Data.Enums;
using DailyDuty.Data.ModuleData.JumboCactpot;
using DailyDuty.Data.SettingsObjects;
using DailyDuty.Data.SettingsObjects.Weekly;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using DailyDuty.Utilities.Helpers.JumboCactpot;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;

namespace DailyDuty.Modules.Weekly
{
    internal unsafe class JumboCactpot : 
        IConfigurable, 
        IUpdateable,
        IResettable,
        IZoneChangeThrottledNotification,
        ILoginNotification,
        ICompletable
    {
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
            if (Condition.IsBoundByDuty()) return;
     
            Notification();
        }

        public void NotificationOptions()
        {
            Draw.OnLoginReminderCheckbox(Settings);

            Draw.OnTerritoryChangeCheckbox(Settings);
        }

        public void EditModeOptions()
        {
            var removeList = new List<TicketData>();

            foreach (var ticket in Settings.CollectedTickets)
            {
                DrawTicketData(ticket);

                if (ImGui.Button($"Remove##{HeaderText}{ticket.DrawingAvailableTime}{ticket.CollectedDate}{ticket.ExpirationDate}"))
                {
                    removeList.Add(ticket);
                }

                ImGui.Separator();
            }

            if (ImGui.Button($"Add New##{HeaderText}"))
            {
                Settings.CollectedTickets.Add(new TicketData
                {
                    DrawingAvailableTime = GetNextReset(),
                    ExpirationDate = GetNextReset().AddDays(7),
                    CollectedDate = DateTime.UtcNow
                });
                Service.Configuration.Save();
            }

            if (removeList.Count > 0)
            {
                foreach (var ticket in removeList)
                {
                    Settings.CollectedTickets.Remove(ticket);
                }

                Service.Configuration.Save();
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

            CollectReward();
        }
    
        public void Dispose()
        {

        }

        DateTime IResettable.GetNextReset()
        {
            return GetNextReset();
        }

        void IResettable.ResetThis()
        {
            Settings.CollectedTickets.RemoveAll(t => DateTime.UtcNow > t.ExpirationDate);
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

        private void UpdatePlayerRegion()
        {
            if (Settings.PlayerRegion != 0) return;

            var region = DatacenterLookup.TryGetPlayerDatacenter();
            if (region != null)
            {
                Settings.PlayerRegion = region.Value;
            }
        }

        private void DrawTicketData(TicketData data)
        {
            ImGui.Text("Collected: " + data.CollectedDate);
            ImGui.Text("Drawing Time: " + data.DrawingAvailableTime);
            ImGui.Text("Expires: " + data.ExpirationDate);
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
}