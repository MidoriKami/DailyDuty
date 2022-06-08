using DailyDuty.Data.Components;
using DailyDuty.Enums;

namespace DailyDuty.Data.ModuleSettings
{
    public class DutyRouletteSettings : GenericSettings
    {
        public bool EnableClickableLink = false;
        public bool HideWhenCapped = false;
        public TrackedRoulette[] TrackedRoulettes =
        {
            new()
            {
                Type = RouletteType.Expert
            },
            new()
            {
                Type = RouletteType.Level90
            },
            new()
            {
                Type = RouletteType.Level50607080
            },
            new()
            {
                Type = RouletteType.Leveling
            },
            new()
            {
                Type = RouletteType.Trials
            },
            new()
            {
                Type = RouletteType.MSQ
            },
            new()
            {
                Type = RouletteType.Guildhest
            },
            new()
            {
                Type = RouletteType.Alliance
            },
            new()
            {
                Type = RouletteType.Normal
            },
            new()
            {
                Type = RouletteType.Frontline
            },
            new()
            {
                Type = RouletteType.Mentor
            }
        };
    }
}