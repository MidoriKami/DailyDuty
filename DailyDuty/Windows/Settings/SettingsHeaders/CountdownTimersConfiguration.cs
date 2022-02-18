using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Data.SettingsObjects;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.Windows.Settings.SettingsHeaders;

internal class CountdownTimersConfiguration : ICollapsibleHeader
{
    private CountdownTimerSettings Settings => Service.Configuration.TimerSettings;

    public void Dispose()
    {
            
    }

    public string HeaderText => "Countdown Timers Configuration";

    void ICollapsibleHeader.DrawContents()
    {
        ImGui.Indent(15 * ImGuiHelpers.GlobalScale);

        if (ImGui.BeginTable("CountdownTimersTable", 3))
        {
            ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed, 200f * ImGuiHelpers.GlobalScale);
            ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed, 50f * ImGuiHelpers.GlobalScale);
            ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed, 50f * ImGuiHelpers.GlobalScale);
            
            DrawDailyResetCountdownOptions();

            DrawWeeklyResetCountdownOptions();

            DrawFashionReportCountdownOptions();

            DrawTreasureMapCountdownOptions();

            DrawJumboCactpotCountdownOptions();

            ImGui.EndTable();
        }

        DrawCountdownWidthSlider();

        ImGui.Indent(-15 * ImGuiHelpers.GlobalScale);
    }

    private void DrawJumboCactpotCountdownOptions()
    {
        ImGui.TableNextRow();
        ImGui.TableNextColumn();
        Draw.NotificationField("Jumbo Cactpot", HeaderText, ref Settings.JumboCactpotCountdownEnabled,
            "Show/Hide Jumbo Cactpot Timer");

        ImGui.TableNextColumn();
        ImGui.ColorEdit4("##Jumbo Cactpot Bar Color", ref Settings.JumboCactpotCountdownColor,
            ImGuiColorEditFlags.NoInputs);
        
        ImGui.TableNextColumn();
        ImGui.ColorEdit4("##Jumbo Cactpot Bar Background Color", ref Settings.JumboCactpotCountdownBgColor,
            ImGuiColorEditFlags.NoInputs);
    }

    private void DrawTreasureMapCountdownOptions()
    {
        ImGui.TableNextRow();
        ImGui.TableNextColumn();
        Draw.NotificationField("Treasure Map", HeaderText, ref Settings.TreasureMapCountdownEnabled,
            "Show/Hide Treasure Map Timer");

        ImGui.TableNextColumn();
        ImGui.ColorEdit4("##Treasure Map Bar Color", ref Settings.TreasureMapCountdownColor,
            ImGuiColorEditFlags.NoInputs);
        
        ImGui.TableNextColumn();
        ImGui.ColorEdit4("##Treasure Map Bar Background Color", ref Settings.TreasureMapCountdownBgColor,
            ImGuiColorEditFlags.NoInputs);
    }

    private void DrawCountdownWidthSlider()
    {
        ImGui.Text("Bar Size");

        ImGui.SameLine();

        ImGui.PushItemWidth(175 * ImGuiHelpers.GlobalScale);
        ImGui.SliderInt("", ref Settings.TimerWidth, 86, 600);
        ImGui.PopItemWidth();
    }

    private void DrawFashionReportCountdownOptions()
    {
        ImGui.TableNextRow();
        ImGui.TableNextColumn();
        Draw.NotificationField("Fashion Report Reset", HeaderText, ref Settings.FashionReportCountdownEnabled,
            "Show/Hide Fashion Report Reset Timer");

        ImGui.TableNextColumn();
        ImGui.ColorEdit4("##Fashion Report Reset Bar Color", ref Settings.FashionReportCountdownColor,
            ImGuiColorEditFlags.NoInputs);
        
        ImGui.TableNextColumn();
        ImGui.ColorEdit4("##Fashion Report Reset Bar Background Color", ref Settings.FashionReportCountdownBgColor,
            ImGuiColorEditFlags.NoInputs);
    }

    private void DrawWeeklyResetCountdownOptions()
    {
        ImGui.TableNextRow();
        ImGui.TableNextColumn();
        Draw.NotificationField("Weekly Reset", HeaderText, ref Settings.WeeklyCountdownEnabled,
            "Show/Hide Weekly Reset Timer");

        ImGui.TableNextColumn();
        ImGui.ColorEdit4("##Weekly Reset Bar Color", ref Settings.WeeklyCountdownColor, 
            ImGuiColorEditFlags.NoInputs);
        
        ImGui.TableNextColumn();
        ImGui.ColorEdit4("##Weekly Reset Bar Background Color", ref Settings.WeeklyCountdownBgColor, 
            ImGuiColorEditFlags.NoInputs);
    }

    private void DrawDailyResetCountdownOptions()
    {
        ImGui.TableNextRow();
        ImGui.TableNextColumn();
        Draw.NotificationField("Daily Reset", HeaderText, ref Settings.DailyCountdownEnabled,
            "Show/Hide Daily Reset Timer");

        ImGui.TableNextColumn();
        ImGui.ColorEdit4("##Daily Reset Bar Color", ref Settings.DailyCountdownColor, 
            ImGuiColorEditFlags.NoInputs);
        
        ImGui.TableNextColumn();
        ImGui.ColorEdit4("##Daily Reset Bar Background Color", ref Settings.DailyCountdownBgColor, 
            ImGuiColorEditFlags.NoInputs);
    }
}