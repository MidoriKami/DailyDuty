using System;
using System.Numerics;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using Dalamud.Interface;

namespace DailyDuty.Components.Graphical
{
    internal class TreasureMapCountdown : ICountdownTimer
    {
        public bool Enabled => Service.Configuration.TimerSettings.TreasureMapCountdownEnabled;
        public int ElementWidth => Service.Configuration.TimerSettings.TimerWidth;
        public Vector4 Color => Service.Configuration.TimerSettings.TreasureMapCountdownColor;
        public Vector4 BgColor => Service.Configuration.TimerSettings.TreasureMapCountdownBgColor;
        public bool ShortStrings => Service.Configuration.TimersWindowSettings.ShortStrings;

        void ICountdownTimer.DrawContents()
        {
            var now = DateTime.Now;

            var harvestTime = Service.Configuration.Current().TreasureMap.LastMapGathered;
            var nextAvailableTime = harvestTime.AddHours(18);

            var timeRemaining = nextAvailableTime - now;
            var percentage = (float)(1 - timeRemaining / TimeSpan.FromHours(18));

            if (now > nextAvailableTime)
            {
                percentage = 1.0f;
                timeRemaining = TimeSpan.Zero;
            }

            if (ShortStrings)
            {
                Draw.DrawProgressBar(percentage, "Map", timeRemaining, ImGuiHelpers.ScaledVector2(ElementWidth, 20), Color, BgColor);
            }
            else
            {
                Draw.DrawProgressBar(percentage, "Treasure Map", timeRemaining, ImGuiHelpers.ScaledVector2(ElementWidth, 20), Color, BgColor);
            }
            
        }
    }
}