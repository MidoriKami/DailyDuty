using System;
using System.Numerics;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;

namespace DailyDuty.Components.Graphical
{
    internal class DailyResetCountdown : IDrawable
    {
        public void Draw()
        {
            var now = DateTime.UtcNow;
            var totalHours = Time.NextDailyReset() - now;
            var percentage = (float) (1 - totalHours / TimeSpan.FromDays(1) );

            Utilities.Draw.DrawProgressBar(percentage, "Daily Reset", totalHours, new Vector2(200, 20), Colors.Blue);
        }
    }
}
