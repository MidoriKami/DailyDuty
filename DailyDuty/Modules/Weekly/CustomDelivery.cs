using System;
using System.Text.RegularExpressions;
using DailyDuty.Data.Enums;
using DailyDuty.Data.SettingsObjects;
using DailyDuty.Data.SettingsObjects.Weekly;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using Dalamud.Game.ClientState.Conditions;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Condition = DailyDuty.Utilities.Condition;

namespace DailyDuty.Modules.Weekly
{
    internal class CustomDelivery : 
        IConfigurable, 
        IZoneChangeThrottledNotification,
        ILoginNotification,
        ICompletable,
        IWeeklyResettable
    {
        private CustomDeliverySettings Settings => Service.Configuration.Current().CustomDelivery;
        public CompletionType Type => CompletionType.Weekly;
        public string HeaderText => "Custom Delivery";
        public GenericSettings GenericSettings => Settings;

        public DateTime NextReset
        {
            get => Settings.NextReset;
            set => Settings.NextReset = value;
        }

        public bool IsCompleted()
        {
            return Settings.AllowancesRemaining == 0;
        }

        public void SendNotification()
        {
            if (Condition.IsBoundByDuty() == true) return;

            PrintRemainingAllowances();
        }

        void IResettable.ResetThis()
        {
            Settings.AllowancesRemaining = 12;
        }

        public void NotificationOptions()
        {        
            Draw.OnLoginReminderCheckbox(Settings);

            Draw.OnTerritoryChangeCheckbox(Settings);
        }

        public void EditModeOptions()
        {
            Draw.EditNumberField("Allowances", ref Settings.AllowancesRemaining);
        }

        public void DisplayData()
        {
            Draw.NumericDisplay("Allowances Remaining", Settings.AllowancesRemaining);
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
                Chat.Print(HeaderText, $"{Settings.AllowancesRemaining} Allowances Remaining");
            }
        }
    }
}