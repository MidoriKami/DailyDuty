using System;
using DailyDuty.Data.Components;
using DailyDuty.Data.ModuleSettings;
using DailyDuty.Enums;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.Utilities;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace DailyDuty.Modules
{
    internal unsafe class MiniCactpotModule :
        IDailyResettable,
        IZoneChangeThrottledNotification,
        ILoginNotification,
        ICompletable,
        IDisposable
    {
        public GenericSettings GenericSettings => Settings;
        public CompletionType Type => CompletionType.Daily;
        private static MiniCactpotSettings Settings => Service.CharacterConfiguration.MiniCactpot;
        public string DisplayName => Strings.Module.MiniCactpotLabel;
        public Action? ExpandedDisplay => null;

        private delegate void* AgentShow(AgentInterface* addon, void* a2, void* a3);

        // LotteryDaily_Show
        [Signature("40 53 57 41 55 48 81 EC ?? ?? ?? ?? 48 8B 05", DetourName = nameof(LotteryDaily_Show))]
        private readonly Hook<AgentShow>? receiveEventHook = null;

        private readonly DalamudLinkPayload goldSaucerTeleport;

        public MiniCactpotModule()
        {
            SignatureHelper.Initialise(this);

            receiveEventHook?.Enable();

            goldSaucerTeleport = Service.TeleportManager.GetPayload(ChatPayloads.GoldSaucerTeleport);
        }

        public void Dispose()
        {
            receiveEventHook?.Dispose();
        }

        public void SendNotification()
        {
            if (!IsCompleted() && !Condition.IsBoundByDuty())
            {
                Chat.Print(Strings.Module.MiniCactpotLabel, Settings.TicketsRemaining + " " + Strings.Module.MiniCactpotTicketsRemainingLabel, Settings.EnableClickableLink ? goldSaucerTeleport : null);
            }
        }

        void IResettable.ResetThis()
        {
            Service.LogManager.LogMessage(ModuleType.MiniCactpot, "Daily Reset - Resetting");

            Settings.TicketsRemaining = 3;
        }

        public void* LotteryDaily_Show(AgentInterface* addon, void* a2, void* a3)
        {
            Settings.TicketsRemaining -= 1;
            
            Service.LogManager.LogMessage(ModuleType.MiniCactpot, $"Mini Cactpot Ticket Purchased");
            Service.CharacterConfiguration.Save();

            return receiveEventHook!.Original(addon, a2, a3);
        }

        public bool IsCompleted() => Settings.TicketsRemaining == 0;
    }
}
