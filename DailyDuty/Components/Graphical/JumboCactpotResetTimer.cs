using System;
using System.Numerics;
using DailyDuty.Data.Graphical;
using DailyDuty.Data.SettingsObjects.Timers;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using Dalamud.Interface;

namespace DailyDuty.Components.Graphical
{
    internal class JumboCactpotResetTimer : ITimer
    {
        public string Name => "Jumbo Cactpot";
        public TimerSettings Settings { get; set; }

        private readonly TimerData timerData = new()
        {
            NameLong = "Jumbo Cactpot",
            NameShort = "JCactpot",
            CompletionString = "Available",
            TimePeriod = TimeSpan.FromDays(7),
        };

        private DateTime NextReset => Service.Configuration.Current().JumboCactpot.NextReset;

        public JumboCactpotResetTimer(TimerSettings settings)
        {
            this.Settings = settings;
        }

        void ITimer.DrawContents()
        {
            var now = DateTime.UtcNow;

            timerData.RemainingTime = NextReset - now;

            Draw.Timer(Settings.TimerStyle, timerData);
        }
    }
}