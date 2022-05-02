using DailyDuty.Enums;
using DailyDuty.Structs;

namespace DailyDuty.Data.Components
{
    public class TrackedHunt
    {
        public HuntMarkType Type { get; init; }
        public bool Tracked = false;
        public TrackedHuntState State { get; set; }
    }
}
