using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheapLoc;
using DailyDuty.ConfigurationSystem;
using Dalamud.Logging;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Util = DailyDuty.System.Utilities.Util;

namespace DailyDuty.System.Modules
{
    internal unsafe class JumboCactpotModule : Module
    {
        private Weekly.JumboCactpotSettings Settings => Service.Configuration.CharacterSettingsMap[Service.Configuration.CurrentCharacter].JumboCactpotSettings;
        
        private bool exchangeStarted = false;

        public override void UpdateSlow()
        {
            if (GetJumboCactpotPointer() != null)
            {
                if (exchangeStarted == false)
                {
                    exchangeStarted = true;
                }
            }
            else if (exchangeStarted == true)
            {
                var datacenter = Util.GetPlayerDatacenterID();

                if (datacenter != null)
                {
                    exchangeStarted = false;
                    Settings.UnclaimedTickets -= 1;
                    Settings.ClaimedTickets += 1;
                    Settings.NextDrawing = GetDrawingTimeFromDataCenterID(datacenter.Value);
                    Service.Configuration.Save();
                }
            }
        }


        protected override void OnLoginDelayed()
        {
            if (Settings.Enabled)
            {
                if (Settings.UnclaimedTickets > 0)
                {
                    DisplayRemainingAllowances();
                }
                else if (Settings.ClaimedTickets > 0 && DateTime.UtcNow > Settings.NextDrawing)
                {
                    DisplayDrawingAvailable();
                }
            }
        }

        private void DisplayDrawingAvailable()
        {
            var locString = Loc.Localize("JumboCactpotDrawingAvailable", "You have {0} Jumbo Cactpot Tickets available for rewards!");
            Util.PrintJumboCactpot(locString.Format(Settings.ClaimedTickets));
        }

        private void PrintAvailableRewards()
        {
            var locString = Loc.Localize("JumboCactpotAwardsAvailable", "You have {0} Rewards Unclaimed!");
            Util.PrintJumboCactpot(locString.Format(Settings.ClaimedTickets - Settings.ClaimedRewards));
        }

        protected override void OnTerritoryChanged(object? sender, ushort e)
        {
        }

        public override bool IsCompleted()
        {
            if (TimeUntilNextDrawing() == TimeSpan.Zero && Settings.ClaimedRewards != 3)
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
            var jumboCactpot = settings.JumboCactpotSettings;

            jumboCactpot.ClaimedTickets = 0;
            jumboCactpot.UnclaimedTickets = 3;
            jumboCactpot.NextDrawing = new();
        }

        private AtkUnitBase* GetJumboCactpotPointer()
        {
            return (AtkUnitBase*) Service.GameGui.GetAddonByName("LotteryWeeklyInput", 1);
        }

        private void DisplayRemainingAllowances()
        {
            var locString = Loc.Localize("JumboCactpotAllowances", "You have {0} Unclaimed Jumbo Cactpot Tickets.");
            Util.PrintJumboCactpot(locString.Format(Settings.UnclaimedTickets));
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

            PluginLog.Error("[Util] Unable to determine DataCenter");
            return new();
        }

        private TimeSpan TimeUntilNextDrawing()
        {
            var nextMapTime = Settings.NextDrawing;

            if (DateTime.UtcNow >= nextMapTime)
            {
                return TimeSpan.Zero;
            }
            else
            {
                return nextMapTime - DateTime.UtcNow;
            }
        }
    }
}
