using System;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;

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

        public static void Checkbox(string label, ref bool refValue, string helpText = "")
        {
            ImGui.Checkbox($"{label}", ref refValue);

            if (helpText != string.Empty)
            {
                ImGuiComponents.HelpMarker(helpText);
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

        public static void InfoBox(Vector2 size, string label, string contents)
        {
            var drawList = ImGui.GetWindowDrawList();
            var color = ImGui.GetColorU32(Colors.White);
            var startPosition = ImGui.GetCursorScreenPos();

            size *= ImGuiHelpers.GlobalScale;
            var curveRadius = 20.0f * ImGuiHelpers.GlobalScale;
            var thickness = 2.0f  * ImGuiHelpers.GlobalScale;
            var segmentResolution = 10;

            // Body Text
            ImGui.SetCursorPos(new Vector2(curveRadius + 2.0f, curveRadius));
            ImGui.PushTextWrapPos(size.X - (curveRadius * 0.5f));
            ImGui.Text(contents);
            size.Y = ImGui.GetItemRectMax().Y - ImGui.GetItemRectMin().Y + curveRadius;
            ImGui.PopTextWrapPos();

            var topLeftCurveCenter = new Vector2(startPosition.X + curveRadius, startPosition.Y + curveRadius);
            var topRightCurveCenter = new Vector2(startPosition.X + size.X - curveRadius, startPosition.Y + curveRadius);
            var bottomLeftCurveCenter = new Vector2(startPosition.X + curveRadius, startPosition.Y + size.Y - curveRadius);
            var bottomRightCurveCenter = new Vector2(startPosition.X + size.X - curveRadius, startPosition.Y + size.Y - curveRadius);

            drawList.PathArcTo(topLeftCurveCenter, curveRadius, DegreesToRadians(180), DegreesToRadians(270), segmentResolution);
            drawList.PathStroke(color, ImDrawFlags.None, thickness);

            drawList.PathArcTo(topRightCurveCenter, curveRadius, DegreesToRadians(360), DegreesToRadians(270), segmentResolution);
            drawList.PathStroke(color, ImDrawFlags.None, thickness);

            drawList.PathArcTo(bottomLeftCurveCenter, curveRadius, DegreesToRadians(90), DegreesToRadians(180), segmentResolution);
            drawList.PathStroke(color, ImDrawFlags.None, thickness);

            drawList.PathArcTo(bottomRightCurveCenter, curveRadius, DegreesToRadians(0), DegreesToRadians(90), segmentResolution);
            drawList.PathStroke(color, ImDrawFlags.None, thickness);

            // Left Line
            drawList.AddLine(new Vector2(startPosition.X - 0.5f, startPosition.Y + curveRadius), new Vector2(startPosition.X - 0.5f, startPosition.Y + size.Y - curveRadius), color, thickness);

            // Right Line
            drawList.AddLine(new Vector2(startPosition.X + size.X - 0.5f, startPosition.Y + curveRadius), new Vector2(startPosition.X + size.X - 0.5f, startPosition.Y + size.Y - curveRadius), color, thickness);

            // Bottom Line
            drawList.AddLine(new Vector2(startPosition.X + curveRadius, startPosition.Y + size.Y - 0.5f), new Vector2(startPosition.X + size.X - curveRadius, startPosition.Y + size.Y - 0.5f), color, thickness);

            // Top Line
            var textSize = ImGui.CalcTextSize(label);
            var textStartPadding = 7.0f * ImGuiHelpers.GlobalScale;
            var textEndPadding = 7.0f * ImGuiHelpers.GlobalScale;
            var textVerticalOffset = textSize.Y / 2.0f;

            drawList.AddText(new Vector2(startPosition.X + curveRadius + textStartPadding, startPosition.Y - textVerticalOffset), color, label);
            drawList.AddLine(new Vector2(startPosition.X + curveRadius + textStartPadding + textSize.X + textEndPadding, startPosition.Y - 0.5f), new Vector2(startPosition.X + size.X - curveRadius, startPosition.Y - 0.5f), color, thickness);
        }

        public static float DegreesToRadians(double degrees)
        {
            float radians = (float)((Math.PI / 180) * degrees);
            return (radians);
        }
    }
}