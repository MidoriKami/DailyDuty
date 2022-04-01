using DailyDuty.Data.Enums;
using DailyDuty.Data.ModuleData.HuntMarks;


namespace DailyDuty.Data.SettingsObjects.Weekly
{
    public class HuntMarksSettings : GenericSettings
    {
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public TrackedHunt[] TrackedHunts = 
        {
            new (){ Expansion = ExpansionType.RealmReborn, Tracked = false, State = TrackedHuntState.Unobtained },
            new (){ Expansion = ExpansionType.Heavensward, Tracked = false, State = TrackedHuntState.Unobtained },
            new (){ Expansion = ExpansionType.Stormblood, Tracked = false, State = TrackedHuntState.Unobtained },
            new (){ Expansion = ExpansionType.Shadowbringers, Tracked = false, State = TrackedHuntState.Unobtained },
            new (){ Expansion = ExpansionType.Endwalker, Tracked = false, State = TrackedHuntState.Unobtained },
        };
    }
}

