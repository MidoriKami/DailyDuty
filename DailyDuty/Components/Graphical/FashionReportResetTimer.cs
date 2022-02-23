using System;
using System.Numerics;
using DailyDuty.Data.Graphical;
using DailyDuty.Data.SettingsObjects.Timers;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using Dalamud.Interface;

namespace DailyDuty.Components.Graphical
{
    internal class FashionReportResetTimer : ITimer
    {
        public string Name => "Fashion Report";
        public TimerSettings Settings { get; set; }

        private readonly TimerData timerData = new()
        {
            NameLong = "Fashion Report",
            NameShort = "Fashion",
            CompletionString = "Available",
            TimePeriod = TimeSpan.FromDays(3),
        };

        public FashionReportResetTimer(TimerSettings settings)
        {
            this.Settings = settings;
        }

        void ITimer.DrawContents()
        {
            var now = DateTime.UtcNow;

            var fashionReportOpen = Time.NextFashionReportReset();
            var fashionReportClose = Time.NextWeeklyReset();

            if (now > fashionReportOpen && now < fashionReportClose)
            {
                timerData.RemainingTime = TimeSpan.Zero;
            }
            else
            {
                timerData.RemainingTime =  fashionReportOpen - now;
            }

            Draw.Timer(Settings.TimerStyle, timerData);
        }
    }
}