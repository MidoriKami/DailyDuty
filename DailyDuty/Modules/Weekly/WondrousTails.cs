using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DailyDuty.Data.Enums;
using DailyDuty.Data.ModuleData.WondrousTails;
using DailyDuty.Data.SettingsObjects;
using DailyDuty.Data.SettingsObjects.WeeklySettings;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using DailyDuty.Utilities.Helpers.WondrousTails;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using Condition = DailyDuty.Utilities.Condition;

namespace DailyDuty.Modules.Weekly
{
    internal unsafe class WondrousTails : 
        IConfigurable, 
        IZoneChangeAlwaysNotification,
        IZoneChangeThrottledNotification,
        ILoginNotification,
        ICompletable,
        IUpdateable
    {
        private WondrousTailsSettings Settings => Service.Configuration.Current().WondrousTails;
        public CompletionType Type => CompletionType.Weekly;
        public string HeaderText => "Wondrous Tails";
        public GenericSettings GenericSettings => Settings;

        private readonly Stopwatch delayStopwatch = new();

        private uint lastDutyInstanceID = 0;
        private bool lastInstanceWasDuty = false;

        [Signature("88 05 ?? ?? ?? ?? 8B 43 18", ScanType = ScanType.StaticAddress)]
        private readonly WondrousTailsStruct* wondrousTails = null;

        [Signature("E8 ?? ?? ?? ?? E9 ?? ?? ?? ?? 41 B0 01 BA 13 00 00 00")]
        private readonly delegate* unmanaged<IntPtr, uint, uint, uint, short, void> useItem = null;

        private IntPtr ItemContextMenuAgent => (IntPtr)Framework.Instance()->GetUiModule()->GetAgentModule()->GetAgentByInternalID(10);
        private const uint WondrousTailsBookItemID = 2002023;

        private readonly DalamudLinkPayload openWondrousTailsPayload;
        private readonly DalamudLinkPayload idyllshireTeleport;

        public WondrousTails()
        {
            SignatureHelper.Initialise(this);
            
            Settings.NumPlacedStickers = wondrousTails->Stickers;

            openWondrousTailsPayload = Service.PluginInterface.AddChatLinkHandler((uint)FunctionalPayloads.OpenWondrousTailsBook, OpenWondrousTailsBook);

            idyllshireTeleport = Service.TeleportManager.GetPayload(TeleportPayloads.IdyllshireTeleport);
        }
    
        public void Dispose()
        {
            Service.PluginInterface.RemoveChatLinkHandler((uint)FunctionalPayloads.OpenWondrousTailsBook);
        }

        private void OpenWondrousTailsBook(uint arg1, SeString arg2)
        {
            if (useItem != null && ItemContextMenuAgent != IntPtr.Zero)
            {
                useItem(ItemContextMenuAgent, WondrousTailsBookItemID, 9999, 0, 0);
            }
        }

        public bool IsCompleted()
        {
            return IsBookComplete() && !NeedsNewBook();
        }

        public void SendNotification()
        {
            if (Settings.Enabled && Settings.NewBookNotification)
            {
                NewBookNotification();
            }

            if (Settings.Enabled && Settings.RerollNotificationTasks && wondrousTails->SecondChance == 9 && !IsBookComplete())
            {
                if (RerollValid())
                {
                    Chat.Print(HeaderText, "You have 9 Second-chance points, you can re-roll your stickers/tasks", openWondrousTailsPayload);
                }
            }

            if (Settings.Enabled && Settings.BookCompleteNotification && HasBook() && IsBookComplete())
            {
                Chat.Print(HeaderText, "You have a completed book available for turn-in", idyllshireTeleport);
            }
        }
    
        void IZoneChangeAlwaysNotification.SendNotification()
        {
            if (Settings.Enabled == false) return;

            if (Condition.IsBoundByDuty() && Settings.InstanceNotifications == true && !IsBookComplete())
            {
                var e = Service.ClientState.TerritoryType;
                lastInstanceWasDuty = true;
                lastDutyInstanceID = e;
                OnDutyStartNotification();
            }
            else if(lastInstanceWasDuty == true && Settings.InstanceNotifications == true && !IsBookComplete())
            {
                OnDutyEndNotification();
                lastInstanceWasDuty = false;
            }
            else
            {
                lastInstanceWasDuty = false;
            }
        }

        private void OnDutyEndNotification()
        {
            var node = FindNode(lastDutyInstanceID);
            if (node == null) return;

            var buttonState = node.Value.Item1;

            if (buttonState is ButtonState.Completable or ButtonState.AvailableNow)
            {
                Chat.Print(HeaderText, "You can claim a stamp for the last instance", openWondrousTailsPayload);
            }
        }

        private void OnDutyStartNotification()
        {
            var node = FindNode(lastDutyInstanceID);
            if (node == null) return;

            var buttonState = node.Value.Item1;
        
            switch (buttonState)
            {
                case ButtonState.Unavailable:
                    if (wondrousTails->SecondChance > 0)
                    {
                        Chat.Print(HeaderText, "This instance is available for a stamp if you re-roll it");
                        Chat.Print(HeaderText, $"You have {wondrousTails->SecondChance} Re-Rolls Available", openWondrousTailsPayload);
                    }
                    break;

                case ButtonState.AvailableNow:
                    Chat.Print(HeaderText, "A stamp is already available for this instance", openWondrousTailsPayload);
                    break;

                case ButtonState.Completable:
                    Chat.Print(HeaderText, "Completing this instance will reward you with a stamp");
                    break;

                case ButtonState.Unknown:
                    break;
            }
        }

        public void NotificationOptions()
        {
            Draw.Checkbox("Duty Start/End Notification",HeaderText, ref Settings.InstanceNotifications, "Send notifications at the start of a duty if the duty is a part of your Wondrous Tails book\n" +
                "Additionally, sends notifications after completing a duty to remind you to collect your stamp");

            Draw.Checkbox("Reroll Alert - Stickers", HeaderText, ref Settings.RerollNotificationStickers, "When changing zones, send a notification if you have the maximum number of second chance points\n" +
                "and between 3 and 7 stickers (inclusive)\n" +
                "Useful to re-roll the stickers for a chance at better rewards, while preventing over-capping");

            Draw.Checkbox("Reroll Alert - Tasks", HeaderText, ref Settings.RerollNotificationTasks, "When changing zones, send a notification if you have the maximum number of second chance points and have a task available to reroll\n" +
                "Useful to re-roll tasks that you might want to complete later");

            Draw.Checkbox("New Book Alert", HeaderText, ref Settings.NewBookNotification, "Notify me that a new book is available if I have a completed book");

            Draw.Checkbox("Completed Book Alert", HeaderText, ref Settings.BookCompleteNotification, "Notify me when my current book is completed to turn it in");

            Settings.ZoneChangeReminder = Settings.InstanceNotifications || Settings.RerollNotificationTasks;
            Settings.LoginReminder = Settings.NewBookNotification;
        }

        public void EditModeOptions()
        {
        }

        public void DisplayData()
        {
            PrintBookStatus();
        }

        public void Update()
        {
            if (Settings.Enabled == false) return;

            Time.UpdateDelayed(delayStopwatch, TimeSpan.FromSeconds(5), UpdateNumStamps );
        }
    
        //
        //  Implementation
        // 

        private void UpdateNumStamps()
        {
            var numStickers = wondrousTails->Stickers;

            if (Settings.NumPlacedStickers != numStickers)
            {
                Settings.NumPlacedStickers = wondrousTails->Stickers;

                if (wondrousTails->Stickers == 9)
                {
                    Settings.CompletionDate = DateTime.UtcNow;
                }

                Service.Configuration.Save();
            }
        }

        private void NewBookNotification()
        {
            if (NeedsNewBook())
            {
                Chat.Print(HeaderText, "A new Wondrous Tails Book is Available", idyllshireTeleport);
            }
        }

        private bool IsBookComplete()
        {
            return wondrousTails->Stickers == 9;
        }

        private bool NeedsNewBook()
        {
            // If the completion time was last week
            if (Settings.CompletionDate < Time.NextWeeklyReset().AddDays(-7))
            {
                // And we don't have a book
                if ( !HasBook() )
                {
                    return true;
                }
            }

            return false;
        }

        private bool RerollValid()
        {
            if (Settings.RerollNotificationTasks)
            {
                for (int i = 0; i < 16; ++i)
                {
                    var status = wondrousTails->TaskStatus(i);
                    if (status is ButtonState.AvailableNow or ButtonState.Unavailable)
                        return true;
                }
            }

            if (Settings.RerollNotificationStickers)
            {
                // We can reroll if any tasks are incomplete
                // We can spend re-rolls if we have more than 7 stickers
                if (wondrousTails->Stickers is >= 3 and <= 7)
                {
                    return true;
                }
            }

            return false;
        }
        private IEnumerable<(ButtonState, List<uint>)> GetAllTaskData()
        {
            var result = new (ButtonState, List<uint>)[16];

            for (int i = 0; i < 16; ++i)
            {
                var taskButtonState = wondrousTails->TaskStatus(i);
                var instances = TaskLookup.GetInstanceListFromID(wondrousTails->Tasks[i]);

                result[i] = (taskButtonState, instances);
            }

            return result;
        }

        private (ButtonState, List<uint>)? FindNode(uint instanceID)
        {
            foreach (var (buttonState, instanceList) in GetAllTaskData())
            {
                if (instanceList.Contains(instanceID))
                {
                    return (buttonState, instanceList);
                }
            }

            return null;
        }

        private void PrintBookStatus()
        {
            ImGui.Text("Book Status");
            ImGui.SameLine();

            Draw.ConditionalText(Settings.NumPlacedStickers == 9, "Complete", "Incomplete");
        }

        private bool HasBook()
        {
            var inventoryManager = InventoryManager.Instance();

            var result = inventoryManager->GetInventoryItemCount(WondrousTailsBookItemID);

            return result > 0;
        }
    }
}