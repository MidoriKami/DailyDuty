using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Data.Enums;
using DailyDuty.Data.ModuleData.HuntMarks;

namespace DailyDuty.Data.SettingsObjects.Daily
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
