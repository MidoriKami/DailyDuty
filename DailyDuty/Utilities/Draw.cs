using System;
using System.Numerics;
using DailyDuty.Data.Graphical;
using DailyDuty.Data.SettingsObjects;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;

namespace DailyDuty.Utilities
{
    internal static class Draw
    {

        public static void Timer(TimerStyle style, TimerData data)
        {
            ImGui.PushStyleColor(ImGuiCol.FrameBg, style.BackgroundColor);
            ImGui.PushStyleColor(ImGuiCol.PlotHistogram, style.ForegroundColor);

            ImGui.BeginGroup();

            var cursorStart = ImGui.GetCursorPos();
            ImGui.ProgressBar(data.Percentage, new(style.Size, 20), "");

            ImGui.SetCursorPos(new Vector2(cursorStart.X + style.Padding, cursorStart.Y));

            var formatString = GetFormatString(style, data);
            ImGui.TextColored(style.TextColor, formatString);

            ImGui.EndGroup();
            ImGui.PopStyleColor(2);
        }

        private static string GetFormatString(TimerStyle style, TimerData data)
        {
            var formatString = style.Options.UseShortName ? data.NameShort : data.NameLong;

            formatString += data.RemainingTime == TimeSpan.Zero
                ? ": " + data.CompletionString
                : ": " + data.RemainingTime.Format(style.Options);

            return formatString;
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

        public static void Checkbox(string label, string categoryString, ref bool refValue, string helpText = "")
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
                ImGui.TextColored(new(0, 255, 0, 255), $"{span.Format()}");
            }
            else
            {
                ImGui.Text($" {span.Format()}");
            }
        }

        public static void CompleteIncomplete(bool complete)
        {
            ConditionalText(complete, "Complete", "Incomplete");
        }

        public static void ConditionalText(bool condition, string trueString, string falseString)
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
}