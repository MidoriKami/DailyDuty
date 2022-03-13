using System;
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
        public enum ButtonState
        {
            Yes,
            No,
            Null
        }

        private ButtonState internalState;
        private ButtonState lastState;

        private delegate byte EventHandle(AtkUnitBase* atkUnitBase, AtkEventType eventType, uint eventParam, AtkEvent* atkEvent, MouseClickEventData* a5);
        private delegate void* OnSetup(AtkUnitBase* atkUnitBase, int a2, void* a3);
        private delegate void* Finalize(AtkUnitBase* atkUnitBase);

        private Hook<EventHandle>? eventHandleHook = null;
        private Hook<OnSetup>? onSetupHook = null;
        private Hook<Finalize>? finalizeHook = null;

        private AtkUnitBase* addonAddress = null;

        public SelectYesNoAddonHelper()
        {
            Service.Framework.Update += OnFrameworkUpdate;
        }

        public void Dispose()
        {
            Service.Framework.Update -= OnFrameworkUpdate;

            eventHandleHook?.Dispose();
            onSetupHook?.Dispose();
            finalizeHook?.Dispose();
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
            var finalizePointer = ((AtkUnitBase*)addonPointer)->AtkEventListener.vfunc[38];

            onSetupHook = new Hook<OnSetup>(new IntPtr(setupPointer), OnSetupHandler);
            eventHandleHook = new Hook<EventHandle>(new IntPtr(eventHandlePointer), OnButtonEvent);
            finalizeHook = new Hook<Finalize>(new IntPtr(finalizePointer), OnFinalize);

            onSetupHook.Enable();
            eventHandleHook.Enable();
            finalizeHook.Enable();

            internalState = ButtonState.Null;
            addonAddress = (AtkUnitBase*)addonPointer;

            Chat.Debug("YesNo::Hooked");
            Chat.Debug("YesNo::Null Internal State (This is good)");

            Service.Framework.Update -= OnFrameworkUpdate;
        }
        
        private void* OnSetupHandler(AtkUnitBase* atkUnitBase, int a2, void* a3)
        {
            Chat.Debug("YesNo::OnSetup");
            Chat.Debug("YesNo::Resetting Internal State to Null");

            internalState = ButtonState.Null;
            addonAddress = atkUnitBase;

            return onSetupHook!.Original(atkUnitBase, a2, a3);
        }

        private byte OnButtonEvent(AtkUnitBase* atkUnitBase, AtkEventType eventType, uint eventParam, AtkEvent* atkEvent, MouseClickEventData* a5)
        {
            var pointer = GetAddonPointer();

            switch (eventType)
            {
                case AtkEventType.MouseDown when a5->RightClick == false:
                    
                    var button = (AtkComponentButton*) atkUnitBase;

                    if (button->IsEnabled)
                    {
                        if (atkUnitBase == pointer->YesButton)
                        {
                            Chat.Debug("YesNo::Yes Button Clicked");
                            internalState = ButtonState.Yes;
                        }
                        else if (atkUnitBase == pointer->NoButton)
                        {
                            Chat.Debug("YesNo::No Button Clicked");
                            internalState = ButtonState.No;
                        }
                    }
                    break;

                default:
                    break;
            }

            return eventHandleHook!.Original(atkUnitBase, eventType, eventParam, atkEvent, a5);
        }

        private void* OnFinalize(AtkUnitBase* atkUnitBase)
        {
            if (atkUnitBase == addonAddress)
            {
                lastState = internalState;

                Chat.Debug("YesNo Saved:" + lastState);
            }
            
            return finalizeHook!.Original(atkUnitBase);
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
