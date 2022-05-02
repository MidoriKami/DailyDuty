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
            new (){ Type = HuntMarkType.RealmRebornElite, Tracked = false, State = TrackedHuntState.Unobtained },
            new (){ Type = HuntMarkType.HeavenswardElite, Tracked = false, State = TrackedHuntState.Unobtained },
            new (){ Type = HuntMarkType.StormbloodElite, Tracked = false, State = TrackedHuntState.Unobtained },
            new (){ Type = HuntMarkType.ShadowbringersElite, Tracked = false, State = TrackedHuntState.Unobtained },
            new (){ Type = HuntMarkType.EndwalkerElite, Tracked = false, State = TrackedHuntState.Unobtained },
        };
    }
}

