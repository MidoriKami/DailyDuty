using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DailyDuty.ConfigurationSystem;
using DailyDuty.System.Utilities;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace DailyDuty.System.Modules
{
    internal unsafe class FashionReportModule : Module
    {
        private Weekly.FashionReportSettings Settings => Service.Configuration.CharacterSettingsMap[Service.Configuration.CurrentCharacter].FashionReportSettings;

        private bool exchangeStarted = false;
        
        public override void UpdateSlow()
        {
            if (Settings.Enabled)
            {
                // If we are occupied by talking to a quest npc
                if (Service.Condition[ConditionFlag.OccupiedInQuestEvent] == true)
                {
                    // If FashionReport Windows are open
                    if (GetFashionReportScoreGauge() != null && GetFashionReportInfoWindow() != null)
                    {
                        if (exchangeStarted == false)
                        {
                            var allowances = GetRemainingAllowances();

                            if (allowances != null)
                            {
                                exchangeStarted = true;

                                Settings.AllowancesRemaining = allowances.Value - 1;
                                Settings.HighestWeeklyScore = GetHighScore();
                                Service.Configuration.Save();
                            }
                        }
                    }
                }
                else
                {
                    exchangeStarted = false;
                }
            }
        }

        protected override void OnLoginDelayed()
        {
            if (Settings.Enabled && Settings.LoginReminder)
            {
                SendNotification();
            }
        }

        protected override void OnTerritoryChanged(object? sender, ushort e)
        {
            if (Settings.Enabled && Settings.TerritoryChangeReminder && ConditionManager.IsBoundByDuty() == false)
            {
                SendNotification();
            }
        }

        public override bool IsCompleted()
        {
            if (Settings.Mode == FashionReportMode.Single)
            {
                return Settings.AllowancesRemaining < 4;
            }
            else if(Settings.Mode == FashionReportMode.All)
            {
                return Settings.AllowancesRemaining == 0;
            }

            return false;
        }

        public override void DoDailyReset(Configuration.CharacterSettings settings)
        {
            // Not a daily thing
        }

        public override void DoWeeklyReset(Configuration.CharacterSettings settings)
        {
            var fashionReportSettings = settings.FashionReportSettings;

            fashionReportSettings.AllowancesRemaining = 4;
            fashionReportSettings.HighestWeeklyScore = 0;
        }

        private void SendNotification()
        {
            if (Settings.Mode == FashionReportMode.Single)
            {
                if (Settings.AllowancesRemaining == 4 && FashionReportAvailable())
                {
                    Util.PrintFashionReport("You have a Fashion Report Allowance available!");
                }
            }
            else if (Settings.Mode == FashionReportMode.All)
            {
                if (Settings.AllowancesRemaining > 0 && FashionReportAvailable())
                {
                    Util.PrintFashionReport($"You have {Settings.AllowancesRemaining} Fashion Report Allowances available!");
                }
            }
        }

        private static bool FashionReportAvailable()
        {
            var now = DateTime.UtcNow;
            var lastReset = Util.NextWeeklyReset().AddDays(-7);
            var delta = now - lastReset;

            return delta.Days >= 3;
        }

        public static TimeSpan TimeUntilFashionReport()
        {
            if (FashionReportAvailable() == true)
            {
                return TimeSpan.Zero;
            }
            else
            {
                var availableDateTime = Util.NextWeeklyReset().AddDays(-4);

                return DateTime.UtcNow - availableDateTime;
            }
        }

        private int? GetRemainingAllowances()
        {
            var pointer = GetFashionReportInfoWindow();
            if (pointer == null) return null;

            var textNode = (AtkTextNode*)pointer->GetNodeById(32);
            if (textNode == null) return null;

            var resultString = Regex.Match(textNode->NodeText.ToString(), @"\d+").Value;

            var number = int.Parse(resultString);

            return number;
        }

        private int GetHighScore()
        {
            var gaugeScore = GetGaugeScore() ?? 0;
            var windowScore = GetWindowScore() ?? 0;

            return Math.Max(gaugeScore, windowScore);
        }

        private int? GetGaugeScore()
        {
            var pointer = GetFashionReportScoreGauge();
            if (pointer == null) return null;

            var textNode = (AtkCounterNode*)pointer->GetNodeById(12);
            if(textNode == null) return null;

            var resultString = Regex.Match(textNode->NodeText.ToString(), @"\d+").Value;

            var number = int.Parse(resultString);

            return number;
        }

        private int? GetWindowScore()
        {
            var pointer = GetFashionReportInfoWindow();
            if (pointer == null) return null;

            var textNode = (AtkTextNode*)pointer->GetNodeById(33);
            if (textNode == null) return null;

            var resultString = Regex.Match(textNode->NodeText.ToString(), @"\d+").Value;

            var number = int.Parse(resultString);

            return number;
        }

        private AtkUnitBase* GetFashionReportScoreGauge()
        {
            return (AtkUnitBase*)Service.GameGui.GetAddonByName("FashionCheckScoreGauge", 1);
        }

        private AtkUnitBase* GetFashionReportInfoWindow()
        {
            return (AtkUnitBase*)Service.GameGui.GetAddonByName("FashionCheck", 1);
        }
    }
}
