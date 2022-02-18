using System;
using System.Numerics;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using Dalamud.Interface;

namespace DailyDuty.Components.Graphical;

internal class DailyResetCountdown : ICountdownTimer
{
    bool ICountdownTimer.Enabled => Service.Configuration.TimerSettings.DailyCountdownEnabled;
    public int ElementWidth => Service.Configuration.TimerSettings.TimerWidth;
    public Vector4 Color => Service.Configuration.TimerSettings.DailyCountdownColor;
    public Vector4 BgColor => Service.Configuration.TimerSettings.DailyCountdownBgColor;
    public bool ShortStrings => Service.Configuration.TimersWindowSettings.ShortStrings;

    void ICountdownTimer.DrawContents()
    {
        var now = DateTime.UtcNow;
        var totalHours = Time.NextDailyReset() - now;
        var percentage = (float) (1 - totalHours / TimeSpan.FromDays(1) );

        if (ShortStrings)
        {
            Draw.DrawProgressBar(percentage, "Day", totalHours, ImGuiHelpers.ScaledVector2(ElementWidth, 20), Color, BgColor);
        }
        else
        {
            Draw.DrawProgressBar(percentage, "Daily Reset", totalHours, ImGuiHelpers.ScaledVector2(ElementWidth, 20), Color, BgColor);
        }
        
    }
}