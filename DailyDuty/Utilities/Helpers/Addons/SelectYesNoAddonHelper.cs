using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Addons;
using Dalamud.Game;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace DailyDuty.Utilities.Helpers.Addons
{
    public unsafe class SelectYesNoAddonHelper : IDisposable
    {
        public enum ButtonState
        {
            Yes,
            No,
            Neither
        }

        private ButtonState lastState;

        private delegate byte EventHandle(AtkUnitBase* atkUnitBase, AtkEventType eventType, uint eventParam, AtkEvent* atkEvent, void* a5);

        private Hook<EventHandle>? eventHandleHook = null;

        public SelectYesNoAddonHelper()
        {
            Service.Framework.Update += OnFrameworkUpdate;
        }

        public void Dispose()
        {
            eventHandleHook?.Dispose();

            Service.Framework.Update -= OnFrameworkUpdate;
        }

        public void ResetState()
        {
            lastState = ButtonState.Neither;
        }

        public ButtonState GetLastState()
        {
            return lastState;
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

        private byte OnButtonEvent(AtkUnitBase* atkunitbase, AtkEventType eventtype, uint eventparam, AtkEvent* atkevent, void* a5)
        {
            if (eventtype == AtkEventType.MouseDown)
            {
                var pointer = GetAddonPointer();

                if (pointer != null)
                {
                    if (atkunitbase == pointer->YesButton)
                    {
                        lastState = ButtonState.Yes;
                    }
                    else if (atkunitbase == pointer->NoButton)
                    {
                        lastState = ButtonState.No;
                    }
                    else
                    {
                        lastState = ButtonState.Neither;
                    }
                }
            }

            return eventHandleHook!.Original(atkunitbase, eventtype, eventparam, atkevent, a5);
        }

        public bool IsOpen()
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
