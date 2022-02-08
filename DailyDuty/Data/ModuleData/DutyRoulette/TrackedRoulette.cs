using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyDuty.Data.ModuleData.DutyRoulette
{
    public class TrackedRoulette
    {
        public RouletteType Type { get; }
        public bool Tracked = false;
        public bool Completed = false;

        public TrackedRoulette(RouletteType type)
        {
            Type = type;
        }
    }
}
