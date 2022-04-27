using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DailyDuty.Data.Components;
using DailyDuty.Data.ModuleSettings;
using DailyDuty.Enums;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.Utilities;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Condition = DailyDuty.Utilities.Condition;

namespace DailyDuty.Modules
{
    internal unsafe class FashionReportModule :
        IUpdateable,
        IResettable,
        ILoginNotification,
        IZoneChangeThrottledNotification,
        ICompletable
    {
        public GenericSettings GenericSettings => Settings;
        public CompletionType Type => CompletionType.Weekly;
        public static FashionReportSettings Settings => Service.CharacterConfiguration.FashionReport;

        private readonly DalamudLinkPayload goldSaucerTeleport;
        private bool exchangeStarted;

        public FashionReportModule()
        {
            goldSaucerTeleport = Service.TeleportManager.GetPayload(TeleportPayloads.GoldSaucerTeleport);
        }

        public void Update()
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
                                Service.CharacterConfiguration.Save();
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

        public void SendNotification()
        {
            if (!IsCompleted() && !Condition.IsBoundByDuty())
            {
                switch(Settings.Mode)
                {
                    case FashionReportMode.Single:
                        if(Settings.AllowancesRemaining == 4)
                        {
                            Chat.Print(Strings.Module.FashionReportLabel, Strings.Common.AvailableNowLabel, goldSaucerTeleport);
                        }
                        break;
                    case FashionReportMode.Plus80:
                        if (Settings.HighestWeeklyScore < 80)
                        {
                            Chat.Print(Strings.Module.FashionReportLabel, $"{Settings.AllowancesRemaining} " + Strings.Common.AllowancesRemainingLabel, goldSaucerTeleport);
                            Chat.Print(Strings.Module.FashionReportLabel,$"{Settings.HighestWeeklyScore} " + Strings.Module.FashionReportHighestScoreLabel, goldSaucerTeleport);
                        }
                        break;
                    case FashionReportMode.All:
                        Chat.Print(Strings.Module.FashionReportLabel, $"{Settings.AllowancesRemaining} " + Strings.Common.AllowancesRemainingLabel, goldSaucerTeleport);
                        break;
                }
            }
        }

        DateTime IResettable.GetNextReset() => Time.NextWeeklyReset();

        void IResettable.ResetThis()
        {
            Settings.AllowancesRemaining = 4;
            Settings.HighestWeeklyScore = 0;
        }

        public bool IsCompleted() => FashionReportComplete();

        private bool FashionReportComplete()
        {
            if (FashionReportAvailable() == false) return true;

            // Zero is always "Complete"
            // Four is always "Incomplete"
            if (Settings.AllowancesRemaining == 0) return true;
            if (Settings.AllowancesRemaining == 4) return false;

            // If this line is reached, then we have between 1 and 3 remaining allowances (inclusive)
            switch (Settings.Mode)
            {
                case FashionReportMode.Single:
                case FashionReportMode.All when Settings.AllowancesRemaining == 0:
                case FashionReportMode.Plus80 when Settings.HighestWeeklyScore >= 80:
                    return true;

                default:
                    return false;
            }
        }

        public bool FashionReportAvailable()
        {
            var reportOpen = Time.NextFashionReportReset();
            var reportClosed = Time.NextWeeklyReset();

            var now = DateTime.UtcNow;

            return now > reportOpen && now < reportClosed;
        }

        private int? GetRemainingAllowances()
        {
            var pointer = GetFashionReportInfoWindow();
            if (pointer == null) return null;

            var textNode = (AtkTextNode*)pointer->GetNodeById(32);
            if (textNode == null) return null;

            var resultString = Regex.Match(textNode->NodeText.ToString(), @"\d+").Value;

            var number = int.Parse(resultString);
            if (number == 0) return null;

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
