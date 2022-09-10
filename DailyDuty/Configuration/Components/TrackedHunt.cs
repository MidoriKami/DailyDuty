using System;
using System.Numerics;
using DailyDuty.DataStructures;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.UserInterface.Components.InfoBox;
using DailyDuty.Utilities;

namespace DailyDuty.Configuration.Components;

public enum TrackedHuntState
{
    Unobtained,
    Obtained,
    Killed
}

public record TrackedHunt(HuntMarkType HuntType, TrackedHuntState State, Setting<bool> Tracked) : IInfoBoxTableRow
{
    public Tuple<Action?, Action?> GetConfigurationRow()
    {
        return new Tuple<Action?, Action?>(null, null);
    }

    public Tuple<Action?, Action?> GetDataRow()
    {
        return new Tuple<Action?, Action?>(
            Actions.GetStringAction(HuntType.GetLabel()),
            Actions.GetStringAction(State.GetTranslatedString(), color: State.GetColor()));
    }

    public TrackedHuntState State { get; set; } = State;
}

public static class TrackedHuntStateExtensions
{
    public static string GetTranslatedString(this TrackedHuntState state)
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