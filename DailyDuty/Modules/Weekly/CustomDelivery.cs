using System;
using System.Text.RegularExpressions;
using DailyDuty.Data.Enums;
using DailyDuty.Data.SettingsObjects;
using DailyDuty.Data.SettingsObjects.Weekly;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Condition = DailyDuty.Utilities.Condition;

namespace DailyDuty.Modules.Weekly
{
    internal unsafe class CustomDelivery : 
        IConfigurable, 
        IZoneChangeThrottledNotification,
        ILoginNotification,
        ICompletable
    {
        private CustomDeliverySettings Settings => Service.Configuration.Current().CustomDelivery;
        public CompletionType Type => CompletionType.Weekly;
        public string HeaderText => "Custom Delivery";
        public GenericSettings GenericSettings => Settings;

        [Signature("0F B6 51 1B 44 0F B6 41")]
        private readonly delegate* unmanaged<byte*, int> getCustomDeliveryAllowances = null!;

        [Signature("48 8D 0D ?? ?? ?? ?? 41 0F BA EC", ScanType = ScanType.StaticAddress)]
        private readonly byte* staticArrayPointer = null!;

        public CustomDelivery()
        {
            SignatureHelper.Initialise(this);
        }

        public bool IsCompleted()
        {
            return GetAllowances() == 0;
        }

        public void SendNotification()
        {
            if (Condition.IsBoundByDuty() == true) return;

            PrintRemainingAllowances();
        }

        public void NotificationOptions()
        {        
            Draw.OnLoginReminderCheckbox(Settings);

            Draw.OnTerritoryChangeCheckbox(Settings);
        }

        public void EditModeOptions()
        {
        }

        public void DisplayData()
        {
            Draw.NumericDisplay("Allowances Remaining", GetAllowances());
        }
        
        public void Dispose()
        {

        }

        //
        //  Implementation
        //

        private void PrintRemainingAllowances()
        {
            if (Settings.AllowancesRemaining > 0)
            {
                Chat.Print(HeaderText, $"{GetAllowances()} Allowances Remaining");
            }
        }

        private int GetAllowances()
        {
            return 12 - getCustomDeliveryAllowances(staticArrayPointer);
        }
    }
}