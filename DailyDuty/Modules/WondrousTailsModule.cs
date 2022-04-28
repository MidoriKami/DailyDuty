using System;
using System.Collections.Generic;
using DailyDuty.Data.Components;
using DailyDuty.Data.ModuleSettings;
using DailyDuty.Enums;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.Structs;
using DailyDuty.Utilities;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
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
        ICompletable
    {
        private static WondrousTailsSettings Settings => Service.CharacterConfiguration.WondrousTails;
        public CompletionType Type => CompletionType.Weekly;
        public GenericSettings GenericSettings => Settings;
        public string DisplayName => Strings.Module.WondrousTailsLabel;
        public Action? ExpandedDisplay => null;

        private delegate void UseItemDelegate(IntPtr a1, uint a2, uint a3, uint a4, short a5);

        [Signature("88 05 ?? ?? ?? ?? 8B 43 18", ScanType = ScanType.StaticAddress)]
        private readonly WondrousTailsStruct* wondrousTails = null;

        [Signature("E8 ?? ?? ?? ?? E9 ?? ?? ?? ?? 41 B0 01 BA 13 00 00 00")]
        private readonly UseItemDelegate useItemFunction = null!;

        private IntPtr ItemContextMenuAgent => (IntPtr)Framework.Instance()->GetUiModule()->GetAgentModule()->GetAgentByInternalId(AgentId.InventoryContext);
        private const uint WondrousTailsBookItemID = 2002023;

        private readonly DalamudLinkPayload openWondrousTails;
        private readonly DalamudLinkPayload idyllshireTeleport;

        private uint lastDutyInstanceID = 0;
        private bool lastInstanceWasDuty = false;

        public WondrousTailsModule()
        {
            SignatureHelper.Initialise(this);

            Service.PluginInterface.RemoveChatLinkHandler((uint)ChatPayloads.OpenWondrousTailsBook);
            openWondrousTails = Service.PluginInterface.AddChatLinkHandler((uint)ChatPayloads.OpenWondrousTailsBook, OpenWondrousTailsBook);

            idyllshireTeleport = Service.TeleportManager.GetPayload(ChatPayloads.IdyllshireTeleport);
        }

        public void Dispose()
        {
            Service.PluginInterface.RemoveChatLinkHandler((uint)ChatPayloads.OpenWondrousTailsBook);
        }

        private void OpenWondrousTailsBook(uint arg1, SeString arg2)
        {
            if (ItemContextMenuAgent != IntPtr.Zero && InventoryContainsBook())
            {
                useItemFunction(ItemContextMenuAgent, WondrousTailsBookItemID, 9999, 0, 0);
            }
        }

        private bool InventoryContainsBook()
        {
            var inventoryManager = InventoryManager.Instance();

            var result = inventoryManager->GetInventoryItemCount(WondrousTailsBookItemID);

            return result > 0;
        }

        public bool IsCompleted() => wondrousTails->Stickers == 9;

        void IZoneChangeAlwaysNotification.SendNotification()
        {
            if (Settings.Enabled == false) return;

            if (Condition.IsBoundByDuty() && Settings.InstanceNotifications && !IsCompleted())
            {
                var e = Service.ClientState.TerritoryType;
                lastInstanceWasDuty = true;
                lastDutyInstanceID = e;
                OnDutyStartNotification();
            }
            else if(lastInstanceWasDuty && Settings.InstanceNotifications && !IsCompleted())
            {
                OnDutyEndNotification();
                lastInstanceWasDuty = false;
            }
            else
            {
                lastInstanceWasDuty = false;
            }
        }

        private void OnDutyStartNotification()
        {
            var node = FindNode(lastDutyInstanceID);
            if (node == null) return;

            var buttonState = node.TaskState;
        
            switch (buttonState)
            {
                case ButtonState.Unavailable:
                    if (wondrousTails->SecondChance > 0)
                    {
                        Chat.Print(Strings.Module.WondrousTailsLabel, Strings.Module.WondrousTailsUnavailableMessage);
                        Chat.Print(Strings.Module.WondrousTailsLabel, Strings.Module.WondrousTailsUnavailableRerollMessage.Format(wondrousTails->SecondChance), openWondrousTails);
                    }
                    break;

                case ButtonState.AvailableNow:
                    Chat.Print(Strings.Module.WondrousTailsLabel, Strings.Module.WondrousTailsAvailableMessage, openWondrousTails);
                    break;

                case ButtonState.Completable:
                    Chat.Print(Strings.Module.WondrousTailsLabel, Strings.Module.WondrousTailsCompletableMessage);
                    break;

                case ButtonState.Unknown:
                    break;
            }
        }

        private void OnDutyEndNotification()
        {
            var node = FindNode(lastDutyInstanceID);
            if (node == null) return;

            var buttonState = node.TaskState;

            if (buttonState is ButtonState.Completable or ButtonState.AvailableNow)
            {
                Chat.Print(Strings.Module.WondrousTailsLabel, Strings.Module.WondrousTailsClaimableMessage, openWondrousTails);
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

                result.Add(new WondrousTailsTask()
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
    }
}
