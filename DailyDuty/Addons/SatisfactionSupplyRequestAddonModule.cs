using System;
using DailyDuty.Data.Enums;
using DailyDuty.Data.SettingsObjects.Weekly;
using DailyDuty.Data.Structs;
using DailyDuty.Interfaces;
using DailyDuty.System;
using DailyDuty.Utilities;
using DailyDuty.Utilities.Helpers.Addons;
using Dalamud.Game;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace DailyDuty.Addons
{
    internal unsafe class SatisfactionSupplyRequestAddonModule : IAddonModule
    {
        private CustomDeliverySettings Settings => Service.Configuration.Current().CustomDelivery;
        
        public AddonName AddonName => AddonName.SatisfactionSupplyRequest;

        private delegate byte EventHandle(AtkUnitBase* atkUnitBase, AtkEventType eventType, uint eventParam, AtkEvent* atkEvent, MouseClickEventData* a5);
        private delegate void* Finalize(AtkUnitBase* atkUnitBase);
        private delegate void* OnSetup(AtkUnitBase* atkUnitBase, int a2, void* a3);

        private Hook<OnSetup>? onSetupHook = null;
        private Hook<EventHandle>? eventHandleHook = null;
        private Hook<Finalize>? finalizeHook = null;

        private bool handOverButtonPressed = false;
        private AtkUnitBase* addonAddress = null;

        public SatisfactionSupplyRequestAddonModule()
        {
            Service.Framework.Update += FrameworkOnUpdate;
        }

        public void Dispose()
        {
            Service.Framework.Update -= FrameworkOnUpdate;

            onSetupHook?.Dispose();
            eventHandleHook?.Dispose();
            finalizeHook?.Dispose();
        }

        private void FrameworkOnUpdate(Framework framework)
        {
            if (IsCustomDeliveryWindowOpen() == false) return;
            if (IsRequestWindowOpen() == false) return;

            var addonPointer = GetCustomDeliveryPointer();
            var handOverButton = GetHandOverButton();

            if (addonPointer == null || handOverButton == null) return;
            
            var setupPointer = addonPointer->AtkEventListener.vfunc[45];
            var finalizePointer = addonPointer->AtkEventListener.vfunc[38];
            var eventHandlePointer = handOverButton->AtkEventListener.vfunc[2];

            onSetupHook = new Hook<OnSetup>(new IntPtr(setupPointer), OnSetupHandler);
            eventHandleHook = new Hook<EventHandle>(new IntPtr(eventHandlePointer), OnButtonEvent);
            finalizeHook = new Hook<Finalize>(new IntPtr(finalizePointer), OnFinalize);

            onSetupHook.Enable();
            eventHandleHook.Enable();
            finalizeHook.Enable();

            handOverButtonPressed = false;
            addonAddress = addonPointer;

            Service.Framework.Update -= FrameworkOnUpdate;
        }

        private void* OnSetupHandler(AtkUnitBase* atkUnitBase, int a2, void* a3)
        {
            handOverButtonPressed = false;
            addonAddress = atkUnitBase;

            return onSetupHook!.Original(atkUnitBase, a2, a3);
        }
        
        private byte OnButtonEvent(AtkUnitBase* atkUnitBase, AtkEventType eventType, uint eventParam, AtkEvent* atkEvent, MouseClickEventData* a5)
        {
            // If this module is enabled
            if (Settings.Enabled && IsCustomDeliveryWindowOpen())
            {
                switch (eventType)
                {
                    case AtkEventType.InputReceived when atkUnitBase == GetHandOverButton():
                    case AtkEventType.MouseDown when a5->RightClick == false && atkUnitBase == GetHandOverButton():

                        var button = (AtkComponentButton*) atkUnitBase;

                        if (button->IsEnabled)
                        {
                            handOverButtonPressed = true;
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
            if (Settings.Enabled && atkUnitBase == addonAddress)
            {
                var yesNoState = AddonManager.YesNoAddonHelper.GetCurrentState();

                if (yesNoState == SelectYesNoAddonHelper.ButtonState.Null)
                    yesNoState = AddonManager.YesNoAddonHelper.GetLastState();

                var yesPopupSelected = yesNoState == SelectYesNoAddonHelper.ButtonState.Yes;
                var nullPopup = yesNoState == SelectYesNoAddonHelper.ButtonState.Null;

                if (handOverButtonPressed && ( yesPopupSelected || nullPopup ) )
                {
                    Settings.AllowancesRemaining -= 1;
                    Service.Configuration.Save();
                }
            }
            
            return finalizeHook!.Original(atkUnitBase);
        }

        //
        //  Implementation
        //

        private bool IsCustomDeliveryWindowOpen()
        {
            return GetCustomDeliveryPointer() != null;
        }

        private bool IsRequestWindowOpen()
        {
            return GetRequestPointer() != null;
        }

        private AtkUnitBase* GetCustomDeliveryPointer()
        {
            return (AtkUnitBase*)Service.GameGui.GetAddonByName("SatisfactionSupply", 1);
        }

        private AtkUnitBase* GetRequestPointer()
        {
            return (AtkUnitBase*)Service.GameGui.GetAddonByName("Request", 1);
        }

        private AtkComponentBase* GetHandOverButton()
        {
            var requestPointer = GetRequestPointer();
            if(requestPointer == null) return null;

            var buttonNode = requestPointer->GetNodeById(14);
            if(buttonNode == null) return null;

            return buttonNode->GetComponent();
        }
    }
}
