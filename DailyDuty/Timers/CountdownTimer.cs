using System;
using System.Numerics;
using DailyDuty.Data.Components;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.Timers
{
    public class CountdownTimer : IDrawable
    {
        private static MainWindowSettings Settings => Service.SystemConfiguration.Windows.MainWindow;
        public string Label { get; init; } = "Label Not Set";
        public string ShortLabel { get; init; } = "Short Label Not Set";
        public TimeSpan Period { get; init; }
        public Func<DateTime>? UpdateNextReset { get; init; }
        private DateTime NextReset { get; set; }
        public TimerSettings TimerSettings { get; init; } = new();

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

        public void DrawStyled()
        {
            Update();

            ImGui.PushStyleColor(ImGuiCol.FrameBg, TimerSettings.TimerStyle.BackgroundColor);
            ImGui.PushStyleColor(ImGuiCol.PlotHistogram, TimerSettings.TimerStyle.ForegroundColor);

            ImGui.BeginGroup();
            
            var remainingTime = NextReset - DateTime.UtcNow;
            var deltaTime = 1.0f - (float)(remainingTime / Period);
            var cursorStart = ImGui.GetCursorPos();
            ImGui.ProgressBar(deltaTime, new(TimerSettings.TimerStyle.Size, 20), "");

            ImGui.SetCursorPos(new Vector2(cursorStart.X + TimerSettings.TimerStyle.Padding, cursorStart.Y));
            ImGui.TextColored(TimerSettings.TimerStyle.TextColor, TimerSettings.TimerStyle.Options.UseShortName ? ShortLabel : Label);

            var timeText = Format(remainingTime);
            var timeTextSize = ImGui.CalcTextSize(timeText);
            ImGui.SetCursorPos(new Vector2(cursorStart.X + TimerSettings.TimerStyle.Size - TimerSettings.TimerStyle.Padding - timeTextSize.X, cursorStart.Y));
            ImGui.TextColored(TimerSettings.TimerStyle.TimeColor, timeText);

            ImGui.EndGroup();

            ImGui.PopStyleColor(2);
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

        public string Format(TimeSpan span)
        {
            var options = TimerSettings.TimerStyle.Options;
            
            string result = "";

            if (span.Days > 0)
            {
                if (options.CondensedDisplay)
                {
                    result = $"{span.Days}:";
                }
                else
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
            }

            result += $"{span.Hours:00}:{span.Minutes:00}";

            if (options.ShowSeconds)
            {
                result += $":{span.Seconds:00}";
            }

            return result;
        }
    }
}
