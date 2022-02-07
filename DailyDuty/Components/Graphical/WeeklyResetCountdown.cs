using System;
using System.Numerics;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using Dalamud.Interface;

namespace DailyDuty.Components.Graphical;

internal class WeeklyResetCountdown : ICountdownTimer
{
    bool ICountdownTimer.Enabled => Service.Configuration.TimerSettings.WeeklyCountdownEnabled;
    public Vector4 Color => Service.Configuration.TimerSettings.WeeklyCountdownColor;
    public int ElementWidth => Service.Configuration.TimerSettings.TimerWidth;

    void ICountdownTimer.DrawContents()
    {
        var now = DateTime.UtcNow;
        var totalHours = Time.NextWeeklyReset() - now;
        var percentage = (float) (1 - totalHours / TimeSpan.FromDays(7) );

        Draw.DrawProgressBar(percentage, "Weekly Reset", totalHours, ImGuiHelpers.ScaledVector2(ElementWidth, 20), Color);
    }
}