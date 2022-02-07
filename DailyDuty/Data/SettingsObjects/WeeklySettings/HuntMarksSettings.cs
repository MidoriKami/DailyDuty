using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Data.Enums;
using DailyDuty.Data.ModuleData.HuntMarks;
// ReSharper disable FieldCanBeMadeReadOnly.Global

namespace DailyDuty.Data.SettingsObjects.WeeklySettings;

public class HuntMarksSettings : GenericSettings
{
    public TrackedHunt[] TrackedHunts = 
    {
        new (ExpansionType.RealmReborn, false),
        new (ExpansionType.Heavensward, false),
        new (ExpansionType.Stormblood, false),
        new (ExpansionType.Shadowbringers, false),
        new (ExpansionType.Endwalker, false)
    };
}

