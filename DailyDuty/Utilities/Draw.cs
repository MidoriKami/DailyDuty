using System;
using System.Numerics;
using DailyDuty.Enums;
using DailyDuty.Localization;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;

namespace DailyDuty.Utilities
{
    internal static class Draw
    {
        public static bool Checkbox(string label, ref bool refValue, string helpText = "")
        {
            var result = ImGui.Checkbox($"{label}", ref refValue);

            if (helpText != string.Empty)
            {
                ImGuiComponents.HelpMarker(helpText);
            }

            return result;
        }

        public static void CompleteIncomplete(bool complete)
        {
            ConditionalText(complete, Strings.Common.CompleteLabel, Strings.Common.IncompleteLabel);
        }

        public static void CompletionStatus(CompletionStatus status)
        {
            var region = ImGui.GetContentRegionAvail();

            var text = string.Empty;
            var color = Colors.Red;

            switch (status)
            {
                case Enums.CompletionStatus.Complete:
                    text = Strings.Common.CompleteLabel;
                    color = Colors.Green;
                    break;

                case Enums.CompletionStatus.Incomplete:
                    text = Strings.Common.IncompleteLabel;
                    color = Colors.Red;
                    break;

                case Enums.CompletionStatus.Unavailable:
                    text = Strings.Common.UnavailableLabel;
                    color = Colors.Orange;
                    break;
            }

            var textSize = ImGui.CalcTextSize(text);

            ImGui.SameLine(region.X - textSize.X - 10.0f * ImGuiHelpers.GlobalScale);
            ImGui.TextColored(color, text);
        }

        public static void CompletionStatus(bool complete)
        {
            CompletionStatus(complete ? Enums.CompletionStatus.Complete : Enums.CompletionStatus.Incomplete);
        }

        public static void TextRightAligned(string text, Vector4? color = null)
        {
            var region = ImGui.GetContentRegionAvail();
            var textSize = ImGui.CalcTextSize(text);

            ImGui.SameLine(region.X - textSize.X - 10.0f * ImGuiHelpers.GlobalScale);
            if (color == null)
            {
                ImGui.Text(text);
            }
            else
            {
                ImGui.TextColored(color.Value, text);
            }
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
        
        public static void Rectangle(Vector2 position, Vector2 size, float thickness)
        {
            var drawList = ImGui.GetWindowDrawList();
            var color = ImGui.GetColorU32(Colors.White);

            drawList.AddRect(position, position + size, color, 5.0f, ImDrawFlags.None, thickness);
        }

        public static void VerticalLine()
        {
            var contentArea = ImGui.GetContentRegionAvail();
            var cursor = ImGui.GetCursorScreenPos();
            var drawList = ImGui.GetWindowDrawList();
            var color = ImGui.GetColorU32(Colors.White);

            drawList.AddLine(cursor, cursor with {Y = cursor.Y + contentArea.Y}, color, 1.0f);
        }

        public static string Format(this TimeSpan span, bool showSeconds = true)
        {
            string result = "";

            if (span.Days > 0)
            {
                if (span.Days == 1)
                {
                    result = $"{span.Days} day, ";
                }
                else if (span.Days > 1)
                {
                    result = $"{span.Days} days, ";
                }
            }

            result += $"{span.Hours:00}:{span.Minutes:00}";

            if (showSeconds)
            {
                result += $":{span.Seconds:00}";
            }

            return result;
        }
    }
}