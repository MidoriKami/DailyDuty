using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.Utilities
{
    internal static class Draw
    {
        public static void DrawProgressBar(float percentage, string prependText, TimeSpan remainingTime, Vector2 size, Vector4 barColor)
        {
            ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0, 0, 0, 1));
            ImGui.PushStyleColor(ImGuiCol.PlotHistogram, barColor);

            ImGui.ProgressBar(percentage, ImGuiHelpers.ScaledVector2(200, 20),
                remainingTime.Days > 0
                    ? $"{prependText}: {remainingTime.FormatDays()}"
                    : $"{prependText}: {remainingTime.FormatHours()}");

            ImGui.PopStyleColor(2);
        }
    }
}
