using System;
using System.Numerics;
using DailyDuty.Configuration.Components;
using DailyDuty.DataStructures;
using DailyDuty.Interfaces;
using DailyDuty.System.Localization;
using DailyDuty.UserInterface.Components.InfoBox;
using DailyDuty.Utilities;

namespace DailyDuty.Configuration.ModuleSettings;

public enum TrackedHuntState
{
    Unobtained,
    Obtained,
    Killed
}

public record TrackedHunt(HuntMarkType HuntType, TrackedHuntState State, Setting<bool> Tracked) : IInfoBoxTableRow
{
    public Tuple<Action?, Action?> GetInfoBoxTableRow()
    {
        return new Tuple<Action?, Action?>(
            Actions.GetStringAction(HuntType.GetLabel()),
            Actions.GetStringAction(State.GetLocalizedString(), color: State.GetColor()));
    }

    public TrackedHuntState State { get; set; } = State;
}

public class HuntMarksDailySettings : GenericSettings
{
    public readonly TrackedHunt[] TrackedHunts = 
    {
        new(HuntMarkType.RealmRebornLevelOne, TrackedHuntState.Unobtained, new Setting<bool>(false)),
        new(HuntMarkType.HeavenswardLevelOne, TrackedHuntState.Unobtained, new Setting<bool>(false)),
        new(HuntMarkType.HeavenswardLevelTwo, TrackedHuntState.Unobtained, new Setting<bool>(false)),
        new(HuntMarkType.HeavenswardLevelThree, TrackedHuntState.Unobtained, new Setting<bool>(false)),
        new(HuntMarkType.StormbloodLevelOne, TrackedHuntState.Unobtained, new Setting<bool>(false)),
        new(HuntMarkType.StormbloodLevelTwo, TrackedHuntState.Unobtained, new Setting<bool>(false)),
        new(HuntMarkType.StormbloodLevelThree, TrackedHuntState.Unobtained, new Setting<bool>(false)),
        new(HuntMarkType.ShadowbringersLevelOne, TrackedHuntState.Unobtained, new Setting<bool>(false)),
        new(HuntMarkType.ShadowbringersLevelTwo, TrackedHuntState.Unobtained, new Setting<bool>(false)),
        new(HuntMarkType.ShadowbringersLevelThree, TrackedHuntState.Unobtained, new Setting<bool>(false)),
        new(HuntMarkType.EndwalkerLevelOne, TrackedHuntState.Unobtained, new Setting<bool>(false)),
        new(HuntMarkType.EndwalkerLevelTwo, TrackedHuntState.Unobtained, new Setting<bool>(false)),
        new(HuntMarkType.EndwalkerLevelThree, TrackedHuntState.Unobtained, new Setting<bool>(false)),
    };
}

public static class TrackedHuntStateExtensions
{
    public static string GetLocalizedString(this TrackedHuntState state)
    {
        return state switch
        {
            TrackedHuntState.Unobtained => Strings.Module.HuntMarks.Unobtained,
            TrackedHuntState.Obtained => Strings.Module.HuntMarks.Obtained,
            TrackedHuntState.Killed => Strings.Module.HuntMarks.Killed,
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
        };
    }

    public static Vector4 GetColor(this TrackedHuntState state)
    {
        return state switch
        {
            TrackedHuntState.Unobtained => Colors.Red,
            TrackedHuntState.Obtained => Colors.Orange,
            TrackedHuntState.Killed => Colors.Green,
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
        };
    }
}