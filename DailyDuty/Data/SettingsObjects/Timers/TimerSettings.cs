using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Data.Graphical;

namespace DailyDuty.Data.SettingsObjects.Timers
{
    public class TimerSettings
    {
        public bool Enabled = false;
        public TimerStyle TimerStyle = new();
    }
}
