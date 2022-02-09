using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DailyDuty.Data.Enums;
using DailyDuty.Data.ModuleData.FashionReport;
using DailyDuty.Data.SettingsObjects;
using DailyDuty.Data.SettingsObjects.WeeklySettings;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using Condition = DailyDuty.Utilities.Condition;

namespace DailyDuty.Modules.Weekly;

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

    public FashionReport()
    {
        modeSelect = (int) Settings.Mode;
    }

    public bool IsCompleted()
    {
        return FashionReportComplete();
    }
    
    public void SendNotification()
    {
        if (Condition.IsBoundByDuty() == true) return;

        if (Settings.Mode == FashionReportMode.Single)
        {
            if (Settings.AllowancesRemaining == 4 && FashionReportAvailable())
            {
                Chat.Print(HeaderText, "Fashion Report Available");
            }
        }
        else if (Settings.Mode == FashionReportMode.All)
        {
            if (Settings.AllowancesRemaining > 0 && FashionReportAvailable())
            {
                Chat.Print(HeaderText, $"{Settings.AllowancesRemaining} Allowances Remaining");
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
        ImGui.SameLine();

        ImGui.Indent(125 * ImGuiHelpers.GlobalScale);

        ImGui.RadioButton($"All##{HeaderText}", ref modeSelect, (int) FashionReportMode.All);
        ImGuiComponents.HelpMarker("notify if any allowances remain this week and fashion report is available for turn-in");

        ImGui.Indent(-125 * ImGuiHelpers.GlobalScale);

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
    
    public void Dispose()
    {
    }

    void IResettable.ResetThis(CharacterSettings settings)
    {
        var fashionReportSettings = settings.FashionReport;

        fashionReportSettings.AllowancesRemaining = 4;
        fashionReportSettings.HighestWeeklyScore = 0;
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
        if (FashionReportAvailable() == false)
            return true;

        return Settings.Mode switch
        {
            FashionReportMode.Single => Settings.AllowancesRemaining < 4,
            FashionReportMode.All => Settings.AllowancesRemaining == 0,
            _ => false
        };
    }

    private static bool FashionReportAvailable()
    {
        var reportOpen = Time.NextWeeklyReset().AddDays(-4);
        var reportClosed = Time.NextWeeklyReset();

        var now = Time.Now();

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

            return availableDateTime - Time.Now();
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