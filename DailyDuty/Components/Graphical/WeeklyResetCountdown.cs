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
    public Vector4 BgColor => Service.Configuration.TimerSettings.WeeklyCountdownBgColor;
    public int ElementWidth => Service.Configuration.TimerSettings.TimerWidth;
    public bool ShortStrings => Service.Configuration.TimersWindowSettings.ShortStrings;

    void ICountdownTimer.DrawContents()
    {
        var now = DateTime.UtcNow;
        var totalHours = Time.NextWeeklyReset() - now;
        var percentage = (float) (1 - totalHours / TimeSpan.FromDays(7) );

        if (ShortStrings)
        {
            Draw.DrawProgressBar(percentage, "Week", totalHours, ImGuiHelpers.ScaledVector2(ElementWidth, 20), Color, BgColor);
        }
        else
        {
            Draw.DrawProgressBar(percentage, "Weekly Reset", totalHours, ImGuiHelpers.ScaledVector2(ElementWidth, 20), Color, BgColor);
        }
        
    }
}