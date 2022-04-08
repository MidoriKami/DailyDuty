using DailyDuty.Data.Enums;

namespace DailyDuty.Data.ModuleData.HuntMarks
{
    public class TrackedHunt
    {
        public HuntMarkType Type { get; init; }
        public bool Tracked = false;
        public TrackedHuntState State { get; set; }
    }
}
