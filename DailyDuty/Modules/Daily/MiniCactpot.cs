using System;
using DailyDuty.Data.Enums;
using DailyDuty.Data.SettingsObjects;
using DailyDuty.Data.SettingsObjects.Daily;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using FFXIVClientStructs.FFXIV.Client.UI;

namespace DailyDuty.Modules.Daily
{
    internal unsafe class MiniCactpot : 
        IConfigurable,
        IUpdateable,
        IDailyResettable,
        IZoneChangeThrottledNotification,
        ILoginNotification,
        ICompletable
    {
        private MiniCactpotSettings Settings => Service.Configuration.Current().MiniCactpot;
        public GenericSettings GenericSettings => Settings;
        public CompletionType Type => CompletionType.Daily;
        public string HeaderText => "Mini Cactpot";

        private bool exchangeStarted;

        private readonly DalamudLinkPayload goldSaucerTeleport;

        public MiniCactpot()
        {
            goldSaucerTeleport = Service.TeleportManager.GetPayload(TeleportPayloads.GoldSaucerTeleport);

        }

        public DateTime NextReset
        {
            get => Settings.NextReset;
            set => Settings.NextReset = value;
        }
        
        public void NotificationOptions()
        {
            Draw.OnLoginReminderCheckbox(Settings, HeaderText);

            Draw.OnTerritoryChangeCheckbox(Settings, HeaderText);
        }

        public void EditModeOptions()
        {
            Draw.EditNumberField("Override Ticket Count", HeaderText, ref Settings.TicketsRemaining);
        }

        public void DisplayData()
        {
            Draw.NumericDisplay("Tickets Remaining", Settings.TicketsRemaining);
        }

        public void Dispose()
        {
        }

        public bool IsCompleted()
        {
            return Settings.TicketsRemaining == 0;
        }

        public void Update()
        {
            UpdateMiniCactpot();
        }

        void IResettable.ResetThis()
        {
            Settings.TicketsRemaining = 3;
        }

        public void SendNotification()
        {
            if (Settings.TicketsRemaining > 0 && Condition.IsBoundByDuty() == false)
            {
                Chat.Print(HeaderText, $"{Settings.TicketsRemaining} Tickets Remaining", goldSaucerTeleport);
            }
        }

        //
        // Implementation
        //
        private void UpdateMiniCactpot()
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
        private AddonLotteryDaily* GetMiniCactpotPointer()
        {
            return (AddonLotteryDaily*) Service.GameGui.GetAddonByName("LotteryDaily", 1);
        }
    }
}