using System;
using System.Numerics;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using KamiLib.Configuration;
using KamiLib.InfoBoxSystem;
using KamiLib.Interfaces;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.Configuration.Components;

public enum RouletteType
{
    Expert = 5,
    Level90 = 8,
    Level50607080 = 2,
    Leveling = 1,
    Trials = 6,
    MSQ = 3,
    Guildhest = 4,
    Alliance = 15,
    Normal = 17,
    Mentor = 9,
    Frontline = 7
}

public enum RouletteState
{
    Complete,
    Incomplete,
    Overriden
}

public static class RouletteTypeExtensions
{
    public static string GetTranslatedString(this RouletteType type)
    {
        return Service.DataManager.GetExcelSheet<ContentRoulette>()!.GetRow((uint) type)!.Category.RawString;
    }
}

public static class RouletteStateExtensions
{
    public static Vector4 GetColor(this RouletteState type)
    {
        return type switch
        {
            RouletteState.Complete => Service.ConfigurationManager.CharacterConfiguration.DutyRoulette.CompleteColor.Value,
            RouletteState.Incomplete => Service.ConfigurationManager.CharacterConfiguration.DutyRoulette.IncompleteColor.Value,
            RouletteState.Overriden => Service.ConfigurationManager.CharacterConfiguration.DutyRoulette.OverrideColor.Value,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    public static string GetTranslatedString(this RouletteState type)
    {
        return type switch
        {
            RouletteState.Complete => Strings.Common.Complete,
            RouletteState.Incomplete => Strings.Common.Incomplete,
            RouletteState.Overriden => Strings.Common.Overriden,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}


public record TrackedRoulette(RouletteType Roulette, Setting<bool> Tracked, RouletteState State) : IInfoBoxTableDataRow, IInfoBoxListConfigurationRow
{
    public RouletteState State { get; set; } = State;

    public void GetDataRow(InfoBoxTable owner)
    {
        owner
            .BeginRow()
            .AddString(Roulette.GetTranslatedString())
            .AddString(State.GetTranslatedString(), State.GetColor())
            .EndRow();
    }

    public void GetConfigurationRow(InfoBoxList owner)
    {
        owner.AddConfigCheckbox(Roulette.GetTranslatedString(), Tracked);
    }
}
