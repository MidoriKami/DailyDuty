using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;

namespace DailyDuty.Utilities
{
    internal static class Draw
    {
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
    }
}