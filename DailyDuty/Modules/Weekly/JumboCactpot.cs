using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics;
using DailyDuty.Addons;
using DailyDuty.Data.Enums;
using DailyDuty.Data.SettingsObjects;
using DailyDuty.Data.SettingsObjects.Weekly;
using DailyDuty.Data.Structs;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using DailyDuty.Utilities.Helpers.JumboCactpot;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Hooking;
using Dalamud.Interface;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
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
        private JumboCactpotSettings Settings => Service.Configuration.Current().JumboCactpot;
        public GenericSettings GenericSettings => Settings;
        public CompletionType Type => CompletionType.Weekly;
        public string HeaderText => "Jumbo Cactpot";

        private readonly DalamudLinkPayload goldSaucerTeleport;
        
        private delegate void* ReceiveEventDelegate(AgentInterface* addon, void* a2, void* a3, int a4, int a5);

        // LotteryWeeklyRewardList
        [Signature("48 89 5C 24 ?? 44 89 4C 24 ?? 4C 89 44 24 ?? 55 56 57 41 54 41 55 41 56 41 57 48 8D 6C 24", DetourName = nameof(LotteryWeekly_ReceiveEvent))]
        private readonly Hook<ReceiveEventDelegate>? receiveEventHook = null;

        private int ticketData = -1;
        private int manualAddTicketValue = -1;

        private void* LotteryWeekly_ReceiveEvent(AgentInterface* addon, void* a2, void* a3, int a4, int a5)
        {
            var eventData = *(int*) ((byte*) a3 + 8);

            switch (a5)
            {
                // Message is from JumboCactpot
                case 0 when eventData >= 0:
                    ticketData = eventData;
                    break;

                // Message is from SelectYesNo
                case 5:
                    switch (eventData)
                    {
                        case -1:
                        case 1:
                            ticketData = -1;
                            break;

                        case 0 when ticketData >= 0:
                            Settings.Tickets.Add(ticketData);
                            ticketData = -1;
                            Service.Configuration.Save();
                            break;
                    }

                    break;
            }

            return receiveEventHook!.Original(addon, a2, a3, a4, a5);
        }


        public JumboCactpot()
        {
            SignatureHelper.Initialise(this);

            goldSaucerTeleport = Service.TeleportManager.GetPayload(TeleportPayloads.GoldSaucerTeleport);

            receiveEventHook?.Enable();
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

        public bool IsCompleted()
        {
            return Settings.Tickets.Count == 3;
        }

        public void SendNotification()
        {
            if (Condition.IsBoundByDuty()) return;

            if(Settings.Tickets.Count < 3)
            {
                Chat.Print(HeaderText,
                    Settings.Tickets.Count == 2
                        ? $"{3 - Settings.Tickets.Count} Ticket Available"
                        : $"{3 - Settings.Tickets.Count} Tickets Available", goldSaucerTeleport);
            }

        }

        public void NotificationOptions()
        {
            Draw.OnLoginReminderCheckbox(Settings);

            Draw.OnTerritoryChangeCheckbox(Settings);
        }

        public void EditModeOptions()
        {
            ImGui.Text("Manually Add/Remove Ticket");
            ImGui.SetNextItemWidth(100.0f * ImGuiHelpers.GlobalScale);
            ImGui.InputInt("##AddTicketValue", ref manualAddTicketValue, 0, 0);

            ImGui.SameLine();

            if (ImGui.Button("Add"))
            {
                Settings.Tickets.Add(manualAddTicketValue);
                Service.Configuration.Save();
            }

            ImGui.SameLine();

            if (ImGui.Button("Remove"))
            {
                Settings.Tickets.Remove(manualAddTicketValue);
                Service.Configuration.Save();
            }
        }

        public void DisplayData()
        {
            if (Settings.Tickets.Count > 0)
            {
                ImGui.Text("Ticket Values");
                ImGui.SameLine();
                ImGui.Text("[" + string.Join("] [", Settings.Tickets) + "]");
            }
            else
            {
                ImGui.Text("No Tickets Claimed");
            }

            var timespan = Settings.NextReset - DateTime.UtcNow;
            Draw.TimeSpanDisplay("Next Ticket Drawing", timespan);
        }

        public void Update()
        {
            UpdatePlayerRegion();
        }

        DateTime IResettable.GetNextReset() => GetNextJumboCactpotReset();

        void IResettable.ResetThis()
        {
            Settings.Tickets.Clear();
        }

        //
        //  Implementation
        //
        private DateTime GetNextJumboCactpotReset()
        {
            return DatacenterLookup.GetDrawingTime(Settings.PlayerRegion);
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
    }
}