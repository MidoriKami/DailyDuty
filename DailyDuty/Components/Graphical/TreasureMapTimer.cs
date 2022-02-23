using System;
using System.Numerics;
using DailyDuty.Data.Graphical;
using DailyDuty.Data.SettingsObjects.Timers;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using Dalamud.Interface;

namespace DailyDuty.Components.Graphical
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

            timerData.RemainingTime = nextAvailableTime - now;

            Draw.Timer(Settings.TimerStyle, timerData);
        }
    }
}