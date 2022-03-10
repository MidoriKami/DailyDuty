using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private delegate byte EventHandle(AtkUnitBase* atkUnitBase, AtkEventType eventType, uint eventParam, AtkEvent* atkEvent, void* a5);
        private delegate void* Finalize(AtkUnitBase* atkUnitBase);

        private Hook<EventHandle>? eventHandleHook = null;
        private Hook<Finalize>? finalizeHook = null;

        private bool HandOverButtonPressed = false;
        private AtkUnitBase* addonAddress = null;

        public SatisfactionSupplyRequestAddonModule()
        {
            Service.Framework.Update += FrameworkOnUpdate;
        }

        public void Dispose()
        {
            Service.Framework.Update -= FrameworkOnUpdate;

            eventHandleHook?.Dispose();
            finalizeHook?.Dispose();
        }

        private void FrameworkOnUpdate(Framework framework)
        {
            if (IsCustomDeliveryWindowOpen() == false) return;
            if (IsRequestWindowOpen() == false) return;

            var addonPointer = GetCustomDeliveryPointer();
            var depositButton = GetHandOverButton();

            if (addonPointer == null || depositButton == null) return;
            
            var finalizePointer = addonPointer->AtkEventListener.vfunc[38];
            var eventHandlePointer = depositButton->AtkEventListener.vfunc[2];

            eventHandleHook = new Hook<EventHandle>(new IntPtr(eventHandlePointer), OnButtonEvent);
            finalizeHook = new Hook<Finalize>(new IntPtr(finalizePointer), OnFinalize);

            eventHandleHook.Enable();
            finalizeHook.Enable();

            Service.Framework.Update -= FrameworkOnUpdate;
        }

        private void* OnFinalize(AtkUnitBase* atkUnitBase)
        {
            if (Settings.Enabled)
            {
                Chat.Debug("CustomDelivery::Finalize");

                var yesNoState = AddonManager.YesNoAddonHelper.GetLastState();
                var yesPopupSelected = yesNoState == SelectYesNoAddonHelper.ButtonState.Yes;
                var yesNoNotOpened = yesNoState == SelectYesNoAddonHelper.ButtonState.Null;

                Chat.Debug($"YesNoState:{yesNoState}");

                if (HandOverButtonPressed && atkUnitBase == addonAddress && (yesPopupSelected || yesNoNotOpened))
                {
                    Chat.Debug("Perform Logic!");
                    //if (Settings.ShowTrackedDonationAmount)
                    //{
                    //    Chat.Print("DonationAmount", $"{depositAmount:n0} gil donated to Doman Enclave");
                    //}

                    //HandOverButtonPressed = false;
                    //Settings.CurrentEarnings += depositAmount;
                    //Service.Configuration.Save();
                }
            }
            
            return finalizeHook!.Original(atkUnitBase);
        }

        private byte OnButtonEvent(AtkUnitBase* atkUnitBase, AtkEventType eventType, uint eventParam, AtkEvent* atkEvent, void* a5)
        {
            if (Settings.Enabled)
            {
                // Click!
                if (eventType == AtkEventType.MouseDown)
                {
                    var eventData = (MouseClickEventData*) a5;

                    // We are a Left Click!
                    if (eventData->RightClick == false)
                    {
                        // Left Click on what we care about
                        if (atkUnitBase == GetHandOverButton())
                        {
                            var buttonNode = (AtkComponentButton*) atkUnitBase;

                            if (buttonNode->IsEnabled)
                            {


                                HandOverButtonPressed = true;
                                addonAddress = GetCustomDeliveryPointer();
                                AddonManager.YesNoAddonHelper.ResetState();
                            }
                        }

                        // Left click on something else
                        else
                        {
                            Chat.Debug("Clicked on Something Else");
                            HandOverButtonPressed = false;
                        }
                    }
                }
            }

            return eventHandleHook!.Original(atkUnitBase, eventType, eventParam, atkEvent, a5);
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
