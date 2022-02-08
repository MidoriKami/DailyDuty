using System.Collections.Generic;
using DailyDuty.Interfaces;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.Components.Graphical;

internal class AllCountdownTimers : IDrawable
{
    private readonly List<ICountdownTimer> timers = new()
    {
        new DailyResetCountdown(),
        new WeeklyResetCountdown(),
        new FashionReportResetCountdown(),
        new TreasureMapCountdown(),
        new JumboCactpotResetCountdown()
    };

    public void Draw()
    {
        int index = 0;

        foreach (var timer in timers)
        {
            if (timer.Enabled)
            {
                if (index > 0)
                {
                    var progressBarWidth = (Service.Configuration.TimerSettings.TimerWidth + 5) * ImGuiHelpers.GlobalScale;
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