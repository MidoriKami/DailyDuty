using System;
using System.Numerics;
using DailyDuty.Data.Components;
using DailyDuty.Interfaces;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.Timers
{
    public class CountdownTimer : IDrawable
    {
        private static MainWindowSettings Settings => Service.SystemConfiguration.Windows.MainWindow;
        public string Label { get; init; } = "Label Not Set";
        public TimeSpan Period { get; init; }
        public Func<DateTime>? UpdateNextReset { get; init; }
        private DateTime NextReset { get; set; }

        public void Draw()
        {
            Update();

            ImGui.PushID(Label);

            var contentWidth = ImGui.GetContentRegionAvail();
            var textWidth = contentWidth.X * 0.30f;
            var progressBarWidth = contentWidth * 0.50f;
            var textSize = ImGui.CalcTextSize(Label);

            ImGui.Text(Label);

            ImGui.SameLine(textWidth);
            var cursorStartPosition = ImGui.GetCursorPos();

            var remainingTime = NextReset - DateTime.UtcNow;
            var deltaTime = 1.0f - (float)(remainingTime / Period);
            ImGui.ProgressBar(deltaTime, progressBarWidth with {Y = textSize.Y}, "");

            textSize = ImGui.CalcTextSize(FormatTimeSpan(remainingTime, Settings.HideSeconds));
            var textStartX = contentWidth.X - textSize.X - 20.0f * ImGuiHelpers.GlobalScale;

            ImGui.SetCursorPos(new Vector2(textStartX, cursorStartPosition.Y));

            ImGui.Text(FormatTimeSpan(remainingTime, Settings.HideSeconds));

            ImGui.PopID();
        }

        private void Update()
        {
            if (DateTime.UtcNow >= NextReset)
            {
                if (UpdateNextReset == null)
                {
                    throw new NotImplementedException();
                }

                NextReset = UpdateNextReset();
            }
        }

        private static string FormatTimeSpan(TimeSpan span, bool hideSeconds = false)
        {
            var baseString = span.Days > 0 ? $"{span.Days:0}:{span.Hours:00}:{span.Minutes:00}" : $"{span.Hours:00}:{span.Minutes:00}";

            if (hideSeconds == false)
            {
                return baseString + $":{span.Seconds:00}";
            }
            else
            {
                return baseString;
            }
        }
    }
}
