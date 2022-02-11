using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using Dalamud.Interface;

namespace DailyDuty.Components.Graphical
{
    internal class JumboCactpotResetCountdown : ICountdownTimer
    {
        public bool Enabled => Service.Configuration.TimerSettings.JumboCactpotCountdownEnabled;
        public int ElementWidth => Service.Configuration.TimerSettings.TimerWidth;
        public Vector4 Color => Service.Configuration.TimerSettings.JumboCactpotCountdownColor;

        private DateTime NextReset => Service.Configuration.Current().JumboCactpot.NextReset;
        void ICountdownTimer.DrawContents()
        {
            var now = DateTime.UtcNow;
            var totalHours = NextReset - now;
            var percentage = (float) (1 - totalHours / TimeSpan.FromDays(7) );

            Draw.DrawProgressBar(percentage, "Jumbo Cactpot", totalHours, ImGuiHelpers.ScaledVector2(ElementWidth, 20), Color);
        }
    }
}
