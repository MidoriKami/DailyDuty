using System;
using System.Numerics;
using DailyDuty.DataStructures.HuntMarks;
using DailyDuty.Localization;
using KamiLib.Configuration;
using KamiLib.Drawing;
using KamiLib.Interfaces;

namespace DailyDuty.DataModels;

public enum TrackedHuntState
{
    Unobtained,
    Obtained,
    Killed
}

public record TrackedHunt(HuntMarkType HuntType, TrackedHuntState State, Setting<bool> Tracked) : IInfoBoxListConfigurationRow, IInfoBoxTableDataRow
{
    public TrackedHuntState State { get; set; } = State;

    public void GetDataRow(InfoBoxTable owner)
    {
        owner
            .BeginRow()
            .AddString(HuntType.GetLabel())
            .AddString(State.GetTranslatedString(), State.GetColor())
            .EndRow();
    }

    public void GetConfigurationRow(InfoBoxList owner)
    {
        owner.AddConfigCheckbox(HuntType.GetLabel(), Tracked);
    }
}

public static class TrackedHuntStateExtensions
{
    public static string GetTranslatedString(this TrackedHuntState state)
    {
        return state switch
        {
            TrackedHuntState.Unobtained => Strings.HuntMarks_Unobtained,
            TrackedHuntState.Obtained => Strings.HuntMarks_Obtained,
            TrackedHuntState.Killed => Strings.HuntMarks_Killed,
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