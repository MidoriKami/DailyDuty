using DailyDuty.Enums;

namespace DailyDuty.Data.Components
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
