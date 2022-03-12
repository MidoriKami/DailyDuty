using System;
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
            Neither,
            Null
        }

        private ButtonState lastState;

        private delegate byte EventHandle(AtkUnitBase* atkUnitBase, AtkEventType eventType, uint eventParam, AtkEvent* atkEvent, void* a5);
        private delegate void* OnSetup(AtkUnitBase* atkUnitBase, int a2, void* a3);

        private Hook<EventHandle>? eventHandleHook = null;
        private Hook<OnSetup>? onSetupHook = null;

        public SelectYesNoAddonHelper()
        {
            Service.Framework.Update += OnFrameworkUpdate;
        }

        public void Dispose()
        {
            Service.Framework.Update -= OnFrameworkUpdate;

            eventHandleHook?.Dispose();
            onSetupHook?.Dispose();
        }

        public void ResetState()
        {
            lastState = ButtonState.Null;
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
            
            var setupPointer = ((AtkUnitBase*)addonPointer)->AtkEventListener.vfunc[45];
            var eventHandlePointer = yesButton->AtkComponentBase.AtkEventListener.vfunc[2];

            onSetupHook = new Hook<OnSetup>(new IntPtr(setupPointer), OnSetupHandler);
            eventHandleHook = new Hook<EventHandle>(new IntPtr(eventHandlePointer), OnButtonEvent);

            onSetupHook.Enable();
            eventHandleHook.Enable();

            Service.Framework.Update -= OnFrameworkUpdate;
        }

        private void* OnSetupHandler(AtkUnitBase* atkUnitBase, int a2, void* a3)
        {
            lastState = ButtonState.Neither;

            return onSetupHook!.Original(atkUnitBase, a2, a3);
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
