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

        public static void NumericDisplay(string label, string formattedString)
        {
            ImGui.Text(label);
            ImGui.SameLine();
            ImGui.Text(formattedString);
        }

        public static void NumericDisplay(string label, int value, Vector4 color)
        {
            ImGui.Text(label);
            ImGui.SameLine();
            ImGui.TextColored(color, $"{value}");
        }

        public static void OnLoginReminderCheckbox(GenericSettings settings)
        {
            ImGui.Checkbox($"Login Reminder", ref settings.LoginReminder);
            ImGuiComponents.HelpMarker("Display this module's status in chat on login if this module is incomplete");
        }

        public static void OnTerritoryChangeCheckbox(GenericSettings settings)
        {
            ImGui.Checkbox($"Zone Change Reminder", ref settings.ZoneChangeReminder);
            ImGuiComponents.HelpMarker("Display this module's status in chat on any non-duty instance change if this module is incomplete");
        }

        public static void EditNumberField(string label, ref int refValue)
        {
            EditNumberField(label, 30, ref refValue);
        }

        public static void EditNumberField(string label, float fieldWidth, ref int refValue)
        {
            ImGui.Text(label);

            ImGui.SameLine();

            ImGui.PushItemWidth(fieldWidth * ImGuiHelpers.GlobalScale);
            ImGui.InputInt($"##{label}", ref refValue, 0, 0);
            ImGui.PopItemWidth();
        }

        public static void Checkbox(string label, ref bool refValue, string helpText = "")
        {
            ImGui.Checkbox($"{label}", ref refValue);

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

        public static void DrawWordWrappedString(string message)
        {
            var words = message.Split(' ');

            var windowWidth = ImGui.GetContentRegionAvail().X;
            var cumulativeSize = 0.0f;
            var padding = 2.0f;

            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(2.0f, 0.0f));

            foreach (var word in words)
            {
                var wordWidth = ImGui.CalcTextSize(word).X;

                if (cumulativeSize == 0)
                {
                    ImGui.Text(word);
                    cumulativeSize += wordWidth + padding;
                }
                else if ((cumulativeSize + wordWidth) < windowWidth)
                {
                    ImGui.SameLine();
                    ImGui.Text(word);
                    cumulativeSize += wordWidth + padding;
                }
                else if ((cumulativeSize + wordWidth) >= windowWidth)
                {
                    ImGui.Text(word);
                    cumulativeSize = wordWidth + padding;
                }
            }

            ImGui.PopStyleVar();
        }
    }
}