using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DailyDuty.Data.Enums;
using DailyDuty.Data.Enums.Addons;
using DailyDuty.Data.SettingsObjects.Weekly;
using DailyDuty.Data.Structs;
using DailyDuty.Interfaces;
using DailyDuty.System;
using DailyDuty.Utilities;
using DailyDuty.Utilities.Helpers.Addons;
using Dalamud.Game;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;

namespace DailyDuty.Addons
{
    internal unsafe class ReconstructionBoxAddonModule : IAddonModule
    {
        public AddonName AddonName => AddonName.ReconstructionBox;
        private DomanEnclaveSettings Settings => Service.Configuration.Current().DomanEnclave;

        private delegate byte EventHandle(AtkUnitBase* atkUnitBase, AtkEventType eventType, uint eventParam, AtkEvent* atkEvent, MouseClickEventData* a5);
        private delegate void* OnSetup(AtkUnitBase* atkUnitBase, int a2, void* a3);
        private delegate void* Finalize(AtkUnitBase* atkUnitBase);

        private Hook<EventHandle>? eventHandleHook = null;
        private Hook<OnSetup>? onSetupHook = null;
        private Hook<Finalize>? finalizeHook = null;

        private bool depositButtonPressed = false;
        private int depositAmount = 0;
        private AtkUnitBase* addonAddress = null;

        private YesNoState yesNoState = YesNoState.Null;

        public ReconstructionBoxAddonModule()
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
            if (IsReconstructionBoxOpen() == false) return;

            var addonPointer = GetAddonPointer();
            var depositButton = GetDepositButton();

            if (addonPointer == null || depositButton == null) return;

            var setupPointer = addonPointer->AtkEventListener.vfunc[45];
            var eventHandlerPointer = depositButton->AtkEventListener.vfunc[2];
            var finalizePointer = addonPointer->AtkEventListener.vfunc[38];

            onSetupHook = new Hook<OnSetup>(new IntPtr(setupPointer), OnSetupHandler);
            eventHandleHook = new Hook<EventHandle>(new IntPtr(eventHandlerPointer), EventHandler);
            finalizeHook = new Hook<Finalize>(new IntPtr(finalizePointer), FinalizeHandler);

            onSetupHook.Enable();
            eventHandleHook.Enable();
            finalizeHook.Enable();

            Initialize(addonPointer);

            Service.Framework.Update -= FrameworkOnUpdate;
        }

        private void Initialize(AtkUnitBase* addonPointer)
        {
            depositButtonPressed = false;
            depositAmount = 0;
            addonAddress = addonPointer;
            yesNoState = YesNoState.Null;
        }

        private void* OnSetupHandler(AtkUnitBase* atkUnitBase, int a2, void* a3)
        {
            Initialize(atkUnitBase);

            return onSetupHook!.Original(atkUnitBase, a2, a3);
        }

        private void YesNoAction(YesNoState yesNoState)
        {
            switch (yesNoState)
            {
                case YesNoState.Null:
                    break;

                case YesNoState.Yes:
                    this.yesNoState = yesNoState;
                    break;

                case YesNoState.No:
                    AddonManager.YesNoAddonHelper.RemoveListener(AddonName);
                    break;
            }
        }
        
        private byte EventHandler(AtkUnitBase* atkUnitBase, AtkEventType eventType, uint eventParam, AtkEvent* atkEvent, MouseClickEventData* a5)
        {
            // If this module is enabled
            if (Settings.Enabled && IsReconstructionBoxOpen() && atkUnitBase == GetDepositButton())
            {
                switch (eventType)
                {
                    case AtkEventType.InputReceived when ((InputReceivedEventData*)a5)->KeyUp && ((InputReceivedEventData*)a5)->KeyCode == 1:
                    case AtkEventType.MouseDown when ((MouseClickEventData*)a5)->LeftClick:

                        var button = (AtkComponentButton*) atkUnitBase;

                        if (button->IsEnabled)
                        {
                            depositButtonPressed = true;
                            depositAmount = GetGrandTotal();

                            yesNoState = YesNoState.Null;
                            AddonManager.YesNoAddonHelper.AddListener(AddonName, YesNoAction);
                        }
                        break;

                    default:
                        break;
                }
            }

            return eventHandleHook!.Original(atkUnitBase, eventType, eventParam, atkEvent, a5);
        }

        private void* FinalizeHandler(AtkUnitBase* atkUnitBase)
        {
            if (Settings.Enabled && atkUnitBase == addonAddress)
            {
                AddonManager.YesNoAddonHelper.RemoveListener(AddonName);

                var yesPopupSelected = yesNoState == YesNoState.Yes;

                if (depositButtonPressed && yesPopupSelected)
                {
                    if (Settings.ShowTrackedDonationAmount)
                    {
                        Chat.Print("Donation Amount", $"{depositAmount:n0} gil donated to Doman Enclave");
                    }

                    depositButtonPressed = false;
                    Settings.CurrentEarnings += depositAmount;
                    Service.Configuration.Save();
                }
            }
            
            return finalizeHook!.Original(atkUnitBase);
        }

        //
        //  Implementation
        //

        private bool IsReconstructionBoxOpen()
        {
            return GetAddonPointer() != null;
        }

        private AtkUnitBase* GetAddonPointer()
        {
            return (AtkUnitBase*)Service.GameGui.GetAddonByName("ReconstructionBox", 1);
        }

        private AtkComponentBase* GetDepositButton()
        {
            var basePointer = GetAddonPointer();

            if(basePointer == null) return null;

            return basePointer->GetNodeById(30)->GetComponent();
        }

        private AtkTextNode* GetGrandTotalTextNode()
        {
            var basePointer = GetAddonPointer();
            
            if(basePointer == null) return null;

            return (AtkTextNode*)basePointer->GetNodeById(25);
        }

        private int GetGrandTotal()
        {
            var textNode = GetGrandTotalTextNode();
            if(textNode == null) return 0;

            var resultString = Regex.Replace(textNode->NodeText.ToString().ToLower(), "\\P{N}", "");

            return int.Parse(resultString);
        }
    }
}
