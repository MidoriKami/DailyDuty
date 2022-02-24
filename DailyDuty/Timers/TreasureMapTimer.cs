using System;
using DailyDuty.Data.Graphical;
using DailyDuty.Data.SettingsObjects.Timers;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;

namespace DailyDuty.Timers
{
    internal class TreasureMapTimer : ITimer
    {
        public string Name => "Treasure Map";
        public TimerSettings Settings { get; set; }

        private readonly TimerData timerData = new()
        {
            NameLong = "Treasure Map",
            NameShort = "Map",
            CompletionString = "Available",
            TimePeriod = TimeSpan.FromHours(18),
        };

        public TreasureMapTimer(TimerSettings settings)
        {
            this.Settings = settings;
        }

        void ITimer.DrawContents()
        {
            var harvestTime = Service.Configuration.Current().TreasureMap.LastMapGathered;
            var nextAvailableTime = harvestTime.AddHours(18);

            // Use Local Time for Treasure Maps
            var now = DateTime.Now;

            timerData.RemainingTime = now > nextAvailableTime ? TimeSpan.Zero : nextAvailableTime - now;

            Draw.Timer(Settings.TimerStyle, timerData);
        }
    }
}