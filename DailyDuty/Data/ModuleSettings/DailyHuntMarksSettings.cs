using DailyDuty.Data.Components;
using DailyDuty.Enums;
using DailyDuty.Structs;

namespace DailyDuty.Data.ModuleSettings
{
    public class DailyHuntMarksSettings : GenericSettings
    {
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public TrackedHunt[] TrackedHunts = 
        {
            new() { Type = HuntMarkType.RealmRebornLevelOne, State = TrackedHuntState.Unobtained, Tracked = false},
            new() { Type = HuntMarkType.HeavenswardLevelOne, State = TrackedHuntState.Unobtained, Tracked = false},
            new() { Type = HuntMarkType.HeavenswardLevelTwo, State = TrackedHuntState.Unobtained, Tracked = false},
            new() { Type = HuntMarkType.HeavenswardLevelThree, State = TrackedHuntState.Unobtained, Tracked = false},
            new() { Type = HuntMarkType.StormbloodLevelOne, State = TrackedHuntState.Unobtained, Tracked = false},
            new() { Type = HuntMarkType.StormbloodLevelTwo, State = TrackedHuntState.Unobtained, Tracked = false},
            new() { Type = HuntMarkType.StormbloodLevelThree, State = TrackedHuntState.Unobtained, Tracked = false},
            new() { Type = HuntMarkType.ShadowbringersLevelOne, State = TrackedHuntState.Unobtained, Tracked = false},
            new() { Type = HuntMarkType.ShadowbringersLevelTwo, State = TrackedHuntState.Unobtained, Tracked = false},
            new() { Type = HuntMarkType.ShadowbringersLevelThree, State = TrackedHuntState.Unobtained, Tracked = false},
            new() { Type = HuntMarkType.EndwalkerLevelOne, State = TrackedHuntState.Unobtained, Tracked = false},
            new() { Type = HuntMarkType.EndwalkerLevelTwo, State = TrackedHuntState.Unobtained, Tracked = false},
            new() { Type = HuntMarkType.EndwalkerLevelThree, State = TrackedHuntState.Unobtained, Tracked = false},
        };
    }
}
