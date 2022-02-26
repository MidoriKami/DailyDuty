using DailyDuty.Data.ModuleData.HuntMarks;

// ReSharper disable FieldCanBeMadeReadOnly.Global

namespace DailyDuty.Data.SettingsObjects.Weekly
{
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
}

