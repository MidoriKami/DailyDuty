using System;
using DailyDuty.Data.Enums;
using DailyDuty.Data.ModuleData.JumboCactpot;
using DailyDuty.Data.SettingsObjects.Weekly;
using DailyDuty.Data.Structs;
using DailyDuty.Interfaces;
using DailyDuty.System;
using DailyDuty.Utilities.Helpers.Addons;
using DailyDuty.Utilities.Helpers.JumboCactpot;
using Dalamud.Game;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace DailyDuty.Addons
{
    internal unsafe class LotteryWeeklyInputAddonModule : IAddonModule
    {
        public AddonName AddonName => AddonName.LotteryWeeklyInput;
        private JumboCactpotSettings Settings => Service.Configuration.Current().JumboCactpot;

        private delegate byte EventHandle(AtkUnitBase* atkUnitBase, AtkEventType eventType, uint eventParam, AtkEvent* atkEvent, MouseClickEventData* a5);
        private delegate void* OnSetup(AtkUnitBase* atkUnitBase, int a2, void* a3);
        private delegate void* Finalize(AtkUnitBase* atkUnitBase);

        private Hook<OnSetup>? onSetupHook = null;
        private Hook<EventHandle>? eventHandleHook = null;
        private Hook<Finalize>? finalizeHook = null;

        private bool purchaseButtonPressed = false;
        private AtkUnitBase* addonAddress = null;

        public LotteryWeeklyInputAddonModule()
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
            if (IsWeeklyInputWindowOpen() == false) return;

            var addonPointer = GetAddonPointer();
            var purchaseButton = GetPurchaseButton();

            if (addonPointer == null || purchaseButton == null) return;
            
            var setupPointer = addonPointer->AtkEventListener.vfunc[45];
            var finalizePointer = addonPointer->AtkEventListener.vfunc[38];
            var eventHandlePointer = purchaseButton->AtkEventListener.vfunc[2];

            onSetupHook = new Hook<OnSetup>(new IntPtr(setupPointer), OnSetupHandler);
            eventHandleHook = new Hook<EventHandle>(new IntPtr(eventHandlePointer), OnButtonEvent);
            finalizeHook = new Hook<Finalize>(new IntPtr(finalizePointer), OnFinalize);

            onSetupHook.Enable();
            eventHandleHook.Enable();
            finalizeHook.Enable();

            purchaseButtonPressed = false;
            addonAddress = addonPointer;

            AddonManager.YesNoAddonHelper.ResetState();

            Service.Framework.Update -= FrameworkOnUpdate;
        }

        private void* OnSetupHandler(AtkUnitBase* atkUnitBase, int a2, void* a3)
        {
            purchaseButtonPressed = false;
            addonAddress = atkUnitBase;

            AddonManager.YesNoAddonHelper.ResetState();

            return onSetupHook!.Original(atkUnitBase, a2, a3);
        }

        private byte OnButtonEvent(AtkUnitBase* atkUnitBase, AtkEventType eventType, uint eventParam, AtkEvent* atkEvent, MouseClickEventData* a5)
        {
            // If this module is enabled
            if (Settings.Enabled && IsWeeklyInputWindowOpen())
            {
                switch (eventType)
                {
                    case AtkEventType.MouseDown when a5->RightClick == false && atkUnitBase == GetPurchaseButton():

                        var button = (AtkComponentButton*) atkUnitBase;

                        if (button->IsEnabled)
                        {
                            purchaseButtonPressed = true;

                            AddonManager.YesNoAddonHelper.ResetState();
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
                var yesNoState = AddonManager.YesNoAddonHelper.GetLastState();
                var yesPopupSelected = yesNoState == SelectYesNoAddonHelper.ButtonState.Yes;

                if (purchaseButtonPressed  && yesPopupSelected)
                {
                    purchaseButtonPressed = false;
                    Settings.CollectedTickets.Add(new TicketData
                    {
                        DrawingAvailableTime = GetNextReset(),
                        ExpirationDate = GetNextReset().AddDays(7),
                        CollectedDate = DateTime.UtcNow
                    });
                    Service.Configuration.Save();
                }
            }
            
            return finalizeHook!.Original(atkUnitBase);
        }

        //
        //  Implementation
        //

        private bool IsWeeklyInputWindowOpen()
        {
            return GetAddonPointer() != null;
        }

        private AtkUnitBase* GetAddonPointer()
        {
            return (AtkUnitBase*)Service.GameGui.GetAddonByName("LotteryWeeklyInput", 1);
        }

        private AtkComponentBase* GetPurchaseButton()
        {
            var basePointer = GetAddonPointer();

            if(basePointer == null) return null;

            var purchaseButtonNode = (AtkComponentNode*)basePointer->GetNodeById(31);

            if(purchaseButtonNode == null) return null;

            return purchaseButtonNode->Component;
        }

        private AtkComponentBase* GetCloseButton()
        {
            var basePointer = GetAddonPointer();

            if(basePointer == null) return null;

            var closeButtonNode = (AtkComponentNode*)basePointer->GetNodeById(35);

            if(closeButtonNode == null) return null;

            return closeButtonNode->Component;
        }

        private DateTime GetNextReset()
        {
            return DatacenterLookup.GetDrawingTime(Settings.PlayerRegion);
        }
    }
}
