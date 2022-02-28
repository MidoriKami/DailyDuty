using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DailyDuty.Data.Enums;
using DailyDuty.Data.SettingsObjects.Weekly;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using Dalamud.Game;
using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace DailyDuty.Addons
{
    internal unsafe class ReconstructionBoxAddonModule : IAddonModule
    {
        public AddonName AddonName => AddonName.ReconstructionBox;
        private DomanEnclaveSettings Settings => Service.Configuration.Current().DomanEnclave;

        private delegate byte EventHandle(AtkUnitBase* atkUnitBase, AtkEventType eventType, uint eventParam, AtkEvent* atkEvent, void* a5);
        private delegate void* Finalize(AtkUnitBase* atkUnitBase);

        private Hook<EventHandle>? eventHandleHook = null;
        private Hook<Finalize>? finalizeHook = null;

        private bool depositButtonPressed = false;
        private int depositAmount = 0;
        private AtkUnitBase* addonAddress = null;

        public ReconstructionBoxAddonModule()
        {
            SignatureHelper.Initialise(this);

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
            if (IsReconstructionBoxOpen() == false) return;

            var addonPointer = GetAddonPointer();
            var depositButton = GetDepositButton();

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
                if (depositButtonPressed && atkUnitBase == addonAddress)
                {
                    if (Settings.ShowTrackedDonationAmount)
                    {
                        Chat.Print("DonationAmount", $"{depositAmount:n0} gil donated to Doman Enclave");
                    }

                    depositButtonPressed = false;
                    Settings.CurrentEarnings += depositAmount;
                    Service.Configuration.Save();
                }
            }
            
            return finalizeHook!.Original(atkUnitBase);
        }

        private byte OnButtonEvent(AtkUnitBase* atkUnitBase, AtkEventType eventType, uint eventParam, AtkEvent* atkEvent, void* a5)
        {
            if (Settings.Enabled)
            {
                if (eventType == AtkEventType.MouseClick)
                {
                    // Close Button
                    if (atkUnitBase == GetCloseButton())
                    {
                        depositButtonPressed = false;
                        depositAmount = 0;
                    }

                    // Deposit Button
                    else if (atkUnitBase == GetDepositButton())
                    {
                        depositButtonPressed = true;
                        depositAmount = GetGrandTotal();
                        addonAddress = GetAddonPointer();
                    }
                }
            }

            return eventHandleHook!.Original(atkUnitBase, eventType, eventParam, atkEvent, a5);
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

        private AtkComponentBase* GetCloseButton()
        {
            var basePointer = GetAddonPointer();

            if(basePointer == null) return null;

            var windowComponent = (AtkComponentNode*)basePointer->GetNodeById(31);
            if(windowComponent == null) return null;

            var closeButtonNode = Node.GetNodeByID<AtkComponentNode>(windowComponent, 2);
            return closeButtonNode->Component;
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
