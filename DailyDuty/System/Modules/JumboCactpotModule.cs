using System;
using DailyDuty.ConfigurationSystem;
using DailyDuty.System.Utilities;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Util = DailyDuty.System.Utilities.Util;

namespace DailyDuty.System.Modules
{
    internal unsafe class JumboCactpotModule : Module
    {
        private Weekly.JumboCactpotSettings Settings => Service.Configuration.CharacterSettingsMap[Service.Configuration.CurrentCharacter].JumboCactpotSettings;
        
        private bool buyingTicketExchangeStarted = false;
        private bool claimingRewardExchangeStarted = false;

        public override void Update()
        {
            UpdateDatacenter();

            CheckForWeeklyReset();

            CheckForBuyingTicket();

            CheckForClaimingReward();
        }

        private void UpdateDatacenter()
        {
            var dataCenter = Service.ClientState.LocalPlayer?.HomeWorld.GameData?.DataCenter.Row;
            if (dataCenter == null) return;

            // If datacenter is default, or different than last time
            if (Settings.Datacenter == 0 || Settings.Datacenter != dataCenter.Value)
            {
                Settings.Datacenter = dataCenter.Value;
            }
        }

        private void CheckForBuyingTicket()
        {
            if (GetJumboCactpotPointer() != null)
            {
                if (buyingTicketExchangeStarted == false)
                {
                    buyingTicketExchangeStarted = true;
                }
            }
            else
            {
                if (buyingTicketExchangeStarted == true)
                {
                    buyingTicketExchangeStarted = false;
                    Settings.UnclaimedTickets -= 1;

                    if(Settings.UnclaimedTickets < 0)
                        Settings.UnclaimedTickets = 0;

                    Service.Configuration.Save();
                }
            }
        }

        private void CheckForClaimingReward()
        {
            if (GetRewardWindowPointer() != null)
            {
                if (claimingRewardExchangeStarted == false)
                {
                    claimingRewardExchangeStarted = true;
                }
            }
            else
            {
                if (claimingRewardExchangeStarted == true)
                {
                    claimingRewardExchangeStarted = false;
                    Settings.UnclaimedRewards -= 1;

                    if(Settings.UnclaimedRewards < 0)
                        Settings.UnclaimedRewards = 0;

                    Service.Configuration.Save();
                }
            }
        }

        private void CheckForWeeklyReset()
        {
            if (Service.LoggedIn == true)
            {
                // Is it after the saved reset?
                if (DateTime.UtcNow > Settings.NextDrawing)
                {
                    // Is it more than a week after saved reset?
                    if (DateTime.UtcNow > Settings.NextDrawing.AddDays(7))
                    {
                        foreach (var (_, settings) in Service.Configuration.CharacterSettingsMap)
                        {
                            var jumboCactpotSettings = settings.JumboCactpotSettings;

                            jumboCactpotSettings.NextDrawing = GetDrawingTimeFromDataCenterID(jumboCactpotSettings.Datacenter);
                            jumboCactpotSettings.UnclaimedRewards = 0;
                            jumboCactpotSettings.UnclaimedTickets = 0;
                        }
                    }
                    else
                    {
                        foreach (var (_, settings) in Service.Configuration.CharacterSettingsMap)
                        {
                            var jumboCactpotSettings = settings.JumboCactpotSettings;

                            jumboCactpotSettings.NextDrawing = GetDrawingTimeFromDataCenterID(jumboCactpotSettings.Datacenter);
                            jumboCactpotSettings.UnclaimedRewards = 3 - jumboCactpotSettings.UnclaimedTickets;
                            jumboCactpotSettings.UnclaimedTickets = 0;
                        }
                    }

                    Service.Configuration.Save();
                }
            }
        }

        protected override void OnLoginDelayed()
        {
            if (Settings.Enabled && Settings.LoginReminder)
            {
                if (Settings.UnclaimedTickets > 0)
                {
                    DisplayAllowancesAvailable();
                }
                else if (Settings.UnclaimedRewards > 0)
                {
                    DisplayRewardsAvailable();
                }
            }
        }

        private void DisplayAllowancesAvailable()
        {
            Util.PrintJumboCactpot($"Tickets Remaining: {Settings.UnclaimedTickets}");
        }

        private void DisplayRewardsAvailable()
        {
            Util.PrintJumboCactpot($"Rewards Remaining: {Settings.UnclaimedRewards}");
        }

        protected override void ThrottledOnTerritoryChanged(object? sender, ushort e)
        {
            if (ConditionManager.IsBoundByDuty() == true) return;

            if (Settings.Enabled && Settings.TerritoryChangeReminder)
            {
                if (Settings.UnclaimedTickets > 0)
                {
                    DisplayAllowancesAvailable();
                }
                else if (Settings.UnclaimedRewards > 0)
                {
                    DisplayRewardsAvailable();
                }
            }
        }

        public override bool IsCompleted()
        {
            if (TimeUntilNextDrawing() == TimeSpan.Zero && Settings.UnclaimedRewards != 3)
            {
                return false;
            }
            else if(Settings.UnclaimedTickets > 0)
            {
                return false;
            }

            return true;
        }

        public override void DoDailyReset(Configuration.CharacterSettings settings)
        {
            // Not a daily thing
        }

        public override void DoWeeklyReset(Configuration.CharacterSettings settings)
        {
            // Not Aligned to Weekly Reset
        }

        private AtkUnitBase* GetJumboCactpotPointer()
        {
            return (AtkUnitBase*) Service.GameGui.GetAddonByName("LotteryWeeklyInput", 1);
        }

        private AtkUnitBase* GetRewardWindowPointer()
        {
            return (AtkUnitBase*) Service.GameGui.GetAddonByName("LotteryWeeklyRewardList", 1);
        }
        
        private DateTime GetDrawingTimeFromDataCenterID(uint datacenter)
        {
            switch (datacenter)
            {
                // Elemental
                // Gaia
                // Mana
                case 1:
                case 2:
                case 3:
                    return Util.GetDateOfNextWeekday(DayOfWeek.Saturday).AddHours(12);

                // Aether
                // Primal
                // Crystal
                case 4:
                case 5:
                case 8:
                    return Util.GetDateOfNextWeekday(DayOfWeek.Sunday).AddHours(2);

                // Chaos
                // Light
                case 6:
                case 7:
                    return Util.GetDateOfNextWeekday(DayOfWeek.Saturday).AddHours(19);

                // Materia
                case 9:
                    return Util.GetDateOfNextWeekday(DayOfWeek.Saturday).AddHours(9);
            }

            PluginLog.Error($"[Util] Unable to determine DataCenter: {datacenter}");
            return new();
        }

        private TimeSpan TimeUntilNextDrawing()
        {
            var nextDrawing = Settings.NextDrawing;

            if (DateTime.UtcNow >= nextDrawing)
            {
                return TimeSpan.Zero;
            }
            else
            {
                return nextDrawing - DateTime.UtcNow;
            }
        }
    }
}
