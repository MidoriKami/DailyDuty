using DailyDuty.Data.Enums;
using DailyDuty.Data.SettingsObjects;
using DailyDuty.Data.SettingsObjects.Weekly;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using Dalamud.Utility.Signatures;
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

        [Signature("0F B6 41 20 4C 8B C1")]
        private readonly delegate* unmanaged<byte*, int> getCustomDeliveryAllowances = null!;

        [Signature("48 8D 0D ?? ?? ?? ?? 41 0F BA EC", ScanType = ScanType.StaticAddress)]
        private readonly byte* staticArrayPointer = null!;

        public CustomDelivery()
        {
            SignatureHelper.Initialise(this);
        }

        public void Dispose()
        {

        }

        public bool IsCompleted() => GetAllowances() == 0;

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

        public void DisplayData() => Draw.NumericDisplay("Allowances Remaining", GetAllowances());

        //
        //  Implementation
        //

        private void PrintRemainingAllowances()
        {
            var allowances = GetAllowances();

            if (allowances > 0)
            {
                Chat.Print(HeaderText, $"{allowances} Allowances Remaining");
            }
        }

        private int GetAllowances()
        {
            return 12 - getCustomDeliveryAllowances(staticArrayPointer);
        }
    }
}