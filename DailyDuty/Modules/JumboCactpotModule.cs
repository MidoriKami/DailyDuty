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
    internal unsafe class JumboCactpotModule :
        IResettable,
        IZoneChangeThrottledNotification,
        ILoginNotification,
        ICompletable,
        IDisposable
    {
        public GenericSettings GenericSettings => Settings;
        public CompletionType Type => CompletionType.Weekly;
        public string DisplayName => Strings.Module.JumboCactpotLabel;

        private readonly DalamudLinkPayload goldSaucerTeleport;
        public Action? ExpandedDisplay => null;

        private delegate void* AddonReceiveEvent(AgentInterface* addon, void* a2, AtkValue* eventData, int eventDataItemCount, int senderID);

            // LotteryWeekly_ReceiveEvent
        [Signature("48 89 5C 24 ?? 44 89 4C 24 ?? 4C 89 44 24 ?? 55 56 57 41 54 41 55 41 56 41 57 48 8D 6C 24", DetourName = nameof(LotteryWeekly_ReceiveEvent))]
        private readonly Hook<AddonReceiveEvent>? receiveEventHook = null;

        private static JumboCactpotSettings Settings => Service.CharacterConfiguration.JumboCactpot;
        private int ticketData = -1;

        public JumboCactpotModule()
        {
            SignatureHelper.Initialise(this);

            goldSaucerTeleport = Service.TeleportManager.GetPayload(TeleportPayloads.GoldSaucerTeleport);

            receiveEventHook?.Enable();
        }

        public void Dispose()
        {
            receiveEventHook?.Dispose();
        }

        public void SendNotification()
        {
            if (!IsCompleted() && !Condition.IsBoundByDuty())
            {
                Chat.Print(Strings.Module.JumboCactpotLabel,
                    Settings.Tickets.Count == 2
                        ? $"{3 - Settings.Tickets.Count} " + Strings.Module.JumboCactpotTicketAvailableLabel
                        : $"{3 - Settings.Tickets.Count} " + Strings.Module.JumboCactpotTicketsAvailableLabel, Settings.EnableClickableLink ? goldSaucerTeleport : null);
            }
        }

        DateTime IResettable.GetNextReset() => Time.NextJumboCactpotReset();

        void IResettable.ResetThis()
        {
            Service.LogManager.LogMessage(ModuleType.JumboCactpot, "Weekly Reset - Resetting");

            Settings.Tickets.Clear();
        }

        public bool IsCompleted() => Settings.Tickets.Count == 3;

        private void* LotteryWeekly_ReceiveEvent(AgentInterface* agent, void* a2, AtkValue* eventData, int eventDataItemCount, int senderID)
        {
            var data = eventData->Int;

            switch (senderID)
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
                            Service.LogManager.LogMessage(ModuleType.JumboCactpot, "Ticket Purchased - " + ticketData);
                            Settings.Tickets.Add(ticketData);
                            ticketData = -1;
                            Service.CharacterConfiguration.Save();
                            break;
                    }
                    break;
            }

            return receiveEventHook!.Original(agent, a2, eventData, eventDataItemCount, senderID);
        }
    }
}
