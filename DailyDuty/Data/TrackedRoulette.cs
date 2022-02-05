using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyDuty.Data
{
    public enum DutyRoulette
    {
        Expert = 5,
        Level90 = 8,
        Level50607080 = 2,
        Leveling = 1,
        Trials = 6,
        MSQ = 3,
        Guildhest = 4,
        Alliance = 15,
        Normal = 17,
        Mentor = 9,
        Frontline = 7
    }

    public class TrackedRoulette
    {
        public DutyRoulette Roulette;
        public bool Tracked;
        public bool Completed;

        public TrackedRoulette(DutyRoulette roulette)
        {
            Roulette = roulette;
            Tracked = false;
            Completed = false;
        }
    }
}
