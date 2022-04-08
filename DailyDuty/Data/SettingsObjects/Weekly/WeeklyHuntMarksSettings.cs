using DailyDuty.Data.Enums;
using DailyDuty.Data.ModuleData.HuntMarks;


namespace DailyDuty.Data.SettingsObjects.Weekly
{
    public class WeeklyHuntMarksSettings : GenericSettings
    {
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public TrackedHunt[] TrackedHunts = 
        {
            new (){ Type = HuntMarkType.RealmReborn_Elite, Tracked = false, State = TrackedHuntState.Unobtained },
            new (){ Type = HuntMarkType.Heavensward_Elite, Tracked = false, State = TrackedHuntState.Unobtained },
            new (){ Type = HuntMarkType.Stormblood_Elite, Tracked = false, State = TrackedHuntState.Unobtained },
            new (){ Type = HuntMarkType.Shadowbringers_Elite, Tracked = false, State = TrackedHuntState.Unobtained },
            new (){ Type = HuntMarkType.Endwalker_Elite, Tracked = false, State = TrackedHuntState.Unobtained },
        };
    }
}

