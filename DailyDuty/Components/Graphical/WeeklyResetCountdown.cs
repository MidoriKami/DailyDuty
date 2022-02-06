using System;
using System.Numerics;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;

namespace DailyDuty.Components.Graphical
{
    internal class WeeklyResetCountdown : IDrawable
    {
        public void Draw()
        {
            var now = DateTime.UtcNow;
            var totalHours = Time.NextWeeklyReset() - now;
            var percentage = (float) (1 - totalHours / TimeSpan.FromDays(7) );

            Utilities.Draw.DrawProgressBar(percentage, "Weekly Reset", totalHours, new Vector2(200, 20), Colors.Purple);
        }
    }
}
