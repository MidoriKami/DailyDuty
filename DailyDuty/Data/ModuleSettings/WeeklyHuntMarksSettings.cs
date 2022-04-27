using DailyDuty.Data.Components;
using DailyDuty.Enums;
using DailyDuty.Structs;

namespace DailyDuty.Data.ModuleSettings
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

