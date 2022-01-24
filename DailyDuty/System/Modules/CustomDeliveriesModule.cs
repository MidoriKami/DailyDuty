using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CheapLoc;
using DailyDuty.ConfigurationSystem;
using DailyDuty.System.Utilities;
using Dalamud.Logging;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.GeneratedSheets;
using Util = DailyDuty.System.Utilities.Util;

namespace DailyDuty.System.Modules
{
    internal unsafe class CustomDeliveriesModule : Module
    {
        private Weekly.CustomDeliveriesSettings Settings => Service.Configuration.CharacterSettingsMap[Service.Configuration.CurrentCharacter].CustomDeliveriesSettings;

        public CustomDeliveriesModule()
        {
            Service.ClientState.Login += OnLogin;
            Service.ClientState.TerritoryChanged += OnTerritoryChanged;
        }

        private void OnTerritoryChanged(object? sender, ushort e)
        {
            if (Settings.Enabled == false) return;
            if (ConditionManager.IsBoundByDuty() == true) return;

            if (Settings.NotificationEnabled == false) return;
            if (Service.LoggedIn == false) return;

            if (Settings.AllowancesRemaining > 0)
            {
                var locString = Loc.Localize("CDM_AllowancesRemaining", "You have {0} Allowances Remaining this week.");
                Util.PrintCustomDelivery(locString.Format(Settings.AllowancesRemaining));
            }
        }

        private void OnLogin(object? sender, EventArgs e)
        {
            Task.Delay(TimeSpan.FromSeconds(3)).ContinueWith(task => OnLoginDelayed());
        }

        private void OnLoginDelayed()
        {
            if (Settings.Enabled == false) return;

            var locString = Loc.Localize("CDM_AllowancesRemaining", "You have {0} Allowances Remaining this week.").Format(Settings.AllowancesRemaining);
            Util.PrintCustomDelivery(locString);
        }

        public override void Update()
        {
            if (Settings.Enabled == false) return;
            if (GetCustomDeliveryPointer() == null) return;

            var targetNPC = GetIDForNPC();
            if (targetNPC == null) return;

            // If we haven't see this NPC before
            if (!Settings.DeliveryNPC.ContainsKey(targetNPC.Value))
            {
                var deliveryCount = GetRemainingDeliveriesCount();
                if (deliveryCount == null) return;

                // Log NPC with delivery count
                Settings.DeliveryNPC.Add(targetNPC.Value, deliveryCount.Value);
                Service.Configuration.Save();
            }
            // If we have seen this NPC before
            else
            {
                var deliveryCount = GetRemainingDeliveriesCount();
                if (deliveryCount == null) return;

                // Update the delivery count
                Settings.DeliveryNPC[targetNPC.Value] = deliveryCount.Value;
            }

            base.Update();
        }

        private uint? GetIDForNPC()
        {
            var pointer = GetCustomDeliveryPointer();
            if (pointer == null) return null;

            var textNode = (AtkTextNode*)((AtkUnitBase*)pointer)->GetNodeById(36);
            if (textNode == null) return null;

            var nodeText = textNode->NodeText.ToString();
            if (nodeText == string.Empty) return null;

            var npcID = Service.DataManager.GetExcelSheet<NotebookDivision>()
                !.Where(r => r.Name == nodeText)
                .Select(r => r.RowId)
                .FirstOrDefault();

            return npcID;
        }

        public override void Dispose()
        {
            Service.ClientState.Login -= OnLogin;
            Service.ClientState.TerritoryChanged -= OnTerritoryChanged;
        }

        public override bool IsCompleted()
        {
            return Settings.AllowancesRemaining == 0;
        }

        public override void DoDailyReset()
        {
            // Custom Deliveries is a Weekly Task
        }

        public override void DoWeeklyReset()
        {
            foreach (var (_, settings) in Service.Configuration.CharacterSettingsMap)
            {
                var customDeliveriesSettings = settings.CustomDeliveriesSettings;

                foreach (var key in customDeliveriesSettings.DeliveryNPC.Keys.ToList())
                {
                    customDeliveriesSettings.DeliveryNPC[key] = 6;
                }
            }

            Service.Configuration.Save();
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
