using DailyDuty.Enums;

namespace DailyDuty.Data.Components
{
    public class TrackedRoulette
    {
        public RouletteType Type { get; init; }
        public bool Tracked = false;
        public bool Completed = false;
    }
}
