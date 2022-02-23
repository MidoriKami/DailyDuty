using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DailyDuty.Data.Graphical
{
    internal class TimerData
    {
        public string NameShort = "ShortNameNotSet";
        public string NameLong = "LongNameNotSet";
        public string CompletionString = "CompletionStringNotSet";
        public TimeSpan RemainingTime { get; set; }
        public TimeSpan TimePeriod { get; init; }
        public float Percentage => (float) (1 - RemainingTime / TimePeriod);
    }
}
