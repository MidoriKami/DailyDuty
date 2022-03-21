using System.Linq;
using System.Collections.Generic;
using DailyDuty.Interfaces;
using DailyDuty.Data.SettingsObjects.Windows;
using ImGuiNET;

namespace DailyDuty.Timers
{
    internal class CountdownTimers : IDrawable
    {
        public readonly List<ITimer> Timers;

        public CountdownTimers(List<ITimer> timers)
        {
            Timers = timers;
        }

        public void Draw()
        {
            var timers = Timers.Where(x => x.Settings.Enabled).ToArray();
            var totalWidth = (int)ImGui.GetContentRegionAvail().X;
            var spacing = (int)ImGui.GetStyle().ItemSpacing.X;
            var i = 0;

            while(i < timers.Length)
            {
                var width = 0;
                var count = 0;
                var resizeCount = 0;

                for(var j = i; j < timers.Length; j++)
                {
                    var timer = timers[j];
                    var w = timer.Settings.TimerStyle.Size + (count > 0 ? spacing : 0);
                    if(count > 0 && width + w > totalWidth)
                        break;

                    count++;
                    width += w;
                    if(timer.Settings.TimerStyle.StretchToFit)
                        resizeCount++;
                }

                var add = resizeCount > 0 ? ((totalWidth - width) / resizeCount) : 0;

                for(var j = i; j < i + count; j++)
                {
                    var timer = timers[j];

                    if(timer.Settings.TimerStyle.StretchToFit)
                    {
                        timer.Settings.TimerStyle.Size += add;
                        timer.Draw();
                        timer.Settings.TimerStyle.Size -= add;
                    }
                    else
                    {
                        timer.Draw();
                    }

                    if(j < i + count - 1)
                        ImGui.SameLine();
                }

                i += count;
            }
        }

        public int EnabledTimersCount()
        {
            return Timers.Count(timer => timer.Settings.Enabled);
        }
    }
}