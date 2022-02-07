using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyDuty.Data.ModuleData.WondrousTails
{
    public enum ButtonState
    {
        // Needs instance completion to become available
        Completable = 0b00,

        // Can click button to get a stamp right now
        AvailableNow = 0b01,

        // Already completed, needs re-roll
        Unavailable = 0b10,

        // Data is state, unknown state
        Unknown = 0b11
    }
}
