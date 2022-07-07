using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.Data.Components;
using DailyDuty.Data.ModuleSettings;
using DailyDuty.Enums;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.Structs;
using DailyDuty.Utilities;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud.Utility;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace DailyDuty.Modules
{
    internal unsafe class WondrousTailsModule :
        IDisposable,
        IZoneChangeAlwaysNotification,
        IZoneChangeThrottledNotification,
        ICompletable
    {
        private static WondrousTailsSettings Settings => Service.CharacterConfiguration.WondrousTails;
        public CompletionType Type => CompletionType.Weekly;
        public GenericSettings GenericSettings => Settings;

        public string DisplayName => Strings.Module.WondrousTailsLabel;
        public Action? ExpandedDisplay => null;

        private delegate void UseItemDelegate(IntPtr a1, uint a2, uint a3, uint a4, short a5);
        private delegate byte DutyEventDelegate(void* a1, void* a2, ushort* a3);
        private delegate int WondrousTailsGetDeadlineDelegate(int* deadlineIndex);

        [Signature("88 05 ?? ?? ?? ?? 8B 43 18", ScanType = ScanType.StaticAddress)]
        private readonly WondrousTailsStruct* wondrousTails = null;

        [Signature("E8 ?? ?? ?? ?? E9 ?? ?? ?? ?? 41 B0 01 BA 13 00 00 00")]
        private readonly UseItemDelegate useItemFunction = null!;

        [Signature("48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC ?? 48 8B D9 49 8B F8 41 0F B7 08", DetourName = nameof(DutyEventFunction))]
        private readonly Hook<DutyEventDelegate>? dutyEventHook = null;

        [Signature("48 8D 0D ?? ?? ?? ?? 48 89 BD ?? ?? ?? ?? E8 ?? ?? ?? ?? 44 8B C0", ScanType = ScanType.StaticAddress)]
        private readonly int* wondrousTailsDeadlineIndex = null;
  
        [Signature("8B 81 ?? ?? ?? ?? C1 E8 04 25")]
        private readonly WondrousTailsGetDeadlineDelegate wondrousTailsGetDeadline = null!;

        private bool dutyEndNotificationSent;

        private IntPtr ItemContextMenuAgent => (IntPtr)Framework.Instance()->GetUiModule()->GetAgentModule()->GetAgentByInternalId(AgentId.InventoryContext);
        private const uint WondrousTailsBookItemID = 2002023;

        private readonly DalamudLinkPayload openWondrousTails;
        private readonly DalamudLinkPayload idyllshireTeleport;

        public WondrousTailsModule()
        {
            SignatureHelper.Initialise(this);

            dutyEventHook?.Enable();

            Service.PluginInterface.RemoveChatLinkHandler((uint)ChatPayloads.OpenWondrousTailsBook);
            openWondrousTails = Service.PluginInterface.AddChatLinkHandler((uint)ChatPayloads.OpenWondrousTailsBook, OpenWondrousTailsBook);
            idyllshireTeleport = Service.TeleportManager.GetPayload(ChatPayloads.IdyllshireTeleport);
        }

        public void Dispose()
        {
            Service.PluginInterface.RemoveChatLinkHandler((uint)ChatPayloads.OpenWondrousTailsBook);

            dutyEventHook?.Dispose();
        }

        private byte DutyEventFunction(void* a1, void* a2, ushort* a3)
        {
            try
            {
                if (Service.LoggedIn && Settings.Enabled)
                {
                    var category = *(a3);
                    var type = *(uint*) (a3 + 4);

                    // DirectorUpdate Category
                    if (category == 0x6D)
                    {
                        switch (type)
                        {
                            // Duty Commenced
                            case 0x40000001 when Settings.InstanceNotifications && !IsCompleted():
                                OnDutyStartNotification(Service.ClientState.TerritoryType);
                                break;

                            // Party Wipe
                            case 0x40000005:
                                break;

                            // Duty Recommence
                            case 0x40000006:
                                break;

                            // Duty Completed
                            case 0x40000003 when Settings.InstanceNotifications && !IsCompleted():
                                OnDutyEndNotification(Service.ClientState.TerritoryType);
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "Failed to get Duty Started Status");
            }
                
            return dutyEventHook!.Original(a1, a2, a3);
        }

        private void OpenWondrousTailsBook(uint arg1, SeString arg2)
        {
            if (ItemContextMenuAgent != IntPtr.Zero && InventoryContainsWondrousTailsBook())
            {
                useItemFunction(ItemContextMenuAgent, WondrousTailsBookItemID, 9999, 0, 0);
            }
        }

        public static bool InventoryContainsWondrousTailsBook()
        {
            var inventoryManager = InventoryManager.Instance();

            var result = inventoryManager->GetInventoryItemCount(WondrousTailsBookItemID);

            return result > 0;
        }

        public bool IsCompleted() => wondrousTails->Stickers == 9;

        void IZoneChangeAlwaysNotification.SendNotification(ushort newTerritory)
        {
            // Sticker Available Notification
            if (!Condition.IsBoundByDuty() && Settings.StickerAvailableNotification && !IsCompleted())
            {
                if (AnyTasksAvailableNow() && !dutyEndNotificationSent)
                { 
                    Chat.Print(Strings.Module.WondrousTailsLabel, Strings.Module.WondrousTailsStickersAvailableNotification, Settings.EnableOpenBookLink ? openWondrousTails : null);
                }
            }

            dutyEndNotificationSent = false;
        }

        public void SendNotification()
        {
            if (!Condition.IsBoundByDuty() && Settings.DeadlineEarlyWarning)
            {
                var deadLine = GetDeadline();
                var now = DateTime.Now;
                var daysRemaining = deadLine - now;

                if (now + TimeSpan.FromDays(Settings.EarlyWarningDays) > deadLine)
                {
                    if (daysRemaining.Days > 1)
                    {
                        Chat.Print(Strings.Module.WondrousTailsLabel, Strings.Module.WondrousTailsDeadlineEarlyWarningPlural.Format(daysRemaining.Days), Settings.EnableOpenBookLink ? openWondrousTails : null);
                    }
                    else if (daysRemaining.Days == 1)
                    {
                        Chat.Print(Strings.Module.WondrousTailsLabel, Strings.Module.WondrousTailsDeadlineEarlyWarningSingular.Format(daysRemaining.Days), Settings.EnableOpenBookLink ? openWondrousTails : null);
                    }

                }
            }

            if (!Condition.IsBoundByDuty() && Settings.UnclaimedBookWarning)
            {

                // If the user doesn't have a book
                if (!InventoryContainsWondrousTailsBook())
                {
                    var deadline = GetDeadline();
                    var now = DateTime.Now;

                    // If deadline isn't this week, but next week
                    if (now > deadline - TimeSpan.FromDays(7))
                    {
                        Chat.Print(Strings.Module.WondrousTailsLabel, Strings.Module.WondrousTailsBookAvailableNotification, Settings.EnableOpenBookLink ? idyllshireTeleport : null);
                    }
                }
            }
        }

        private void OnDutyStartNotification(ushort currentInstance)
        {
            var node = FindNode(currentInstance);
            if (node == null) return;

            var buttonState = node.TaskState;
        
            switch (buttonState)
            {
                case ButtonState.Unavailable:
                    if (wondrousTails->SecondChance > 0)
                    {
                        Chat.Print(Strings.Module.WondrousTailsLabel, Strings.Module.WondrousTailsUnavailableMessage);
                        Chat.Print(Strings.Module.WondrousTailsLabel, Strings.Module.WondrousTailsUnavailableRerollMessage.Format(wondrousTails->SecondChance), Settings.EnableOpenBookLink ? openWondrousTails : null);
                    }
                    break;

                case ButtonState.AvailableNow:
                    Chat.Print(Strings.Module.WondrousTailsLabel, Strings.Module.WondrousTailsAvailableMessage, Settings.EnableOpenBookLink ? openWondrousTails : null);
                    break;

                case ButtonState.Completable:
                    Chat.Print(Strings.Module.WondrousTailsLabel, Strings.Module.WondrousTailsCompletableMessage);
                    break;

                case ButtonState.Unknown:
                    break;
            }
        }

        private void OnDutyEndNotification(ushort currentInstance)
        {
            var node = FindNode(currentInstance);

            var buttonState = node?.TaskState;

            if (buttonState is ButtonState.Completable or ButtonState.AvailableNow)
            {
                dutyEndNotificationSent = true;
                Chat.Print(Strings.Module.WondrousTailsLabel, Strings.Module.WondrousTailsClaimableMessage, Settings.EnableOpenBookLink ? openWondrousTails : null);
            }
        }

        public int GetNumStamps()
        {
            return wondrousTails->Stickers;
        }

        public static List<WondrousTailsTask> GetAllTaskData(WondrousTailsStruct* wondrousTailsStruct)
        {
            var result = new List<WondrousTailsTask>();

            for (var i = 0; i < 16; ++i)
            {
                var taskButtonState = wondrousTailsStruct->TaskStatus(i);
                var instances = TaskLookup.GetInstanceListFromID(wondrousTailsStruct->Tasks[i]);

                result.Add(new WondrousTailsTask
                {
                    DutyList = instances,
                    TaskState = taskButtonState,
                });
            }

            return result;
        }

        private WondrousTailsTask? FindNode(uint instanceID)
        {
            foreach (var taskData in GetAllTaskData(wondrousTails))
            {
                if (taskData.DutyList.Contains(instanceID))
                {
                    return taskData;
                }
            }

            return null;
        }

        private bool AnyTasksAvailableNow()
        {
            return GetAllTaskData(wondrousTails).Any(task => task.TaskState == ButtonState.AvailableNow);
        }

        public DateTime GetDeadline()
        {
            var deadline = wondrousTailsGetDeadline(wondrousTailsDeadlineIndex);

            return DateTimeOffset.FromUnixTimeSeconds(deadline).ToLocalTime().DateTime; 
        }
    }
}
