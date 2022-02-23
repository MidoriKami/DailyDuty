using System.Collections.Generic;
using DailyDuty.Interfaces;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.Components.Graphical
{
    internal class CountdownTimers : IDrawable
    {
        public readonly List<ITimer> Timers;

        public CountdownTimers(List<ITimer> timers)
        {
            this.Timers = timers;
        }

        public void Draw()
        {
            int index = 0;

            foreach (var timer in Timers)
            {
                if (timer.Settings.Enabled)
                {
                    if (index > 0)
                    {
                        var progressBarWidth = (timer.Settings.TimerStyle.Size + 5) * ImGuiHelpers.GlobalScale;
                        var remainingWidth = ImGui.GetContentRegionAvail().X - (progressBarWidth * index);

                        if (remainingWidth > progressBarWidth)
                        {
                            ImGui.SameLine();
                        }
                        else
                        {
                            index = 0;
                        }
                    }

                    timer.Draw();

                    index++;
                }
            }
        }
    }
}