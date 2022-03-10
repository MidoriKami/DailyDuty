using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Data.Enums;
using DailyDuty.Data.ModuleData.JumboCactpot;
using DailyDuty.Data.SettingsObjects.Weekly;
using DailyDuty.Data.Structs;
using DailyDuty.Interfaces;
using DailyDuty.System;
using DailyDuty.Utilities;
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

        private delegate byte EventHandle(AtkUnitBase* atkUnitBase, AtkEventType eventType, uint eventParam, AtkEvent* atkEvent, void* a5);
        private delegate void* Finalize(AtkUnitBase* atkUnitBase);

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

            eventHandleHook?.Dispose();
            finalizeHook?.Dispose();
        }

        private void FrameworkOnUpdate(Framework framework)
        {
            if (IsWeeklyInputWindowOpen() == false) return;

            var addonPointer = GetAddonPointer();
            var purchaseButton = GetPurchaseButton();

            if (addonPointer == null || purchaseButton == null) return;
            
            var finalizePointer = addonPointer->AtkEventListener.vfunc[38];
            var eventHandlePointer = purchaseButton->AtkEventListener.vfunc[2];

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
                Chat.Debug("LotteryWeekly::Finalize");

                var yesNoState = AddonManager.YesNoAddonHelper.GetLastState();
                var yesPopupSelected = yesNoState == SelectYesNoAddonHelper.ButtonState.Yes;

                if (purchaseButtonPressed && atkUnitBase == addonAddress && yesPopupSelected)
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

        private byte OnButtonEvent(AtkUnitBase* atkUnitBase, AtkEventType eventType, uint eventParam, AtkEvent* atkEvent, void* a5)
        {
            if (Settings.Enabled)
            {
                if (atkUnitBase == GetCloseButton() || atkUnitBase == GetPurchaseButton())
                {
                    if (eventType == AtkEventType.MouseDown)
                    {
                        var button = (AtkComponentButton*) atkUnitBase;

                        if (button->IsEnabled)
                        {
                            var eventData = (MouseClickEventData*) a5;

                            if (eventData->RightClick == false)
                            {
                                if (atkUnitBase == GetCloseButton())
                                {
                                    purchaseButtonPressed = false;
                                    AddonManager.YesNoAddonHelper.ResetState();
                                }

                                if (atkUnitBase == GetPurchaseButton())
                                {
                                    purchaseButtonPressed = true;
                                    addonAddress = GetAddonPointer();
                                    AddonManager.YesNoAddonHelper.ResetState();
                                }
                            }
                        }
                    }
                }
            }

            return eventHandleHook!.Original(atkUnitBase, eventType, eventParam, atkEvent, a5);
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
