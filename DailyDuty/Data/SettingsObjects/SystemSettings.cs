using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyDuty.Data.SettingsObjects
{
    [Serializable]
    public class SystemSettings
    {
        public DateTime NextDailyReset = new();
        public DateTime NextWeeklyReset = new();
        public int ZoneChangeDelayRate = 1;
    }
}
