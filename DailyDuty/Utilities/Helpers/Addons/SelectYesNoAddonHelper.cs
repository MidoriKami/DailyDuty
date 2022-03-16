using System;
using System.Collections.Generic;
using DailyDuty.Data.Enums;
using DailyDuty.Data.Enums.Addons;
using DailyDuty.Data.Structs;
using DailyDuty.System;
using Dalamud.Game;
using Dalamud.Game.Gui;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace DailyDuty.Utilities.Helpers.Addons
{
    public unsafe class SelectYesNoAddonHelper : IDisposable
    {
        private readonly Dictionary<AddonName, Action<YesNoState>> listeners = new();

        private delegate byte EventHandle(AtkUnitBase* atkUnitBase, AtkEventType eventType, uint eventParam, AtkEvent* atkEvent, MouseClickEventData* a5);

        private Hook<EventHandle>? eventHandleHook = null;

        public SelectYesNoAddonHelper()
        {
            Service.Framework.Update += OnFrameworkUpdate;
        }

        public void Dispose()
        {
            Service.Framework.Update -= OnFrameworkUpdate;

            eventHandleHook?.Dispose();
        }

        private void OnFrameworkUpdate(Framework framework)
        {
            if (IsOpen() == false) return;

            var addonPointer = GetAddonPointer();
            var yesButton = addonPointer->YesButton;

            if (addonPointer == null || yesButton == null) return;

            var eventHandlePointer = yesButton->AtkComponentBase.AtkEventListener.vfunc[2];
            
            eventHandleHook = new Hook<EventHandle>(new IntPtr(eventHandlePointer), OnButtonEvent);
            
            eventHandleHook.Enable();

            Service.Framework.Update -= OnFrameworkUpdate;
        }

        private byte OnButtonEvent(AtkUnitBase* atkUnitBase, AtkEventType eventType, uint eventParam, AtkEvent* atkEvent, MouseClickEventData* a5)
        {
            if (IsOpen())
            {
                switch (eventType)
                {
                    case AtkEventType.InputReceived when ((InputReceivedEventData*)a5)->KeyUp && ((InputReceivedEventData*)a5)->KeyCode == 1:
                    case AtkEventType.MouseDown when a5->RightClick == false:

                        var button = (AtkComponentButton*) atkUnitBase;

                        if (button->IsEnabled)
                        {
                            var pointer = GetAddonPointer();

                            if (atkUnitBase == pointer->YesButton)
                            {
                                foreach (var (addon, listener) in listeners)
                                {
                                    listener(YesNoState.Yes);
                                }
                            }
                            else if (atkUnitBase == pointer->NoButton)
                            {
                                foreach (var (addon, listener) in listeners)
                                {
                                    listener(YesNoState.No);
                                }
                            }
                        }
                        break;

                    default:
                        break;
                }
            }

            return eventHandleHook!.Original(atkUnitBase, eventType, eventParam, atkEvent, a5);
        }

        public void AddListener(AddonName addon, Action<YesNoState> action)
        {
            if (listeners.ContainsKey(addon) == false)
            {
                listeners.Add(addon, action);
            }
        }

        public void RemoveListener(AddonName addon)
        {
            //Chat.Debug("YesNo::RemovingListener::" + addon);
            listeners.Remove(addon);
        }

        private bool IsOpen()
        {
            return GetAddonPointer() != null;
        }

        //
        //  Implementation
        //
        private AddonSelectYesno* GetAddonPointer()
        {
            return (AddonSelectYesno*)Service.GameGui.GetAddonByName("SelectYesno", 1);
        }
    }
}
