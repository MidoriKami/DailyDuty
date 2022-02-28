using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Components.Graphical;
using DailyDuty.Data.Enums;
using DailyDuty.Data.SettingsObjects.Timers;
using DailyDuty.Interfaces;
using DailyDuty.Timers;

namespace DailyDuty.System
{
    public class TimerManager : IDisposable
    {
        public List<ITimer> GetTimers(WindowName window)
        {
            var settings = window switch
            {
                WindowName.Settings => Service.Configuration.Timers,
                WindowName.Timers => Service.Configuration.Windows.Timers.TimersSettings,
                _ => new TimersSettings()
            };

            return  new()
            {
                new DailyResetTimer(settings.Daily),
                new WeeklyResetTimer(settings.Weekly),
                new FashionReportResetTimer(settings.FashionReport),
                new TreasureMapTimer(settings.TreasureMap),
                new JumboCactpotResetTimer(settings.JumboCactpot),
                new LeveAllowanceTimer(settings.LeveAllowance),
            };
        }
        
        public void Dispose()
        {
        }
    }
}
