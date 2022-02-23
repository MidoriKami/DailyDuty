using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Components.Graphical;
using DailyDuty.Interfaces;

namespace DailyDuty.System
{
    public class TimerManager : IDisposable
    {
        public List<ITimer> GetSettingsWindowTimers()
        {
            var settings = Service.Configuration.TimersSettings;

            return  new()
            {
                new DailyResetTimer(settings.Daily),
                new WeeklyResetTimer(settings.Weekly),
                new FashionReportResetTimer(settings.FashionReport),
                new TreasureMapTimer(settings.TreasureMap),
                new JumboCactpotResetTimer(settings.JumboCactpot)
            };
        }

        public List<ITimer> GetTimersWindowTimers()
        {
            var settings = Service.Configuration.TimersWindowSettings;

            return  new()
            {
                new DailyResetTimer(settings.Daily),
                new WeeklyResetTimer(settings.Weekly),
                new FashionReportResetTimer(settings.FashionReport),
                new TreasureMapTimer(settings.TreasureMap),
                new JumboCactpotResetTimer(settings.JumboCactpot)
            };
        }

        public void Dispose()
        {
        }
    }
}
