using System;
using System.Numerics;
using DailyDuty.Data.Graphical;
using DailyDuty.Data.SettingsObjects.Timers;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using Dalamud.Interface;

namespace DailyDuty.Components.Graphical
{
    internal class DailyResetTimer : ITimer
    {
        public string Name => "Daily";
        public TimerSettings Settings { get; set; }

        private readonly TimerData timerData = new()
        {
            NameLong = "Daily Reset",
            NameShort = "Daily",
            CompletionString = "",
            TimePeriod = TimeSpan.FromDays(1),
        };

        public DailyResetTimer(TimerSettings settings)
        {
            this.Settings = settings;
        }

        void ITimer.DrawContents()
        {
            var now = DateTime.UtcNow;

            timerData.RemainingTime = Time.NextDailyReset() - now;

            Draw.Timer(Settings.TimerStyle, timerData);
        }
    }
}