using System;
using DailyDuty.Data.Graphical;
using DailyDuty.Data.SettingsObjects.Timers;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;

namespace DailyDuty.Timers
{
    internal class WeeklyResetTimer : ITimer
    {
        public string Name => "Weekly";
        public TimerSettings Settings { get; set; }

        private readonly TimerData timerData = new()
        {
            NameLong = "Weekly Reset",
            NameShort = "Weekly",
            CompletionString = "",
            TimePeriod = TimeSpan.FromDays(7),
        };

        public WeeklyResetTimer(TimerSettings settings)
        {
            this.Settings = settings;
        }

        void ITimer.DrawContents()
        {
            var now = DateTime.UtcNow;

            timerData.RemainingTime = Time.NextWeeklyReset() - now;

            Draw.Timer(Settings.TimerStyle, timerData);
        }
    }
}