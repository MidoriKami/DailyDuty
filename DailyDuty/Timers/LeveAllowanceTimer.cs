using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Data.Graphical;
using DailyDuty.Data.SettingsObjects.Timers;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;

namespace DailyDuty.Timers
{
    internal class LeveAllowanceTimer : ITimer
    {
        public string Name => "Leve Allowances";
        public TimerSettings Settings { get; set; }

        private readonly TimerData timerData = new()
        {
            NameLong = "Leve Allowances",
            NameShort = "Leve",
            CompletionString = "Available",
            TimePeriod = TimeSpan.FromDays(1),
        };

        public LeveAllowanceTimer(TimerSettings settings)
        {
            this.Settings = settings;
        }

        void ITimer.DrawContents()
        {
            var now = DateTime.UtcNow;

            timerData.RemainingTime = Time.NextLeveAllowanceReset() - now;

            Draw.Timer(Settings.TimerStyle, timerData);
        }
    }
}
