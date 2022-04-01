using DailyDuty.Data.Enums;

namespace DailyDuty.Data.ModuleData.HuntMarks
{
    public class TrackedHunt
    {
        public ExpansionType Expansion { get; init; }
        public bool Tracked = false;
        public TrackedHuntState State { get; set; }
    }
}
