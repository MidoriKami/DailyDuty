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
        public Vector4 BgColor => Service.Configuration.TimerSettings.JumboCactpotCountdownBgColor;
        public bool ShortStrings => Service.Configuration.TimersWindowSettings.ShortStrings;

        private DateTime NextReset => Service.Configuration.Current().JumboCactpot.NextReset;
        void ICountdownTimer.DrawContents()
        {
            var now = DateTime.UtcNow;
            var totalHours = NextReset - now;
            var percentage = (float) (1 - totalHours / TimeSpan.FromDays(7) );

            if (ShortStrings)
            {
                Draw.DrawProgressBar(percentage, "Cactpot", totalHours, ImGuiHelpers.ScaledVector2(ElementWidth, 20), Color, BgColor);
            }
            else
            {
                Draw.DrawProgressBar(percentage, "Jumbo Cactpot", totalHours, ImGuiHelpers.ScaledVector2(ElementWidth, 20), Color, BgColor);
            }
            
        }
    }
}