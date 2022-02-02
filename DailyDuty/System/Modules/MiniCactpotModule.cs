using DailyDuty.ConfigurationSystem;
using DailyDuty.System.Utilities;
using FFXIVClientStructs.FFXIV.Client.UI;
using Util = DailyDuty.System.Utilities.Util;

namespace DailyDuty.System.Modules
{
    internal unsafe class MiniCactpotModule : Module
    {
        private Daily.Cactpot Settings => Service.Configuration.CharacterSettingsMap[Service.Configuration.CurrentCharacter].MiniCactpotSettings;

        private bool exchangeStarted = false;

        public override void Update()
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
            if (Settings.Enabled && Settings.LoginReminder)
            {
                if (Settings.TicketsRemaining > 0)
                {
                    DisplayRemainingAllowances();
                }
            }
        }

        protected override void OnTerritoryChanged(object? sender, ushort e)
        {
            if (ConditionManager.IsBoundByDuty()) return;

            if (Settings.Enabled && Settings.TerritoryChangeReminder)
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
            Util.PrintMiniCactpot($"Tickets Remaining: {Settings.TicketsRemaining}");
        }
    }
}
