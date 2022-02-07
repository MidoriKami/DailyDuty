using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Data.Enums;

namespace DailyDuty.Data.ModuleData.HuntMarks
{
    public class TrackedHunt
    {
        public readonly ExpansionType Expansion;
        public bool Tracked;
        public bool Obtained;

        public TrackedHunt(ExpansionType expansion, bool tracked)
        {
            Expansion = expansion;
            Tracked = tracked;
            Obtained = false;
        }
    }
}
