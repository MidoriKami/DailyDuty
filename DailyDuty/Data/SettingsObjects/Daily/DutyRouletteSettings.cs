using DailyDuty.Data.ModuleData.DutyRoulette;

namespace DailyDuty.Data.SettingsObjects.Daily
{
    public class DutyRouletteSettings : GenericSettings
    {
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