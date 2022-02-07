using System;
using System.Numerics;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;

namespace DailyDuty.Components.Graphical;

internal class DailyResetCountdown : ICountdownTimer
{
    bool ICountdownTimer.Enabled => Service.Configuration.TimerSettings.DailyCountdownEnabled;
    public int ElementWidth => Service.Configuration.TimerSettings.TimerWidth;
    public Vector4 Color => Service.Configuration.TimerSettings.DailyCountdownColor;

    void ICountdownTimer.DrawContents()
    {
        var now = DateTime.UtcNow;
        var totalHours = Time.NextDailyReset() - now;
        var percentage = (float) (1 - totalHours / TimeSpan.FromDays(1) );

        Draw.DrawProgressBar(percentage, "Daily Reset", totalHours, new Vector2(ElementWidth, 20), Color);
    }
}