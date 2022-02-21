using System;
using System.Numerics;
using DailyDuty.Data.SettingsObjects;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;

namespace DailyDuty.Utilities;

internal static class Draw
{
    public static void DrawProgressBar(float percentage, string prependText, TimeSpan remainingTime, Vector2 size, Vector4 barColor, Vector4 bgColor)
    {
        ImGui.PushStyleColor(ImGuiCol.FrameBg, bgColor);
        ImGui.PushStyleColor(ImGuiCol.PlotHistogram, barColor);

        if (remainingTime > TimeSpan.Zero)
        {
            ImGui.ProgressBar(percentage, size, remainingTime.Days > 0
                ? $"{prependText}: {remainingTime.FormatDays()}"
                : $"{prependText}: {remainingTime.FormatHours()}");
        }
        else
        {
            ImGui.ProgressBar(percentage, size, prependText + " Available");
        }

        ImGui.PopStyleColor(2);
    }

    public static void NumericDisplay(string label, int value)
    {
        ImGui.Text(label);
        ImGui.SameLine();
        ImGui.Text($"{value}");
    }

    public static void NumericDisplay(string label, int value, Vector4 color)
    {
        ImGui.Text(label);
        ImGui.SameLine();
        ImGui.TextColored(color, $"{value}");
    }

    public static void OnLoginReminderCheckbox(GenericSettings settings, string categoryString)
    {
        ImGui.Checkbox($"Login Reminder##{categoryString}", ref settings.LoginReminder);
        ImGuiComponents.HelpMarker("Display this module's status in chat on login if this module is incomplete");
    }

    public static void OnTerritoryChangeCheckbox(GenericSettings settings, string categoryString)
    {
        ImGui.Checkbox($"Zone Change Reminder##{categoryString}", ref settings.ZoneChangeReminder);
        ImGuiComponents.HelpMarker("Display this module's status in chat on any non-duty instance change if this module is incomplete");
    }

    public static void EditNumberField(string label, string categoryString, ref int refValue)
    {
        ImGui.Text(label);

        ImGui.SameLine();

        ImGui.PushItemWidth(30 * ImGuiHelpers.GlobalScale);
        ImGui.InputInt($"##{label}{categoryString}", ref refValue, 0, 0);
        ImGui.PopItemWidth();
    }

    public static void NotificationField(string label, string categoryString, ref bool refValue, string helpText = "")
    {
        ImGui.Checkbox($"{label}##{categoryString}", ref refValue);

        if (helpText != string.Empty)
        {
            ImGuiComponents.HelpMarker(helpText);
        }
    }

    public static void TimeSpanDisplay(string label, TimeSpan span)
    {
        ImGui.Text(label);
        ImGui.SameLine();
            
        if (span == TimeSpan.Zero)
        {
            ImGui.TextColored(new(0, 255, 0, 255), $"{span.FormatAuto()}");
        }
        else
        {
            ImGui.Text($" {span.FormatAuto()}");
        }
    }

    public static void PrintCompleteIncomplete(bool complete)
    {
        DrawConditionalText(complete, "Complete", "Incomplete");
    }

    public static void DrawConditionalText(bool condition, string trueString, string falseString)
    {
        if (condition)
        {
            ImGui.TextColored(new Vector4(0, 255, 0, 0.8f), trueString);
        }
        else
        {
            ImGui.TextColored(new Vector4(185, 0, 0, 0.8f), falseString);
        }
    }
}