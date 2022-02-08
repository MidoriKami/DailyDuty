using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Data.ModuleData.DutyRoulette;
using DailyDuty.Modules.Daily;

namespace DailyDuty.Data.SettingsObjects.DailySettings;

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