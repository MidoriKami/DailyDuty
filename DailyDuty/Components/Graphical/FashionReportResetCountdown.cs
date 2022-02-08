using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using Dalamud.Interface;

namespace DailyDuty.Components.Graphical;

internal class FashionReportResetCountdown : ICountdownTimer
{
    bool ICountdownTimer.Enabled => Service.Configuration.TimerSettings.FashionReportCountdownEnabled;
    public Vector4 Color => Service.Configuration.TimerSettings.FashionReportCountdownColor;
    public int ElementWidth => Service.Configuration.TimerSettings.TimerWidth;

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

        Draw.DrawProgressBar(percentage, "Fashion Report", totalHours, ImGuiHelpers.ScaledVector2(ElementWidth, 20), Color);
    }
}