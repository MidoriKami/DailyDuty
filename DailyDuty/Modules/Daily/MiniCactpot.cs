using System;
using DailyDuty.Data.Enums;
using DailyDuty.Data.SettingsObjects;
using DailyDuty.Data.SettingsObjects.Daily;
using DailyDuty.Interfaces;
using DailyDuty.Modules.Weekly;
using DailyDuty.Utilities;
using DailyDuty.Utilities.Helpers.Delegates;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace DailyDuty.Modules.Daily
{
    internal unsafe class MiniCactpot : 
        IConfigurable,
        IDailyResettable,
        IZoneChangeThrottledNotification,
        ILoginNotification,
        ICompletable
    {
        private MiniCactpotSettings Settings => Service.Configuration.Current().MiniCactpot;
        public GenericSettings GenericSettings => Settings;
        public CompletionType Type => CompletionType.Daily;
        public string HeaderText => "Mini Cactpot";

        private readonly DalamudLinkPayload goldSaucerTeleport;

        // LotteryDaily_Show
        [Signature("40 53 57 41 55 48 81 EC ?? ?? ?? ?? 48 8B 05", DetourName = nameof(LotteryDaily_Show))]
        private readonly Hook<Functions.Agent.LotteryDaily.Show>? receiveEventHook = null;

        public void* LotteryDaily_Show(AgentInterface* addon, void* a2, void* a3)
        {
            Settings.TicketsRemaining -= 1;

            return receiveEventHook!.Original(addon, a2, a3);
        }

        public MiniCactpot()
        {
            SignatureHelper.Initialise(this);

            receiveEventHook?.Enable();

            goldSaucerTeleport = Service.TeleportManager.GetPayload(TeleportPayloads.GoldSaucerTeleport);
        }

        public void Dispose()
        {
            receiveEventHook?.Dispose();
        }

        public DateTime NextReset
        {
            get => Settings.NextReset;
            set => Settings.NextReset = value;
        }
        
        public void NotificationOptions()
        {
            Draw.OnLoginReminderCheckbox(Settings);

            Draw.OnTerritoryChangeCheckbox(Settings);
        }

        public void EditModeOptions()
        {
            Draw.EditNumberField("Override Ticket Count", ref Settings.TicketsRemaining);
        }

        public void DisplayData()
        {
            Draw.NumericDisplay("Tickets Remaining", Settings.TicketsRemaining);
        }
        
        public bool IsCompleted()
        {
            return Settings.TicketsRemaining == 0;
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
    }
}