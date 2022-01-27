using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheapLoc;
using DailyDuty.ConfigurationSystem;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Util = DailyDuty.System.Utilities.Util;

namespace DailyDuty.System.Modules
{
    internal unsafe class MiniCactpotModule : Module
    {
        private Daily.Cactpot Settings => Service.Configuration.CharacterSettingsMap[Service.Configuration.CurrentCharacter].MiniCactpotSettings;

        private bool exchangeStarted = false;

        public override void UpdateSlow()
        {
            if (GetMiniCactpotPointer() != null)
            {
                if (exchangeStarted == false)
                {
                    exchangeStarted = true;
                }
            }
            else if(exchangeStarted == true)
            {
                exchangeStarted = false;
                Settings.TicketsRemaining -= 1;
                Service.Configuration.Save();
            }
        }

        protected override void OnLoginDelayed()
        {
            if (Settings.Enabled)
            {
                if (Settings.TicketsRemaining > 0)
                {
                    DisplayRemainingAllowances();
                }
            }
        }

        protected override void OnTerritoryChanged(object? sender, ushort e)
        {
            if (Settings.PersistentReminders && Settings.Enabled)
            {
                if (Settings.TicketsRemaining > 0)
                {
                    DisplayRemainingAllowances();
                }
            }
        }

        public override bool IsCompleted()
        {
            return Settings.TicketsRemaining == 0;
        }

        public override void DoDailyReset(Configuration.CharacterSettings settings)
        {
            var miniCactpot = settings.MiniCactpotSettings;

            miniCactpot.TicketsRemaining = 3;
        }

        public override void DoWeeklyReset(Configuration.CharacterSettings settings)
        {
            // Not a Weekly Event
        }

        private AddonLotteryDaily* GetMiniCactpotPointer()
        {
            return (AddonLotteryDaily*) Service.GameGui.GetAddonByName("LotteryDaily", 1);
        }

        private void DisplayRemainingAllowances()
        {
            var locString = Loc.Localize("MiniCactpotAllowances", "You have {0} Mini MiniCactpot Tickets remaining today.");
            Util.PrintMiniCactpot(locString.Format(Settings.TicketsRemaining));
        }

        private DateTime nextClaimDateTime()
        {


            return DateTime.Now;
        }
    }
}
