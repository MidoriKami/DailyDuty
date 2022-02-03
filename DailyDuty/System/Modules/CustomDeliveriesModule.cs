using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using DailyDuty.ConfigurationSystem;
using DailyDuty.System.Utilities;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.GeneratedSheets;
using Util = DailyDuty.System.Utilities.Util;

namespace DailyDuty.System.Modules
{
    internal unsafe class CustomDeliveriesModule : Module
    {
        private Weekly.CustomDeliveriesSettings Settings => Service.Configuration.CharacterSettingsMap[Service.Configuration.CurrentCharacter].CustomDeliveriesSettings;

        private bool exchangeStarted = false;
        private uint lastDeliveryCount = 0;

        protected override void ThrottledOnTerritoryChanged(object? sender, ushort e)
        {
            if (ConditionManager.IsBoundByDuty()) return;

            if (Settings.Enabled && Settings.TerritoryChangeReminder)
            {
                PrintRemainingAllowances();
            }
        }

        private void PrintRemainingAllowances()
        {
            if (Settings.AllowancesRemaining > 0)
            {
                Util.PrintCustomDelivery($"Remaining Allowances: {Settings.AllowancesRemaining}");
            }
        }

        protected override void OnLoginDelayed()
        {
            if (Settings.Enabled && Settings.LoginReminder)
            {
                PrintRemainingAllowances();
            }
        }

        public override void Update()
        {
            if (Settings.Enabled)
            {
                // If we are occupied by talking to a quest npc
                if (Service.Condition[ConditionFlag.OccupiedInQuestEvent] == true)
                {
                    // If a custom delivery window is open
                    if (GetCustomDeliveryPointer() != null)
                    {
                        StartCustomDeliveryExchange();
                    }
                    // If we started an exchange, check for cutscene event
                    if (Service.Condition[ConditionFlag.OccupiedInCutSceneEvent] == true && exchangeStarted == true)
                    {
                        Settings.AllowancesRemaining -= 1;
                        Service.Configuration.Save();
                        
                        exchangeStarted = false;
                    }
                }
                // End the exchange when we are no longer locked by OccupiedInQuestEvent
                else if(exchangeStarted == true)
                {
                    exchangeStarted = false;
                }
            }
        }

        private void StartCustomDeliveryExchange()
        {
            var count = GetRemainingDeliveriesCount();
            if (count == null) return;

            if (exchangeStarted == false)
            {
                exchangeStarted = true;
                lastDeliveryCount = count.Value;
            }
            else if (exchangeStarted == true)
            {
                if (count.Value != lastDeliveryCount)
                {
                    lastDeliveryCount = count.Value;

                    Settings.AllowancesRemaining -= 1;
                    Service.Configuration.Save();
                }
            }
        }

        public override bool IsCompleted()
        {
            return Settings.AllowancesRemaining == 0;
        }

        public override void DoDailyReset(Configuration.CharacterSettings settings)
        {
            // Custom Deliveries is a Weekly Task
        }

        public override void DoWeeklyReset(Configuration.CharacterSettings settings)
        {
            var customDeliveriesSettings = settings.CustomDeliveriesSettings;

            customDeliveriesSettings.AllowancesRemaining = 12;
        }

        private uint? GetRemainingDeliveriesCount()
        {
            var pointer = GetCustomDeliveryPointer();
            if (pointer == null) return null;

            var textNode = (AtkTextNode*) ((AtkUnitBase*) pointer)->GetNodeById(34);
            if (textNode == null) return null;

            var nodeText = textNode->NodeText.ToString();
            if(nodeText == string.Empty) return null;

            var resultString = Regex.Match(textNode->NodeText.ToString(), @"\d+").Value;

            uint number = uint.Parse(resultString);

            return number;
        }

        private AtkResNode* GetCustomDeliveryPointer()
        {
            return (AtkResNode*)Service.GameGui.GetAddonByName("SatisfactionSupply", 1);
        }
    }
}
