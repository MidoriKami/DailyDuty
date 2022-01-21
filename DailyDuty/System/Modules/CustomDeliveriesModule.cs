using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using DailyDuty.ConfigurationSystem;
using DailyDuty.System.Utilities;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace DailyDuty.System.Modules
{
    internal unsafe class CustomDeliveriesModule : Module
    {
        private Weekly.CustomDeliveriesSettings Settings => Service.Configuration.CharacterSettingsMap[Service.Configuration.CurrentCharacter].CustomDeliveriesSettings;

        private int lastDeliveriesCount = -1;
        private readonly Stopwatch loginNoticeStopwatch = new();

        public CustomDeliveriesModule()
        {
            Service.ClientState.Login += OnLogin;
            Service.ClientState.TerritoryChanged += OnTerritoryChanged;
        }

        private void OnTerritoryChanged(object? sender, ushort e)
        {
            if (Settings.Enabled == false) return;

            lastDeliveriesCount = -1;
            if (Settings.NotificationEnabled == false) return;

            if (Settings.AllowancesRemaining > 0)
            {
                Util.PrintCustomDelivery($"You have {Settings.AllowancesRemaining} Allowances Remaining this week.");
            }
        }

        private void OnLogin(object? sender, EventArgs e)
        {
            if (Settings.Enabled == false) return;
            if (Settings.NotificationEnabled == false) return;

            loginNoticeStopwatch.Start();
        }

        public override void Update()
        {
            if (Settings.Enabled == false) return;
            if (GetCustomDeliveryPointer() == null) return;

            var frameCount = Service.PluginInterface.UiBuilder.FrameCount;
            if (frameCount % 10 != 0) return;

            if (loginNoticeStopwatch.Elapsed >= TimeSpan.FromSeconds(5) && loginNoticeStopwatch.IsRunning && Service.LoggedIn == true)
            {
                if (Settings.AllowancesRemaining > 0)
                {
                    Util.PrintCustomDelivery($"You have {Settings.AllowancesRemaining} Allowances Remaining this week.");
                }

                loginNoticeStopwatch.Stop();
                loginNoticeStopwatch.Reset();
            }


            if (lastDeliveriesCount == -1)
            {
                var newValue = GetRemainingDeliveriesCount();

                if (newValue != null)
                {
                    lastDeliveriesCount = newValue.Value;
                }
            }
            else
            {
                var newValue = GetRemainingDeliveriesCount();

                if (newValue != null)
                {
                    if (newValue != lastDeliveriesCount)
                    {
                        Settings.AllowancesRemaining -= 1;
                        lastDeliveriesCount = newValue.Value;
                        Service.Configuration.Save();
                    }
                }
            }
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
            foreach (var (character, settings) in Service.Configuration.CharacterSettingsMap)
            {
                var customDeliveriesSettings = settings.CustomDeliveriesSettings;
                customDeliveriesSettings.AllowancesRemaining = 12;
            }
        }

        private int? GetRemainingDeliveriesCount()
        {
            var pointer = GetCustomDeliveryPointer();
            if (pointer == null) return null;

            var textNode = (AtkTextNode*) ((AtkUnitBase*) pointer)->GetNodeById(34);

            var resultString = Regex.Match(textNode->NodeText.ToString(), @"\d+").Value;

            int number = int.Parse(resultString);

            return number;
        }

        private AtkResNode* GetCustomDeliveryPointer()
        {
            return (AtkResNode*)Service.GameGui.GetAddonByName("SatisfactionSupply", 1);
        }
    }
}
