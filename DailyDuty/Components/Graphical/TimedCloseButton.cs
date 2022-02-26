using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Interfaces;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.Components.Graphical
{
    internal class TimedCloseButton : IDrawable
    {
        private readonly Stopwatch timer = new();
        private readonly Action functionAction;

        private bool timerStarted = false;

        public TimedCloseButton(Action function)
        {
            functionAction = function;
        }

        public void Draw()
        {
            DoOnce();

            if (timer.Elapsed.Seconds < 15)
            {
                var xPosition = ImGui.GetWindowWidth() - 250 * ImGuiHelpers.GlobalScale;
                var yPosition = ImGui.GetWindowHeight() - 25 * ImGuiHelpers.GlobalScale;
            
                ImGui.SetCursorPos(new Vector2(xPosition, yPosition));

                ImGui.TextColored(new(1.0f, 1.0f, 1.0f, 0.5f) ,$"You can close this window in {15 - timer.Elapsed.Seconds} seconds...");
            }
            else
            {
                timer.Stop();

                var xPosition = ImGui.GetWindowWidth() - 105 * ImGuiHelpers.GlobalScale;
                var yPosition = ImGui.GetWindowHeight() - 30 * ImGuiHelpers.GlobalScale;

                ImGui.SetCursorPos(new Vector2(xPosition, yPosition));

                if (ImGui.Button($"Thanks!", ImGuiHelpers.ScaledVector2(100, 25)))
                {
                    functionAction();
                }
            }
        }

        private void DoOnce()
        {
            if (timerStarted != false) return;

            timer.Start();
            timerStarted = true;
        }

        public void Reset()
        {
            timer.Restart();
            timer.Stop();
            timerStarted = false;
        }
    }
}
