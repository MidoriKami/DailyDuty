using DailyDuty.Data.Components;
using DailyDuty.Enums;

namespace DailyDuty.Data.ModuleSettings
{
    public class DutyRouletteSettings : GenericSettings
    {
        public bool EnableClickableLink = false;
        public TrackedRoulette[] TrackedRoulettes =
        {
            new(RouletteType.Expert),
            new(RouletteType.Level90),
            new(RouletteType.Level50607080),
            new(RouletteType.Leveling),
            new(RouletteType.Trials),
            new(RouletteType.MSQ),
            new(RouletteType.Guildhest),
            new(RouletteType.Alliance),
            new(RouletteType.Normal),
            new(RouletteType.Frontline),
            new(RouletteType.Mentor)
        };
    }
}