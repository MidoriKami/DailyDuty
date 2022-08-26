using DailyDuty.Configuration.Components;
using DailyDuty.DataStructures;

namespace DailyDuty.Configuration.ModuleSettings;

public class HuntMarksWeeklySettings : GenericSettings
{
    public TrackedHunt[] TrackedHunts = 
    {
        new(HuntMarkType.RealmRebornElite, TrackedHuntState.Unobtained, new Setting<bool>(false)),
        new(HuntMarkType.HeavenswardElite, TrackedHuntState.Unobtained, new Setting<bool>(false)),
        new(HuntMarkType.StormbloodElite, TrackedHuntState.Unobtained, new Setting<bool>(false)),
        new(HuntMarkType.ShadowbringersElite, TrackedHuntState.Unobtained, new Setting<bool>(false)),
        new(HuntMarkType.EndwalkerElite, TrackedHuntState.Unobtained, new Setting<bool>(false)),
    };
}