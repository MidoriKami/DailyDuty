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
            new() { Type = HuntMarkType.RealmReborn_LevelOne, State = TrackedHuntState.Unobtained, Tracked = false},
            new() { Type = HuntMarkType.Heavensward_LevelOne, State = TrackedHuntState.Unobtained, Tracked = false},
            new() { Type = HuntMarkType.Heavensward_LevelTwo, State = TrackedHuntState.Unobtained, Tracked = false},
            new() { Type = HuntMarkType.Heavensward_LevelThree, State = TrackedHuntState.Unobtained, Tracked = false},
            new() { Type = HuntMarkType.Stormblood_LevelOne, State = TrackedHuntState.Unobtained, Tracked = false},
            new() { Type = HuntMarkType.Stormblood_LevelTwo, State = TrackedHuntState.Unobtained, Tracked = false},
            new() { Type = HuntMarkType.Stormblood_LevelThree, State = TrackedHuntState.Unobtained, Tracked = false},
            new() { Type = HuntMarkType.Shadowbringers_LevelOne, State = TrackedHuntState.Unobtained, Tracked = false},
            new() { Type = HuntMarkType.Shadowbringers_LevelTwo, State = TrackedHuntState.Unobtained, Tracked = false},
            new() { Type = HuntMarkType.Shadowbringers_LevelThree, State = TrackedHuntState.Unobtained, Tracked = false},
            new() { Type = HuntMarkType.Endwalker_LevelOne, State = TrackedHuntState.Unobtained, Tracked = false},
            new() { Type = HuntMarkType.Endwalker_LevelTwo, State = TrackedHuntState.Unobtained, Tracked = false},
            new() { Type = HuntMarkType.Endwalker_LevelThree, State = TrackedHuntState.Unobtained, Tracked = false},
        };
    }
}
