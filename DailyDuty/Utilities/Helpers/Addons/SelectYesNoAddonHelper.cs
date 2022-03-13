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

        private ButtonState currentState;

        private delegate byte EventHandle(AtkUnitBase* atkUnitBase, AtkEventType eventType, uint eventParam, AtkEvent* atkEvent, MouseClickEventData* a5);
        private delegate void* OnSetup(AtkUnitBase* atkUnitBase, int a2, void* a3);
        private delegate void* Finalize(AtkUnitBase* atkUnitBase);

        private Hook<EventHandle>? eventHandleHook = null;
        private Hook<OnSetup>? onSetupHook = null;
        private Hook<Finalize>? finalizeHook = null;

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

        public ButtonState GetCurrentState()
        {
            return currentState;
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

            currentState = ButtonState.Null;

            Service.Framework.Update -= OnFrameworkUpdate;
        }
        
        private void* OnSetupHandler(AtkUnitBase* atkUnitBase, int a2, void* a3)
        {
            Chat.Debug("YesNo::OnSetup");

            currentState = ButtonState.Null;

            return onSetupHook!.Original(atkUnitBase, a2, a3);
        }

        private byte OnButtonEvent(AtkUnitBase* atkUnitBase, AtkEventType eventType, uint eventParam, AtkEvent* atkEvent, MouseClickEventData* a5)
        {
            if (IsOpen())
            {
                switch (eventType)
                {
                    case AtkEventType.InputReceived:
                    case AtkEventType.MouseDown when a5->RightClick == false:

                        Chat.Debug("YesNo::Event");
                        var button = (AtkComponentButton*) atkUnitBase;

                        if (button->IsEnabled)
                        {
                            var pointer = GetAddonPointer();

                            if (atkUnitBase == pointer->YesButton)
                            {
                                currentState = ButtonState.Yes;
                            }
                            else if (atkUnitBase == pointer->NoButton)
                            {
                                currentState = ButtonState.No;
                            }
                        }
                        break;

                    default:
                        break;
                }
            }

            return eventHandleHook!.Original(atkUnitBase, eventType, eventParam, atkEvent, a5);
        }

        private void* OnFinalize(AtkUnitBase* atkUnitBase)
        {
            Chat.Debug("YesNo::Finalize");
            Chat.Debug("YesNo::PreFinalizedState::" + currentState);

            currentState = ButtonState.Null;
            
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
