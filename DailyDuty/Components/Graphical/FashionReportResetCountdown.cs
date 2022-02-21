using System;
using System.Numerics;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using Dalamud.Interface;

namespace DailyDuty.Components.Graphical;

internal class FashionReportResetCountdown : ICountdownTimer
{
    bool ICountdownTimer.Enabled => Service.Configuration.TimerSettings.FashionReportCountdownEnabled;
    public Vector4 Color => Service.Configuration.TimerSettings.FashionReportCountdownColor;
    public Vector4 BgColor => Service.Configuration.TimerSettings.FashionReportCountdownBgColor;
    public int ElementWidth => Service.Configuration.TimerSettings.TimerWidth;
    public bool ShortStrings => Service.Configuration.TimersWindowSettings.ShortStrings;

    void ICountdownTimer.DrawContents()
    {
        var now = DateTime.UtcNow;

        var fashionReportOpen = Time.NextFashionReportReset();
        var fashionReportClose = Time.NextWeeklyReset();

        var percentage = 0.0f;
        var totalHours = TimeSpan.Zero;

        if (now > fashionReportOpen && now < fashionReportClose)
        {
            percentage = 1.0f;
        }
        else
        {
            totalHours = fashionReportOpen - now;
            percentage = (float) (1 - totalHours / TimeSpan.FromDays(3) );
        }

        if (ShortStrings)
        {
            Draw.DrawProgressBar(percentage, "Fashion", totalHours, ImGuiHelpers.ScaledVector2(ElementWidth, 20), Color, BgColor);
        }
        else
        {
            Draw.DrawProgressBar(percentage, "Fashion Report", totalHours, ImGuiHelpers.ScaledVector2(ElementWidth, 20), Color, BgColor);
        }
        
    }
}