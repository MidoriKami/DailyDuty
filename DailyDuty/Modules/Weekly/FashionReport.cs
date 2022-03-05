using System;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;
using DailyDuty.Data.Enums;
using DailyDuty.Data.ModuleData.FashionReport;
using DailyDuty.Data.SettingsObjects;
using DailyDuty.Data.SettingsObjects.Weekly;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using Condition = DailyDuty.Utilities.Condition;

namespace DailyDuty.Modules.Weekly
{
    internal unsafe class FashionReport : 
        IConfigurable, 
        IUpdateable,
        IWeeklyResettable,
        ILoginNotification,
        IZoneChangeThrottledNotification,
        ICompletable
    {
        private bool exchangeStarted;

        private FashionReportSettings Settings => Service.Configuration.Current().FashionReport;
        public CompletionType Type => CompletionType.Weekly;
        public string HeaderText => "Fashion Report";
        public GenericSettings GenericSettings => Settings;
        public DateTime NextReset
        {
            get => Settings.NextReset;
            set => Settings.NextReset = value;
        }
        private int modeSelect;

        private readonly DalamudLinkPayload goldSaucerTeleport;

        public FashionReport()
        {
            modeSelect = (int) Settings.Mode;

            goldSaucerTeleport = Service.TeleportManager.GetPayload(TeleportPayloads.GoldSaucerTeleport);
        }

        public void Dispose()
        {
        }

        public bool IsCompleted()
        {
            return FashionReportComplete();
        }
    
        public void SendNotification()
        {
            if (Condition.IsBoundByDuty() == true) return;

            if(FashionReportAvailable() && Settings.AllowancesRemaining > 0)
            {
                switch(Settings.Mode)
                {
                    case FashionReportMode.Single:
                        if(Settings.AllowancesRemaining == 4)
                        {
                            Chat.Print(HeaderText, "Fashion Report Available", goldSaucerTeleport);
                        }
                        break;
                    case FashionReportMode.Plus80:
                        if (Settings.HighestWeeklyScore < 80)
                        {
                            Chat.Print(HeaderText, $"{Settings.AllowancesRemaining} Allowances Remaining, Highest score: {Settings.HighestWeeklyScore}", goldSaucerTeleport);
                        }
                        break;
                    case FashionReportMode.All:
                        Chat.Print(HeaderText, $"{Settings.AllowancesRemaining} Allowances Remaining", goldSaucerTeleport);
                        break;
                }
            }
        }

        public void NotificationOptions()
        {
            Draw.OnLoginReminderCheckbox(Settings, HeaderText);

            Draw.OnTerritoryChangeCheckbox(Settings, HeaderText);

            NotificationModeToggle();
        }

        private void NotificationModeToggle()
        {
            ImGui.Text("Notification Mode");

            ImGui.Indent(15 * ImGuiHelpers.GlobalScale);

            ImGui.RadioButton($"Single##{HeaderText}", ref modeSelect, (int)FashionReportMode.Single);
            ImGuiComponents.HelpMarker("Only notify if no allowances have been spent this week and fashion report is available for turn-in");

            ImGui.RadioButton($"80 Plus##{HeaderText}", ref modeSelect, (int)FashionReportMode.Plus80);
            ImGuiComponents.HelpMarker("Notify if any allowances remain this week, the highest score is below 80 and fashion report is available for turn-in");

            ImGui.RadioButton($"All##{HeaderText}", ref modeSelect, (int) FashionReportMode.All);
            ImGuiComponents.HelpMarker("Notify if any allowances remain this week and fashion report is available for turn-in");

            ImGui.Indent(-15 * ImGuiHelpers.GlobalScale);


            if (Settings.Mode != (FashionReportMode) modeSelect)
            {
                Settings.Mode = (FashionReportMode) modeSelect;
                Service.Configuration.Save();
            }
        }

        public void EditModeOptions()
        {
            Draw.EditNumberField("Allowances",HeaderText, ref Settings.AllowancesRemaining);

            Draw.EditNumberField("Highest Score",HeaderText, ref Settings.HighestWeeklyScore);
        }

        public void DisplayData()
        {
            Draw.NumericDisplay("Remaining Allowances", Settings.AllowancesRemaining);

            Draw.NumericDisplay("Highest Score This Week", Settings.HighestWeeklyScore);

            Draw.TimeSpanDisplay("Time Until Fashion Report", TimeUntilFashionReport() );
        }

        public void Update()
        {
            UpdateFashionReport();
        }
    
        void IResettable.ResetThis()
        {
            Settings.AllowancesRemaining = 4;
            Settings.HighestWeeklyScore = 0;
        }

        //
        // Implementation
        //
        private void UpdateFashionReport()
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

        private static bool FashionReportAvailable()
        {
            var reportOpen = Time.NextWeeklyReset().AddDays(-4);
            var reportClosed = Time.NextWeeklyReset();

            var now = DateTime.UtcNow;

            return now > reportOpen && now < reportClosed;
        }

        private static TimeSpan TimeUntilFashionReport()
        {
            if (FashionReportAvailable() == true)
            {
                return TimeSpan.Zero;
            }
            else
            {
                var availableDateTime = Time.NextWeeklyReset().AddDays(-4);

                return availableDateTime - DateTime.UtcNow;
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